using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using DashLib.Settings;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;

namespace web.Controllers
{
    [ApiController]
    public class MailController
    {
        [HttpGet]
        [Route("[controller]/v1/send/test")]
        public async Task<Results<BadRequest<string>, Ok<string>>> SendTestEmail()
        {
            var settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

            if (settings.MonitoringSettings.SmtpPassword != string.Empty)
            {
                settings.MonitoringSettings = MonitoringSettings.DecryptPassword(settings.MonitoringSettings);
            }

            if (null == settings)
                return TypedResults.BadRequest("couldnt fetch settings file");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("dash Test Service", "dash@test.com"));
            message.To.Add(new MailboxAddress("username", settings.MonitoringSettings.SmtpTargetEmail));
            message.Subject = "Test Email";
            message.Body = new TextPart("plain") { Text = "Test body text" };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(settings.MonitoringSettings.SmtpServerAddress, settings.MonitoringSettings.SmtpPort, false);
            }
            catch(Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
            
            if (settings.MonitoringSettings.SmtpAuthenticationIsRequired)
            {
                try
                {
                    await client.AuthenticateAsync(settings.MonitoringSettings.SmtpUsername, settings.MonitoringSettings.SmtpPassword);
                }
                catch(Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            
            try
            {
                await client.SendAsync(message);
                client.Disconnect(true);
                return TypedResults.Ok("success");
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
    }
}
