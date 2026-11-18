using C_Project.Enums;

namespace C_Project.Helpers
{
    internal static class FileHelper<T>
    {
        private static readonly string[] _files = new string[]
        {
            @"C:\Book_Files\Books.txt",
            @"C:\Book_Files\Genre.txt",
            @"C:\Book_Files\Rating.txt",
            @"C:\Book_Files\Status.txt",
            @"C:\Book_Files\Users.txt"
        };

        public static async Task WriteInFileAsync(FILE_TYPE fileType, T entity, bool append = true)
        {
            int fileIndex = Convert.ToInt32(fileType);
            FileMode mode = append ? FileMode.Append : FileMode.Create;

            await using var stream = new FileStream(_files[fileIndex], mode, FileAccess.Write, FileShare.None, 4096, true);
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(entity?.ToString());
        }

        public static async Task WriteInFileAsync(FILE_TYPE fileType, List<T> values, bool append = false)
        {
            int fileIndex = Convert.ToInt32(fileType);
            FileMode mode = append ? FileMode.Append : FileMode.Create;

            await using var stream = new FileStream(_files[fileIndex], mode, FileAccess.Write, FileShare.None, 4096, true);
            await using var writer = new StreamWriter(stream);

            foreach (T entity in values)
            {
                await writer.WriteLineAsync(entity?.ToString());
            }
        }

        public static async Task ReadFromFileAsync(FILE_TYPE fileType)
        {
            int fileIndex = Convert.ToInt32(fileType);
            string filePath = _files[fileIndex];

            try
            {
                await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            catch (FileNotFoundException)
            {
                // Silently handles missing files without requiring an if-else check
            }
            catch (DirectoryNotFoundException)
            {
                // Silently handles missing directories without requiring an if-else check
            }
        }
    }
}