using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace LogViewer
{
    public class Config
    {
        private const string ConfigPath = "config.json";

        public string Filename { get; set; }

        public static Config Get()
        {
            var filepath = System.IO.Path.Combine(Application.StartupPath, ConfigPath);

            if (File.Exists(filepath))
            {
                var text = File.ReadAllText(filepath);

                if (!string.IsNullOrEmpty(text))
                {
                    return JsonConvert.DeserializeObject<Config>(text);
                }
            }

            return new Config();
        }

        public static bool Save(Config config)
        {
            var filepath = System.IO.Path.Combine(Application.StartupPath, ConfigPath);

            if (config != null)
            {
                File.WriteAllText(filepath, JsonConvert.SerializeObject(config));

                return true;
            }

            return false;
        }
    }
}
