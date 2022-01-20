using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotName.Database.Models
{
    public class BlacklistModel
    {
        public ulong Id { get; set; }
        public string Reason { get; set; } = null;
    }
}
