using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Tasks.Chaos
{
    public class ChaosTaskStatistic : TaskStatistic
    {
        public ChaosTaskStatistic()
        {
            StatisticType = "ChaosDungeon";
        }
        public int ChaosLevel { get; set; }
        public string Class { get; set; } = "";
        public TimeSpan Floor1Duration { get; set; }
        public TimeSpan Floor2Duration { get; set; }
        public TimeSpan Floor3Duration { get; set; }
    }
}
