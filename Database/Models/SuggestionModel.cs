namespace BotName.Database.Models
{
    public class SuggestionModel
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public int GuildSuggestionId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public string Description { get; set; }
        public int State { get; set; } = 0;
    }
}
