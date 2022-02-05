using BotName.Database.Models;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace BotName.Database
{
    public class Database
    {

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

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
