using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace RealEstateApp.Services;

public interface ITelegramNotifier
{
    Task SendAsync(string message);
}

public class TelegramNotifier : ITelegramNotifier
{
    private readonly TelegramSettings _settings;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<TelegramNotifier> _logger;

    public TelegramNotifier(
        IOptions<TelegramSettings> settings,
        IHttpClientFactory httpFactory,
        ILogger<TelegramNotifier> logger)
    {
        _settings = settings.Value;
        _httpFactory = httpFactory;
        _logger = logger;
    }

    public async Task SendAsync(string message)
    {
        // ===== DEBUG: тохиргоо зөв уншигдсан эсэхийг шалгах =====
        _logger.LogWarning(
            "TELEGRAM DEBUG: Enabled={Enabled}, TokenLen={TokenLen}, ChatId={ChatId}",
            _settings.Enabled,
            _settings.BotToken?.Length ?? 0,
            string.IsNullOrWhiteSpace(_settings.ChatId) ? "(хоосон)" : _settings.ChatId);

        if (!_settings.Enabled)
        {
            _logger.LogWarning("TELEGRAM: Enabled=false тул алгаслаа.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.BotToken) || string.IsNullOrWhiteSpace(_settings.ChatId))
        {
            _logger.LogWarning("TELEGRAM: BotToken эсвэл ChatId хоосон тул алгаслаа.");
            return;
        }

        try
        {
            var client = _httpFactory.CreateClient("telegram");
            client.Timeout = TimeSpan.FromSeconds(10);

            var url = $"https://api.telegram.org/bot{_settings.BotToken}/sendMessage";

            var payload = new
            {
                chat_id = _settings.ChatId,
                text = message,
                parse_mode = "HTML",
                disable_web_page_preview = true
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await client.PostAsync(url, content);
            var body = await resp.Content.ReadAsStringAsync();

            if (resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("TELEGRAM: Амжилттай илгээгдлээ. Хариу: {Body}", body);
            }
            else
            {
                _logger.LogWarning("TELEGRAM: Амжилтгүй {Status}. Хариу: {Body}", resp.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            // Мэдэгдэл унасан ч үндсэн үйлдэл (хүсэлт хадгалах) тасрахгүй
            _logger.LogWarning(ex, "TELEGRAM: Илгээхэд алдаа гарлаа (интернэт/timeout байж магадгүй).");
        }
    }
}