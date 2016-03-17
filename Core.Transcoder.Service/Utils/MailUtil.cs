using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service.Utils
{
    public static class MailUtil
    {

        public static void SendMail()
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;

                MailMessage mm = new MailMessage("Transcoder - Conversion en ligne", "malfatti.antonin@gmail.com", "subject", "body");
                mm.IsBodyHtml = true;
                client.Send(mm);
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not end email\n\n" + e.ToString());
            }

        }

        public static void SendMail(string typeDemande, TASK task)
        {
            try
            {
                if (task.USER.EMAIL == null)
                    return;

                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;

                MailMessage mm = GenerateMailMessage(typeDemande, task);
                client.Send(mm);

                TRACE Trace = new TRACE { FK_ID_TASK = task.PK_ID_TASK, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, FK_ID_SERVER = 1, DESCRIPTION = mm.ToString(), METHOD = "MAIL APRES CONVERSION", TYPE = "INFO" };
                new TRACE_Service().AddTrace(Trace);
            }
            catch (Exception e)
            {
                TRACE Trace = new TRACE { FK_ID_TASK = task.PK_ID_TASK, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, FK_ID_SERVER = 1, DESCRIPTION = e.Message, METHOD = "MAIL APRES CONVERSION", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
            }

        }

        public static MailMessage GenerateMailMessage(string typeDemande, TASK task)
        {
            MailMessage message = new MailMessage("transcodernoreply@gmail.com", task.USER.EMAIL);
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;

            switch (typeDemande)
            {
                case Enums.StringManager.CONVERSION_TERMINEE:
                    {
                        message.Subject = "Votre conversion n°" + task.PK_ID_TASK;
                        message.Body = "Bonjour " + task.USER.FIRSTNAME + " " + task.USER.LASTNAME + ", <br/>";
                        message.Body += "Votre conversion n° " + task.PK_ID_TASK + " a été effectuée. <br/>";
                        message.Body += "Vous pouvez télécharger votre média via ce lien <a href='#'>Cliquer ici</a><br/>";
                        message.Body += "Nous vous souhaitons une agréable journée. <br/>";
                        message.Body += "Cordialement, <br/>";
                        message.Body += "L'équipe TRANSCODER France <br/>";
                        break;
                    }
                case Enums.StringManager.DEMANDE_CONVERSION:
                    {
                        message.Subject = "Votre demande de conversion n°" + task.PK_ID_TASK;
                        message.Body = "Bonjour " + task.USER.FIRSTNAME + " " + task.USER.LASTNAME + ", <br/>";
                        message.Body += "Votre demande n° " + task.PK_ID_TASK + " a bien été prise en compte. <br/>";
                        message.Body += "Vous recevrez un mail lorsque votre conversion sera terminée et prête a être téléchargée. <br/>";
                        message.Body += "Nous vous souhaitons une agréable journée. <br/>";
                        message.Body += "Cordialement, <br/>";
                        message.Body += "L'équipe TRANSCODER France <br/>";
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return message;
        }

    }
}
