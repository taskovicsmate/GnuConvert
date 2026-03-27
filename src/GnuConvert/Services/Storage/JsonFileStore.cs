using GnuConvert.ExceptionHandling;
using System.IO;
using System.Text.Json;

namespace GnuConvert.Services.Storage
{
    public static class JsonFileStore
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public static T Load<T>(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new PersistenceException("FILE_NOT_FOUND",$"A fájl nem található: {path}");

                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(json)
                       ?? throw new PersistenceException("INVALID_JSON",$"Deserialization returned null for file: {path}");

            }
            catch (JsonException ex)
            {
                throw new PersistenceException(
                    "INVALID_JSON",
                    $"A JSON fájl sérült: {path}",
                    ex);
            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "FILE_READ_ERROR",
                    $"Nem sikerült a fájlt beolvasni: {path}",
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenceException(
                    "FILE_ACCESS_DENIED",
                    $"Nem sikerült a fájlt megnyitni: {path}",
                    ex);
            }
        }

        public static void SaveAtomic<T>(string path, T data)
        {
            try
            {
                var dir = Path.GetDirectoryName(path)!;
                Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(path, json);

            }
            catch (JsonException ex)
            {
                throw new PersistenceException(
                    "JSON_SERIALIZATION_ERROR",
                    $"Failed to serialize data for file: {path}",
                    ex);
            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "FILE_WRITE_ERROR",
                    $"Failed to write file: {path}",
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenceException(
                    "FILE_ACCESS_DENIED",
                    $"Access denied to file: {path}",
                    ex);
            }


        }
    }

}
