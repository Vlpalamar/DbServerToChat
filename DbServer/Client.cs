using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary1.Entities;
using ClassLibrary1.Enums;
using ClassLibrary1.Messages;

namespace DbServer
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private BinaryFormatter bf;
        private string Name;
 

        public Task task;

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.stream = this.tcpClient.GetStream();
            bf = new BinaryFormatter();
        }
        /// <summary>
        /// Проверяет пользователя
        /// </summary>
        /// <returns></returns>
        private bool auth()
        {
            Request request = receiveRequest();
            Response response = new Response();
            if (request.command == Commands.Create)
            {
                response.code = ResponseCodes.OK;
                response.success = true;
                sendResponse(ref response);
                return true;
            }
            if (request.command != Commands.UserLogin)
            {
                response.code = ResponseCodes.BagRequest;
                response.success = false;
                sendResponse(ref response);
                return false;
            }

            

            User u = (User)request.data;
            foreach (User user in Server.users)
            {
                if (u.email==user.email && u.pswd== user.pswd)
                {
                    response.code = ResponseCodes.OK;
                    response.success = true;
                    response.data = user;
                    sendResponse(ref response);
                    Name = u.name;
                    return true;
                }

            }

            response.code = ResponseCodes.Auth;
            response.success = false;
            sendResponse(ref response);
            return false;
        }


        /// <summary>
        /// Отослать ответ клиенту
        /// </summary>
        /// <param name="res">Подготовленный запрос</param>
        private void sendResponse(ref Response res)
        {
            try
            {
                bf.Serialize(stream, res);
            }
            catch (Exception ex)
            {
                Server.onError(" sendResponse: " + ex.Message);
            }
        }


        /// <summary>
        /// Получить запрос от клиента
        /// </summary>
        /// <returns></returns>
        private Request receiveRequest()
        {
            try
            {
                while (!stream.DataAvailable)
                {

                }
                return (Request)bf.Deserialize(stream);
            }
            catch (Exception ex)
            {
                Server.onError(" receiveRequest: " + ex.Message);
            }

            return null;
        }




        /// <summary>
        /// Основной цикл
        /// </summary>
        public void Run()
        {
            // Проверяем авторизацию
            if (!auth())
            {
                Bye();
                return;
            }

            // Если все ок - работаем с командами пользователя
            while (true)
            {
                if (Server.cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (!stream.DataAvailable) continue;

                Request request = receiveRequest();
                switch (request.command)
                {
                    case Commands.Create:
                        if (request.entity == Entities.User)
                        {
                       
                            MessageBox.Show(" Пришел запрос на регистрацию пользователя");
                            Response response = new Response();
                            response.success = true;
                            response.code = ResponseCodes.OK;
                            User u = (User)request.data;
                            response.data = u;
                            Server.users.Add(u);
                            sendResponse( ref response);


                        }
                        break;
                    case Commands.SendMessageToAll:
                        if (request.entity== Entities.UserMessage)
                        {
                            UserMessage message =(UserMessage) request.data;
                            Server.SendToAll(message);

                        }
                        break;
                }
            }
        }


        public void SendMesagge(UserMessage message)
        {
            Response response = new Response();
            response.success = true;
            response.code = ResponseCodes.OK;
            response.Entity = Entities.UserMessage;
            response.data = message;
            sendResponse(ref response);
        }

        /// <summary>
        /// Закрывает соединение
        /// </summary>
        public void Bye()
        {
            Server.tasks.Remove(task);
            Server.clients.Remove(this);
            stream.Close();
            tcpClient.Close();
        }
    }



}
