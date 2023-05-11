using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using FitnessCenterConsole.Common;
using System.ComponentModel;

namespace FitnessCenterConsole.DAL {
    public static class JsonFileReader {
        public static Database Read<Database>(string filePath) {
            string text = File.ReadAllText(filePath);
            TypeDescriptor.AddAttributes(typeof((string, string)), new TypeConverterAttribute(typeof(TupleConverter<string, string>)));
            return JsonConvert.DeserializeObject<Database>(text);
            //return JsonSerializer.Deserialize<Database>(text);
        }
    }
}
