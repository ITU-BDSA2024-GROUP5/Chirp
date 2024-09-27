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
        var sqlQuery = @$"SELECT * FROM message m JOIN user u ON m.author_id = u.user_id WHERE u.username = {author}";

        return ConnectAndExecute(sqlQuery);
    }
    
    private static string GetAuthorFromID(int id)
    {
        string author = "";
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            var query = @$"SELECT username FROM User WHERE user_id = {id}";
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                author = reader.GetString(0);
                break;
            }
        }

        return author;
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
                var author_id = reader.GetInt32(1);
                var message = reader.GetString(2);
                var date = reader.GetInt32(3);
                
                cheeps.Add(new CheepViewModel(GetAuthorFromID(author_id), message, UnixTimeStampToDateTimeString(date)));
            }
        }
        return cheeps;
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("dd'/'MM'/'yyyy HH:mm:ss");
    }
}