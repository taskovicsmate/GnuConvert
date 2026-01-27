using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException("Invalid JSON");
        }

        public static void SaveAtomic<T>(string path, T data)
        {
            var dir = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);

          
        }
    }

}
