using System.Text.Json;
using System.IO;

namespace FitnessCenterConsole.ConsolePL {
    public static class JsonFileReader {
        public static T Read<T>(string filePath) {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }
}
