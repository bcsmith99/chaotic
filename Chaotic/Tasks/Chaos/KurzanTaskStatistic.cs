using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Tasks.Chaos
{
    public class KurzanTaskStatistic : TaskStatistic
    {
        public KurzanTaskStatistic()
        {
            StatisticType = "Kurzan"; 
        }
        public int ChaosLevel { get; set; }
        public string Class { get; set; } = "";
        public string Map { get; set; } = "";
    }
}
