
namespace DbServer
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Log_txtBox = new System.Windows.Forms.RichTextBox();
            this.LogUsers_ListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // Log_txtBox
            // 
            this.Log_txtBox.Location = new System.Drawing.Point(12, 30);
            this.Log_txtBox.Name = "Log_txtBox";
            this.Log_txtBox.Size = new System.Drawing.Size(402, 362);
            this.Log_txtBox.TabIndex = 1;
            this.Log_txtBox.Text = "";
            // 
            // LogUsers_ListBox
            // 
            this.LogUsers_ListBox.FormattingEnabled = true;
            this.LogUsers_ListBox.Location = new System.Drawing.Point(529, 43);
            this.LogUsers_ListBox.Name = "LogUsers_ListBox";
            this.LogUsers_ListBox.Size = new System.Drawing.Size(120, 316);
            this.LogUsers_ListBox.TabIndex = 2;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.LogUsers_ListBox);
            this.Controls.Add(this.Log_txtBox);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox Log_txtBox;
        private System.Windows.Forms.ListBox LogUsers_ListBox;
    }
}