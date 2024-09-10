using CsvHelper;
using System.Globalization;

namespace SimpleDB
{
    public sealed class CSVDatabase<T> : IDatabaseRepository<T>{
        static String path = "chirp_cli_db.csv";
    
        public IEnumerable<T> Read(int? limit = null)
        {
            List<T> cheeps = new List<T>();
            try {
                // https://joshclose.github.io/CsvHelper/getting-started/#reading-a-csv-file
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<T>();
                    
                    if(limit != null)
                    {
                        records = records.Take(limit.Value);
                    }

                    foreach (var record in records)
                    {
                        cheeps.Add(record);
                    }
                }
            } catch (IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return cheeps;
        }
        
        public void Store(T record)
        {
            // https://joshclose.github.io/CsvHelper/getting-started/#writing-a-csv-file
            using (var stream = File.Open(path, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            {
                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = stream.Length == 0
                };
                
                var records = new List<T>();
                records.Add(record);
                
                using (var csv = new CsvWriter(writer, config))              
                {                                                            
                    csv.WriteRecords(records);                               
                }               
                Console.WriteLine("Successfully cheeped");
            }
        }
    }
}
