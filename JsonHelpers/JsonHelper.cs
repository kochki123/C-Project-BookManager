using C_Project.Enums;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace C_Project.JsonHelpers
{
    internal static class JsonHelper<T>
    {
        private static readonly string[] _files = new string[]
        {
            @"C:\Book_Files\Books.json",
            @"C:\Book_Files\Genre.json",
            @"C:\Book_Files\Rating.json",
            @"C:\Book_Files\Status.json",
            @"C:\Book_Files\Users.json"
        };

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static async Task SerializeAsync(FILE_TYPE filetype, T entity)
        {
            int fileIndex = Convert.ToInt32(filetype);
            await using var stream = File.Create(_files[fileIndex]);
            await JsonSerializer.SerializeAsync(stream, entity, _options);
        }

        public static async Task SerializeAsync(FILE_TYPE filetype, List<T> values)
        {
            int fileIndex = Convert.ToInt32(filetype);
            await using var stream = File.Create(_files[fileIndex]);
            await JsonSerializer.SerializeAsync(stream, values, _options);
        }

        public static async Task<List<T>> ReadFromJsonAsync(FILE_TYPE filetype)
        {
            int fileIndex = Convert.ToInt32(filetype);
            string filePath = _files[fileIndex];

            try
            {
                await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, _options);
                return result ?? new List<T>();
            }
            catch (FileNotFoundException)
            {
                return new List<T>();
            }
            catch (DirectoryNotFoundException)
            {
                return new List<T>();
            }
        }
    }
}