using DashLib.Models;
using DashLib.Models.Settings.Monitoring;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics.CodeAnalysis;

namespace web.Services
{
    public class MailService
    {
        private readonly SettingsService _settings;
        private readonly LoggingService _logger;
        private SmtpClient _smtpClient;
        private static readonly LogEntry.LogSource _logSource = LogEntry.LogSource.MailService;

        public MailService(SettingsService settings, LoggingService logger)
        {
            _settings = settings;
            _logger = logger;
            ConnectMail();
        }

        private void ConnectMail()
        {
            _logger.LogInfo("Connecting mail client.", _logSource);
            try
            {
                if (null == _settings.Smtp)
                {
                    _logger.LogWarning("No smtp settings no client will be connected.", _logSource);
                    _smtpClient = new SmtpClient();
                    return;
                }

                string pass = string.Empty;

                if (_settings.Smtp.Password != string.Empty)
                {
                    pass = SmtpSettings.DecryptPassToString(_settings.Smtp ?? new SmtpSettings());
                }

                _smtpClient = new SmtpClient();
                _smtpClient.Connect(_settings.Smtp.ServerAddress, _settings.Smtp.Port, false);

                if (_settings.Smtp.AuthenticationIsRequired)
                {
                    _logger.LogInfo("Authentication is enabled attempting to authenticate.", _logSource);
                    _smtpClient.Authenticate(_settings.Smtp.Username, _settings.Smtp.Password);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Error connecting mail client: " + ex.Message, _logSource);
            }

        }

        private MimeMessage CreateMessage(string subject, string body)
        {
            _logger.LogInfo("Creating mime message.", _logSource);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.Smtp.FromDisplayName, _settings.Smtp.FromEmail));
            message.To.Add(new MailboxAddress(_settings.Smtp.TargetDisplayName, _settings.Smtp.TargetEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };
            return message;
        }

        public async Task SendMailAsync(string subject, string body)
        {
            _logger.LogInfo("Sending mail.", _logSource);

            try
            {
                var msg = CreateMessage(subject, body);
                await _smtpClient.SendAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending mail: " + ex.Message, _logSource);
            }
        }

        public async Task RestartAsync()
        {
            ConnectMail();
        }
    }
}
