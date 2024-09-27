namespace Chirp.Razor.Pages;

using System.Data;
using Microsoft.Data.Sqlite;

public class DBFacade
{
    public static List<CheepViewModel> ReadDB()
    {
        var cheeps = new List<CheepViewModel>();
        var sqlDBFilePath = "/tmp/chirp.db";
        var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                
            }
        }

        return cheeps;
    }
}