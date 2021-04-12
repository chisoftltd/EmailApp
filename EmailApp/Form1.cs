using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmailApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string emailTo { get; private set; }
        public string emailFrom { get; private set; }
        public string emailSubject { get; private set; }
        public string emailBody { get; private set; }
        public string userState { get; private set; }
        public bool hasAttachment { get; private set; }
        public DialogResult openFDResults { get; private set; }
        public int numOfFiles { get; private set; }
        public int counter { get; private set; }
        public string[] aryAttachments { get; private set; }
        public string popHost { get; private set; }
        public string user { get; private set; }
        public string pass { get; private set; }
        public string returnString { get; private set; }
        public NetworkStream networkStream { get; private set; }
        public StreamReader readStream { get; private set; }

        private void btnSend_Click(object sender, EventArgs e)
        {
            emailTo = txtEmailTo.Text.Trim();
            emailFrom = txtEmailFrom.Text.Trim();
            emailSubject = txtEmailSubject.Text.Trim();
            emailBody = txtEmailBody.Text.Trim();

            SmtpClient smtpServer = new SmtpClient();

            smtpServer.Host = "df-svr-dc03.dawnfresh.local";
            smtpServer.Timeout = 60;
            smtpServer.Port = 25;

            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;

            smtpServer.UseDefaultCredentials = true;

            MailMessage message = new MailMessage(emailFrom, emailTo);
            message.Subject = emailSubject;
            message.Body = emailBody;
            message.BodyEncoding = Encoding.UTF8;
            message.Headers.Add("Reply-To", emailFrom);
            message.Headers.Add("X-Organization", "Home and Learn");

            if (hasAttachment)
            {
                for (int i = 0; i < aryAttachments.Length; i++)
                {
                    Attachment fileAttach = new Attachment(aryAttachments[i]);
                    message.Attachments.Add(fileAttach);
                }
            }

            smtpServer.SendCompleted += new SendCompletedEventHandler(DoSendCompleted);

            userState = "- Mail Message";

            try
            {
                smtpServer.SendAsync(message, userState);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DoSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string token = e.UserState.ToString();
            if (e.Cancelled)
            {
                MessageBox.Show("Sending Email Cancelled " + token);
            }
            if (e.Error != null)
            {
                MessageBox.Show(token + e.Error.ToString());
            }
            else
            {
                MessageBox.Show("Message Sent");
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            hasAttachment = false;

            txtAttach.Text = "";

            openFD.InitialDirectory = "C:\\";
            openFD.Title = "Attach Files";
            openFD.Filter = "PDFs|*.pdf|Text Files|*.txt|All Files|*.*";

            openFDResults = openFD.ShowDialog();

            if (openFDResults == DialogResult.Cancel)
            {
                hasAttachment = false;
                return;
            }

            try
            {
                numOfFiles = openFD.FileNames.Length;
                counter = 0;

                aryAttachments = new string[numOfFiles];
                foreach (string singleF in openFD.FileNames)
                {
                    aryAttachments[counter] = singleF;
                    txtAttach.Text += singleF + "";
                    counter++;

                }
                hasAttachment = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            TcpClient popServer = new TcpClient();
            //IPAddress ipAddress = Dns.GetHostEntry("df-svr-dc03.dawnfresh.local").AddressList[0];
            //IPAddress ipAddress = Dns.GetHostEntry("0.0.0.0").AddressList[0];
            //IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);
            popHost = "df-svr-dc03.dawnfresh.local";
            user = "Benjamin.chinwe@dawnfreshco.uk";
            pass = "9thMile@9thMile@";

            try
            {
                popServer.Connect("df-svr-dc03.dawnfresh.local", 8080);
                networkStream = popServer.GetStream();
                readStream = new StreamReader(networkStream);

                returnString = readStream.ReadLine() + "\r\n";
                txtServerResponse.Text = returnString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
