using Chaotic.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace Chaotic.User
{
    public class UserSettings
    {
        public UserSettings()
        {
            Characters = new ObservableCollection<UserCharacter>();
        }

        public LogDetailLevel LogDetailLevel { get; set; }

        public string Resolution { get; set; } = "3440x1440";
        public bool QuitAfterFullRotation { get; set; }
        public int UsePotionPercent { get; set; } = 40;
        public string HealthPotionKey { get; set; } = "F1";
        public bool EnableAura { get; set; }
        public bool RepairGear { get; set; }
        public bool MoveHoningMaterials { get; set; }
        public bool MoveGems { get; set; }
        public int GemLevelThreshold { get; set; }
        public double PerformanceMultiplier { get; set; } = 1;

        public bool CaptureTimeoutScreenshot { get; set; } = true;

        public bool GoOffline { get; set; } = true;

        public DateTime? LastWeeklyReset { get; set; }
        public ObservableCollection<UserCharacter> Characters { get; set; }

        public void Save(string fileName)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (JsonTextWriter jw = new(sw))
                {
                    JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                    serializer.Serialize(jw, this);
                }
            }

            File.WriteAllText(fileName, sb.ToString());
        }

        public UserSettings Read(string fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    using (var tr = new JsonTextReader(sr))
                    {
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<UserSettings>(tr);
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                this.Save(fileName);
                return this.Read(fileName);
            }

        }
    }
}
