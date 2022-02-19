using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Logging
{
    public class MessageUpdateEvent
    {
        public static async Task MessageUpdated(DiscordClient client, MessageUpdateEventArgs args)
        {
            ulong channelId = await Database.Database.Logging.GetLogChannel(args.Guild.Id, LogType.MessageUpdate);
            if (channelId != 0)
            {
                await args.Guild.GetChannel(channelId).SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Title = $"Message updated in #{args.Channel.Name}",
                    Description = Util.AddLint($"Author: {args.Message.Author.UsernameWithDiscriminator}\nAuthor ID: {args.Message.Author.Id}\n" +
                    $"Old Content: {args.MessageBefore.Content}\nNew Content: {args.Message.Content}\nSent Time: {args.Message.CreationTimestamp.DateTime}" +
                    $"\nUpdated Time: {args.Message.EditedTimestamp}")
                }.WithFooter($"Sent by {args.Message.Author.UsernameWithDiscriminator}", args.Message.Author.AvatarUrl)
                .WithThumbnail(args.Message.Author.AvatarUrl));

            }
            else return;
        }
    }
}
