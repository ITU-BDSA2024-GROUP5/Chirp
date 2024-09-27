namespace Chirp.Razor.Pages;

using System.Data;
using Microsoft.Data.Sqlite;

public class DBFacade
{
    static string sqlDBFilePath = "/tmp/chirp.db";
    
    public static List<CheepViewModel> ReadDB()
    {
        var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";
        
        return ConnectAndExecute(sqlQuery);
    }

    public static List<CheepViewModel> ReadDBByAuthor(string author)
    {
        var sqlQuery = @"SELECT * FROM message WHERE author = {author}";

        return ConnectAndExecute(sqlQuery);
    }

    private static List<CheepViewModel> ConnectAndExecute(string query)
    {
        var cheeps = new List<CheepViewModel>();
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var message_id = reader.GetString(0);
                var author_id = reader.GetString(1);
                var message = reader.GetString(2);
                var date = reader.GetString(3);
                
                cheeps.Add(new CheepViewModel(author_id, message, date));
            }
        }
        return cheeps;
    }
}