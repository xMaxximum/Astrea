using BotName.Database.Models;
using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.Database
{
    public class Database
    {
        #region Blacklist
        public static async Task<bool> BlacklistContains(ulong userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
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
            #region oldCode
            /*
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
               
                var sql = "SELECT Id FROM blacklist WHERE blacklist.Id = @userId";

                cnn.Open();


                using var cmd = new SQLiteCommand(sql, (SQLiteConnection)cnn);
                cmd.Parameters.AddWithValue("@userId", userId);
                await cmd.PrepareAsync();
                using var rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    cnn.Close();
                    return true;
                }

                else
                {
                    cnn.Close();
                    return false;
                }
                */
            #endregion oldCode
        }

        public static async Task AddBlacklistAsync(BlacklistModel user)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                await cnn.QueryAsync("insert into blacklist (Id, Reason) values (@Id, @Reason)", user);
            }
        }
        #endregion Blacklist

        #region Suggestions
        public static async Task AddSuggestionAsync(SuggestionModel suggestion)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    await cnn.QueryAsync<SuggestionModel>("insert into suggestions (guildId, guildSuggestionId, channelId, messageId, userId, title, description, state) values (@guildId, @guildSuggestionId, @channelId, @messageId, @userId, @title, @description, @state)", suggestion);

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task EditSuggestionAsync(SuggestionModel suggestion, int id, ulong userId, ulong guildId)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    var test = await cnn.QueryAsync<SuggestionModel>($"update suggestions set description = @Description where guildId = {guildId}, userId = {userId}, guildSuggestionId = {id}", suggestion);
                    Console.WriteLine(test.ToString());
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task<int> GetGuildSuggestionsAsync(ulong guildId)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    int guildSuggestionCount = await cnn.QueryFirstOrDefaultAsync<int>($"select count(guildSuggestionId) from suggestions where guildId = {guildId}");

                    return guildSuggestionCount + 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return 0;
            }
        }
        #endregion Suggestions


        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
