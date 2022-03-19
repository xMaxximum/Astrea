using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.SlashCommands.Moderation
{
    public class ServerInfoCommand : ApplicationCommandsModule
    {
        [SlashCommand("serverinfo", "Gets the server's info"), BannedCheck]
        public static async Task ServerInfo(InteractionContext context)
        {
            var guild = context.Guild;
            var bans = await guild.GetBansAsync();
            var text = guild.Channels.Where(x => x.Value.Type == ChannelType.Text).ToList();
            var voice = guild.Channels.Where(x => x.Value.Type == ChannelType.Voice).ToList().Count > 0 ? $"\nVoice Channels: {guild.Channels.Where(x => x.Value.Type == ChannelType.Voice).ToList().Count}" : "";
            var category = guild.Channels.Where(x => x.Value.Type == ChannelType.Category).ToList().Count > 0 ? $" \nCategorys: {guild.Channels.Where(x => x.Value.Type == ChannelType.Category).ToList().Count}" : "";
            var stage = guild.Channels.Where(x => x.Value.Type == ChannelType.Stage).ToList().Count > 0 ? $"\nStage Channels: {guild.Channels.Where(x => x.Value.Type == ChannelType.Stage).ToList().Count}" : "";

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"Server info for {context.Guild.Name}",
                ImageUrl = guild.BannerUrl,
                Color = new DiscordColor(0x2F3136)
            }.WithThumbnail(guild.IconUrl);

            embed.AddField(new DiscordEmbedField("General Information:", $"ID: {guild.Id}\nOwner: {guild.Owner.Mention} `{guild.Owner.UsernameWithDiscriminator}`\nCreation: <t:{guild.CreationTimestamp.ToUnixTimeSeconds()}:R>\nChannels: {guild.Channels.Where(x => x.Value.IsCategory == false).ToList().Count}\nMembers: {guild.MemberCount}\nRoles: {guild.Roles.Count}\nBoosts: {guild.PremiumSubscriptionCount} (Level: {guild.PremiumTier})"));
            embed.AddField(new DiscordEmbedField("Member Information:", $"Users: {guild.Members.Where(x => x.Value.IsBot == false).ToList().Count}\nBots: {guild.Members.Where(x => x.Value.IsBot == true).ToList().Count}\nBans: {bans.Count}"));
            embed.AddField(new DiscordEmbedField("Channel Information:", $"Text Channels: {text.Count} {voice} {category} {stage}"));
            embed.AddField(new DiscordEmbedField("In-depth Information:", $"Emotes: {guild.Emojis.Count}\nStickers: {guild.Stickers.Count}\nMFA: {guild.MfaLevel}\nNSFW Level: {guild.NsfwLevel}\nVerification Level: {guild.VerificationLevel}\nFeatures: {string.Join("", guild.RawFeatures.Select(x => $"`{x.ToLower()}` ").ToList())}"));


            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }
    }
}
