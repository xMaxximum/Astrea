using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
public class AvatarModule : BaseCommandModule
{
    [Command("avatar"), Aliases(new string[] { "av" }), Description("gets a user's avatar")]
    public async Task AvatarCommand(CommandContext ctx, DiscordMember member = null)
    {
        if (member == null)
        {
            var embed = new DiscordEmbedBuilder
            {
                Description = $"**[Avatar URL]({ctx.Member.AvatarUrl})** for {ctx.Member.DisplayName} (`{ctx.Member.UsernameWithDiscriminator}`)",
                Color = ctx.Member.Color,
                ImageUrl = ctx.Member.AvatarUrl
            };

            _ = ctx.RespondAsync(embed);
        }

        else
        {
            if (ctx.Message.MentionedUsers.Any())
            {
                Console.WriteLine("looking for mentioned");
                var xd = ctx.Message.MentionedUsers[0];
                string g;
                DiscordColor color;

                Console.WriteLine("mentioned user in guild");
                var bruh = await ctx.Guild.GetMemberAsync(xd.Id);
                g = $"**[Avatar URL]({xd.AvatarUrl})** for {bruh.DisplayName} (`{xd.UsernameWithDiscriminator}`)";
                color = bruh.Color;


                var embed = new DiscordEmbedBuilder
                {
                    Description = g,
                    Color = color,
                    ImageUrl = xd.AvatarUrl
                };

                await ctx.RespondAsync(embed);

                return;

            }

            if (ulong.TryParse(ctx.RawArguments[0], out ulong lol))
            {

                Console.WriteLine("ulong feels fr");
                var bro = await ctx.Client.GetUserAsync(lol, true);

                var embed = new DiscordEmbedBuilder
                {
                    Description = $"**[Avatar URL]({bro.AvatarUrl})** for {bro.Username} (`{bro.UsernameWithDiscriminator}`)",
                    Color = DiscordColor.Blue,
                    ImageUrl = bro.AvatarUrl
                };

                await ctx.RespondAsync(embed);
            }

            else
            {

                Console.WriteLine("just tryna get member");
                DiscordMember f = (DiscordMember)member;
                var pp = await ctx.Guild.GetMemberAsync(f.Id);

                var embed = new DiscordEmbedBuilder
                {
                    Description = $"**[Avatar URL]({pp.AvatarUrl})** for {member.DisplayName} (`{member.UsernameWithDiscriminator}`)",
                    Color = member.Color,
                    ImageUrl = member.AvatarUrl
                };

                await ctx.RespondAsync(embed);
            }




        }
    }
}
