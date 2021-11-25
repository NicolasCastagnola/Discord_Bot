using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;

namespace Discord_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
            [Command("ban")]
            [RequireContext(ContextType.Guild)]
            [RequireUserPermission(GuildPermission.BanMembers)]
            public async Task BanMember(IGuildUser user, [Remainder] string reason)
            {
                if (string.IsNullOrWhiteSpace(reason)) return;

                var allBans = await Context.Guild.GetBansAsync();
                bool isBanned = allBans.Select(b => b.User).Where(u => u.Username == user.Username).Any();

                if (!isBanned)
                {
                    var targetHighest = (user as SocketGuildUser).Hierarchy;
                    var senderHighest = (Context.User as SocketGuildUser).Hierarchy;

                    if (targetHighest < senderHighest)
                    {
                        await Context.Guild.AddBanAsync(user);

                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle($"Member has been banned! :point_up_2: :weary: :point_up_2:      :point_down: :triumph: :point_down: ")
                            .WithDescription($"{user} was banned, goodbye {user}")
                            .WithColor(Color.Red)
                            .WithThumbnailUrl("https://pbs.twimg.com/media/E1pBms-XoAA3EZG?format=jpg&name=900x900")
                            .WithFooter($"{DateTime.Now}");

                        await ReplyAsync("", false, builder.Build());

                        var dmChannel = await user.GetOrCreateDMChannelAsync();
                        await dmChannel.SendMessageAsync($"You were banned from **{Context.Guild.Name}** for **{reason}**");
                    }
                }
            }


            [Command("info")]
            public async Task TargetUserInfo(SocketGuildUser user)
            {
                EmbedBuilder builder = new EmbedBuilder();

                builder.AddField("User info", $"{user.Mention}")
                       .WithColor(Color.Magenta)
                       .AddField("Account Created", $"{user.CreatedAt}")
                       .AddField("Joined at", $"{user.JoinedAt}")
                       .AddField("Roles", $"{user.Roles.Count}")
                       .WithThumbnailUrl($"{user.GetAvatarUrl(ImageFormat.Auto)}")
                       .WithFooter($"{DateTime.Now}");

                await ReplyAsync("", false, builder.Build());
            }

            [Command("flush")]
            [RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task PurgeChat(uint amount)
            {
                var messages = await Context.Channel.GetMessageAsync(amount);
                await Context.Channel.DeleteMessageAsync(messages);
                await ReplyAsync($"{amount} messages have been deleted forever!");
            }
    }
}



