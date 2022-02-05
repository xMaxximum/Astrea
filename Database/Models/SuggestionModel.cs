using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotName.Database.Models
{
    public class SuggestionModel
    {
        public int ID { get; set; }
        public ulong GuildId { get; set; }
        // public int GuildSuggestionId { get; set; }
        public ulong UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int State { get; set; } = 0;
    }
}
