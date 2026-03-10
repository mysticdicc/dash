using DashLib.DankAPI;
using DashLib.Models;
using DashLib.Models.Settings;
using Telegram.Bot;

namespace web.Services
{
    public class TelegramService
    {
        private TelegramBotClient? _botClient;
        private readonly SettingsService _settings;
        private readonly LoggingService _logger;
        private readonly LogEntry.LogSource _logSource = LogEntry.LogSource.TelegramService;
        public TelegramService(SettingsService settingsService, LoggingService logger)
        {
            _settings = settingsService;
            _logger = logger;

            try
            {
                _botClient = new TelegramBotClient(_settings.Telegram.BotToken);
            }
            catch 
            {
                _botClient = null;
            }

            _logger.LogInfo("Service has been started.", _logSource);
        }

        public async Task SendMessageAsync(string message)
        {
            if (null == _botClient) return;
            try
            {
                _logger.LogInfo("Sending telegram message.", _logSource);
                await _botClient.SendMessage(_settings.Telegram.ChatId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending telegram message: " + ex.Message, _logSource);
            }
        }

        public async Task Restart()
        {
            _logger.LogInfo("Service restart intiiated.", _logSource);
            _botClient?.Close();
            _botClient = new TelegramBotClient(_settings.Telegram.BotToken);
        }
    }
}
