using DashLib.DankAPI;
using DashLib.Models.Settings;
using Telegram.Bot;

namespace web.Services
{
    public class TelegramService
    {
        private TelegramBotClient? _botClient;
        private readonly SettingsService _settings;

        public TelegramService(SettingsService settingsService)
        {
            _settings = settingsService;

            try
            {
                _botClient = new TelegramBotClient(_settings.Telegram.BotToken);
            }
            catch 
            {
                _botClient = null;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (null == _botClient) return;
            await _botClient.SendMessage(_settings.Telegram.ChatId, message);
        }

        public async Task Restart()
        {
            _botClient?.Close();
            _botClient = new TelegramBotClient(_settings.Telegram.BotToken);
        }
    }
}
