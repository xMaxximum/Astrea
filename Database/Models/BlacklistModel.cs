namespace BotName.Database.Models
{
    public class BlacklistModel
    {
        public ulong Id { get; set; }
        public string Reason { get; set; } = null;
    }
}
