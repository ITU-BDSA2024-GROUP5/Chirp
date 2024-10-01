namespace Chirp.Razor.Pages;

using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;


public class DBFacade
{
    static string sqlDBFilePath = "/tmp/chirp.db";
    private static readonly SqliteConnection connection;

    static DBFacade()
    {
        DbExists(sqlDBFilePath);
        connection = new SqliteConnection($"Data Source={sqlDBFilePath}");
        connection.Open();
        createFile();
    }

    public static void createFile() // Maybe only create file if there is no data in there anyways
    {
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

        using var readerschema = embeddedProvider.GetFileInfo("/data/schema.sql").CreateReadStream();
        using var srschema = new StreamReader(readerschema);

        var query = srschema.ReadToEnd();
        InitDBExecute(query);

        using var readerdump = embeddedProvider.GetFileInfo("/data/dump.sql").CreateReadStream();
        using var srdump = new StreamReader(readerdump);
        var querydb = srdump.ReadToEnd();
        InitDBExecute(querydb);
    }
    
    public static void DbExists(String path)
    {
        if (!File.Exists(path))
        {
            sqlDBFilePath = Path.Combine(Path.GetTempPath(), "chirp.db");
        }
    }

    public static List<CheepViewModel> ReadDB()
    {
        var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";
        
        return ConnectAndExecute(sqlQuery);
    }

    public static List<CheepViewModel> ReadDBByAuthor(string author)
    {
        var sqlQuery = @"SELECT m.message_id, m.author_id, m.text, m.pub_date FROM message m JOIN user u ON m.author_id = u.user_id WHERE u.username = @Author ORDER by m.pub_date desc";

        return ConnectAndExecute(sqlQuery, author);
    }
    
    private static string GetAuthorFromID(int id)
    {
        string author = "";
            var query = @$"SELECT DISTINCT username FROM User WHERE user_id = {id}";
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                author = reader.GetString(0);
                break;
            }
            //connection.Close();
            return author;
    }

    private static void InitDBExecute(String query)
    {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;
            using var reader = command.ExecuteReader();
        
            //connection.Close();
    }
    private static List<CheepViewModel> ConnectAndExecute(string query)
    {
        var cheeps = new List<CheepViewModel>();
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
            //connection.Close();
        
        return cheeps;
    }
    
    private static List<CheepViewModel> ConnectAndExecute(string query, string author)
    {
        var cheeps = new List<CheepViewModel>();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.Add("@Author", SqliteType.Text);
            command.Parameters["@Author"].Value = author;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var message_id = reader.GetString(0);
                var author_id = reader.GetInt32(1);
                var message = reader.GetString(2);
                var date = reader.GetInt32(3);
                
                cheeps.Add(new CheepViewModel(GetAuthorFromID(author_id), message, UnixTimeStampToDateTimeString(date)));
            }
            //connection.Close();
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