namespace RealEstateApp.Services;

public class TelegramSettings
{
    public string BotToken { get; set; } = "";
    public string ChatId { get; set; } = "";
    public bool Enabled { get; set; }
}