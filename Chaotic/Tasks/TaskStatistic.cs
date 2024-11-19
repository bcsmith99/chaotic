using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chaotic.Tasks
{

    public static class TaskOutcomes
    {
        public static readonly string Success = "Success";
        public static readonly string Failure = "Failure";
        public static readonly string Timeout = "Timeout";
    }

    public interface ITaskStatistic
    {
        public string StatisticType { get; set; }
    }

    public class TaskStatistic : ITaskStatistic
    {
        public string StatisticType { get; set; } = "";
        public Guid CharacterIdentifier { get; set; }

        public string TaskOutcome { get; set; } = "";
        public TimeSpan TotalDuration { get; set; }
        public DateTime StartDate { get; set; }
    }
}
