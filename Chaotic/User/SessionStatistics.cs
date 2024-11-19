using Chaotic.Tasks;
using Chaotic.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.User
{
    public class SessionStatistics
    {
        public SessionStatistics()
        {
            Statistics = new List<ITaskStatistic>();
        }

        public List<ITaskStatistic> Statistics { get; set; }

        public void Save(string fileName, bool includeFirstComma = true)
        {
            if (!File.Exists(fileName))
            {
                var sb = new StringBuilder();
                sb.AppendLine("[");
                sb.Append("]");
                File.AppendAllText(fileName, sb.ToString());
                Save(fileName, false);
            }
            else
            {
                if (this.Statistics.Count == 0)
                    return;

                var sb = new StringBuilder();
                for (var i = 0; i < this.Statistics.Count; i++)
                {
                    var statistic = Statistics[i]; 
                    if (i > 0 || includeFirstComma)
                        sb.AppendLine(",");

                    using (var sw = new StringWriter(sb))
                    {
                        using (JsonTextWriter jw = new(sw))
                        {
                            JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                            serializer.Serialize(jw, statistic);
                        }
                    }
                }
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    fs.SetLength(fs.Length - 1);
                }

                sb.Append("]");

                File.AppendAllText(fileName, sb.ToString());
            }

            this.Statistics.Clear();
        }

        public SessionStatistics Read(string fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    var json = sr.ReadToEnd();

                    //var serializer = new JsonSerializer();

                    return new SessionStatistics()
                    {
                        Statistics = JsonConvert.DeserializeObject<List<ITaskStatistic>>(json, new TaskStatisticItemConverter())
                    };
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
