using System.Numerics;

namespace BotName.Database.Models
{
    public class BlacklistModel
    {
        public BigInteger Id { get; set; }
        public string Reason { get; set; } = null;
    }
}
