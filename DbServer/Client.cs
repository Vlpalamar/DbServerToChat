using System;
using System.Collections.Generic;
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
        public string Name;
 

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
            this.Name = u.name;
            if (Server.users.Count>0)
            {
                foreach (User user in Server.users)
                {
                    if (u.email==user.email && u.pswd== user.pswd)
                    {
                        response.code = ResponseCodes.OK;
                        response.success = true;
                        response.data = user;
                        sendResponse(ref response);
                        Name = u.name;
                        //Server.HelloNewUser();
                        return true;
                    }
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

                try
                {
                    if (!stream.DataAvailable) continue;
                }
                catch (Exception e)
                {
                    continue;
                }
                

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
                            this.Name = u.name;
                            response.data = u;
                            Server.users.Add(u);
                            //Server.HelloNewUser();
                            sendResponse( ref response);
                        }
                        break;
                    case Commands.SendMessageToAll:
                        if (request.entity== Entities.UserMessage)
                        {
                            UserMessage message =(UserMessage) request.data;
                            //Server.NewMessage();
                            Server.SendMessageToAll(message);
                        }
                        break;
                    case Commands.UserBye:
                        {
                            //Server.ByuUser();
                            Bye();
                        }
                        break;
                    case Commands.ShowUsers:
                        {
                        Server.SendUserListToAll();

                        }
                        break;
                    case Commands.AddToBlackList:
                        if (request.entity == Entities.User)
                        {

                           

                            Response response = new Response();
                            response.success = true;
                            response.code = ResponseCodes.OK;
                            User u = (User)request.data;
                            response.data = u;
                            foreach (User user in Server.users)
                            {
                                if (user.name== u.name)
                                {
                                    user.BlackList = u.BlackList;
                                }
                            }
                            sendResponse(ref response);
                        }
                        break;


                }
            }
        }

        public void SendUsers(List<User> users)
        {
            Response response = new Response();
            response.success = true;
            response.code = ResponseCodes.OK;
            response.Entity = Entities.UserList;
            response.data = users;
            sendResponse(ref response);
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
            if (this.Name != null) 
            {
                foreach (User user in Server.users)
                {
                    if (user.name==this.Name)
                    {
                        user.isOnline = false;
                        break;
                        
                    }
                }
            }
           
            stream.Close();
            tcpClient.Close();
        }
    }



}
