﻿using Chaotic.Tasks;
using Chaotic.Tasks.Chaos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Statistics
{
    public class ClassStatistic
    {
        public string ClassName { get; set; } = "";
        private double _successRate = 0;
        public double SuccessRate
        {
            get { return _successRate; }
            set
            {
                if (!double.IsNaN(value))
                {
                    _successRate = value;
                }
            }
        }

        public List<KurzanTaskStatistic> KurzanStatistics { get; set; } = new List<KurzanTaskStatistic>();
        public List<ChaosTaskStatistic> ChaosStatistics { get; set; } = new List<ChaosTaskStatistic>();

        public List<KurzanStatDisplay> KurzanStats { get; set; } = new List<KurzanStatDisplay>();
        public List<ChaosStatDisplay> ChaosStats { get; set; } = new List<ChaosStatDisplay>();

        internal void Compute()
        {
            KurzanStats = KurzanStatistics.GroupBy(x => new { x.ChaosLevel, x.Map }).Select(x =>
            {
                return new KurzanStatDisplay()
                {
                    Level = x.Key.ChaosLevel,
                    Map = x.Key.Map,
                    TotalRuns = x.Count(),
                    TotalComplete = x.Count(y => y.TaskOutcome == TaskOutcomes.Success),
                    TotalFailed = x.Count(y => y.TaskOutcome == TaskOutcomes.Failure),
                    TotalTimeout = x.Count(y => y.TaskOutcome == TaskOutcomes.Timeout),
                    AverageTime = new TimeSpan(0, 0, 0, (int)x.Average(y => y.TotalDuration.TotalSeconds))
                };
            }).OrderBy(x => x.Level).ThenBy(x => x.Map).ToList();

            ChaosStats = ChaosStatistics.GroupBy(x => x.ChaosLevel).Select(x =>
            {
                return new ChaosStatDisplay()
                {
                    Level = x.Key,
                    TotalRuns = x.Count(),
                    TotalComplete = x.Count(y => y.TaskOutcome == TaskOutcomes.Success),
                    TotalFailed = x.Count(y => y.TaskOutcome == TaskOutcomes.Failure),
                    TotalTimeout = x.Count(y => y.TaskOutcome == TaskOutcomes.Timeout),
                    AverageTime = new TimeSpan(0, 0, 0, (int)x.Average(y => y.TotalDuration.TotalSeconds)),
                    AverageFloor1 = new TimeSpan(0, 0, 0, (int)x.Where(y => y.Floor1Duration.TotalMilliseconds > 0).Average(y => y.Floor1Duration.TotalSeconds)),
                    AverageFloor2 = new TimeSpan(0, 0, 0, (int)x.Where(y => y.Floor2Duration.TotalMilliseconds > 0).Average(y => y.Floor2Duration.TotalSeconds)),
                    AverageFloor3 = new TimeSpan(0, 0, 0, (int)x.Where(y => y.Floor3Duration.TotalMilliseconds > 0).Average(y => y.Floor3Duration.TotalSeconds)),
                };

            }).ToList();
        }
    }

    public class ChaosStatDisplay
    {
        public int Level { get; set; }
        public int TotalRuns { get; set; }
        public int TotalComplete { get; set; }
        public int TotalFailed { get; set; }
        public int TotalTimeout { get; set; }

        public TimeSpan AverageTime { get; set; }
        public TimeSpan AverageFloor1 { get; set; }
        public TimeSpan AverageFloor2 { get; set; }
        public TimeSpan AverageFloor3 { get; set; }
    }

    public class KurzanStatDisplay
    {
        public int Level { get; set; }
        public string Map { get; set; }
        public int TotalRuns { get; set; }
        public int TotalComplete { get; set; }
        public int TotalFailed { get; set; }
        public int TotalTimeout { get; set; }

        public TimeSpan AverageTime { get; set; }
    }
}
