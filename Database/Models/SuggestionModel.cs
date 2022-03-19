using System.Numerics;

namespace BotName.Database.Models
{
    public class SuggestionModel
    {
        public int Id { get; set; }
        public BigInteger GuildId { get; set; }
        public int GuildSuggestionId { get; set; }
        public BigInteger ChannelId { get; set; }
        public BigInteger MessageId { get; set; }
        public BigInteger UserId { get; set; }
        public string Description { get; set; }
        public int State { get; set; } = 0;
        public string Reason { get; set; } = "";

        public override string ToString()
        {
            return $"Id: {Id}, GuildId: {GuildId}, GuildSugId: {GuildSuggestionId}, ChannelId: {ChannelId}, MessageId: {MessageId}, {UserId}, {Description}, {State}"; 
        }

        public static string SuggestionState(int state, string reason = null)
        {
            return state switch
            {
                0 => "\n\n❌This suggestion hasn't been responded to yet!",
                1 => $"\n\n✅This suggestion has been approved{(reason is not null ? $" for `{reason}`" : "")}!",
                2 => $"\n\n❌This suggestion has been declined{(reason is not null ? $" for `{reason}`" : "")}!",
                _ => "\n\n⏲️This suggestion hasn't been responded to yet!",
            };
        }
    }

}
