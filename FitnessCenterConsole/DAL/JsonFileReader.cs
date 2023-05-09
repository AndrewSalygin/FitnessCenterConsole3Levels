using System.Text.Json;
using System.IO;

namespace FitnessCenterConsole.DAL {
    public static class JsonFileReader {
        public static Database Read<Database>(string filePath) {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Database>(text);
        }
    }
}
