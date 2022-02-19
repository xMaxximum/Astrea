using BotName.Database.Models;
using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using static BotName.Database.Models.LoggingModel;

namespace BotName.Database
{
    public class Database
    {
        #region Blacklist

        public class Blacklists
        {
            public static async Task<bool> BlacklistContains(ulong userId)
            {
                using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                var output = await cnn.QueryAsync<BlacklistModel>($"select id from blacklist where id = {userId}");
                if (output.ToList().Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static async Task AddBlacklistAsync(BlacklistModel user)
            {
                using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                await cnn.QueryAsync("insert into blacklist (Id, Reason) values (@Id, @Reason)", user);
            }
        }
        #endregion Blacklist


        #region Suggestions
        public class Suggestions
        {
            public static async Task AddSuggestionAsync(SuggestionModel suggestion)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    await cnn.QueryAsync<SuggestionModel>("insert into suggestions (guildId, guildSuggestionId, channelId, messageId, userId, description, state) values (@guildId, @guildSuggestionId, @channelId, @messageId, @userId, @description, @state)", suggestion);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public static async Task EditSuggestionAsync(string description, int id, ulong userId, ulong guildId)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    await cnn.QueryAsync<SuggestionModel>($"update suggestions set description = @Description where guildId = {guildId} and userId = {userId} and guildSuggestionId = {id}", new
                    {
                        Description = description
                    });
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public static async Task EditSuggestionState(ulong guildId, int suggestionId, int state)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    await cnn.QueryAsync($"update suggestions set state = @state where guildId = {guildId} and guildSuggestionId = {suggestionId}", new { state });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public static async Task<bool> IsSuggestionOwner(int guildSuggestionId, ulong userId)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    var suggestion = await cnn.QueryFirstOrDefaultAsync<string>("select 1 from suggestions where guildSuggestionId = @guildsuggestionid and userId = @userid", new
                    {
                        guildsuggestionid = guildSuggestionId,
                        userid = userId
                    });
                    if (suggestion == "1")
                    {
                        return true;
                    }
                    else
                        return false;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    return false;
                }
            }

            public static async Task<int> GetGuildSuggestionsAsync(ulong guildId)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    int guildSuggestionCount = await cnn.QueryFirstOrDefaultAsync<int>($"select count(guildSuggestionId) from suggestions where guildId = {guildId}");

                    return guildSuggestionCount + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return 0;
                }
            }

            public static async Task<SuggestionModel> GetSuggestionMsgOfGuildAsync(ulong guildId, int guildSuggestionId)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    var message = await cnn.QueryFirstAsync<SuggestionModel>("select messageId, channelId, state, description from suggestions where guildId = @guildid and guildSuggestionId = @guildsuggestionid",
                        new { guildid = guildId, guildsuggestionid = guildSuggestionId });

                    return message;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return new SuggestionModel();
                }
            }
        }
        #endregion Suggestions

        #region Logging
        public class Logging
        {
            public static async Task AddLogServer(ulong guildId)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    await cnn.QueryFirstOrDefaultAsync<ulong>($"insert into logging (GuildId) values ({guildId})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return;
                }
            }
            public static async Task<ulong> GetLogChannel(ulong guildId, LogType type)
            {
                try
                {
                    using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
                    var channelId = await cnn.QueryFirstOrDefaultAsync<ulong>($"select {type} from logging where GuildId = @guildid", new { guildid = guildId });
                    return channelId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return 0;
                }
            }
        }
        #endregion Logging

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
