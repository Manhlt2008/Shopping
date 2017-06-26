using System;
using System.Net.Mail;

namespace WebApplication.Lib.Bll
{
    public class SendMail
    {
        public static string SendEmail(string fromAddress, string formAddressPassword, string toAddress, string subject, string body)
        {
            var result = "Message Sent Successfully..!!";
            
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(fromAddress, formAddressPassword),
                    Timeout = 30000,
                };

                var message = new MailMessage(fromAddress, toAddress, subject, body) {IsBodyHtml = true};

                smtp.Send(message);
            }
            catch (Exception)
            {
                result = "Result sending email.!!!";
            }
            return result;
        }
    }
}