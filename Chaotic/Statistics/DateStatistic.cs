﻿using Chaotic.Tasks;
using Chaotic.Tasks.Chaos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Statistics
{
    public class DateStatistic
    {
        public DateTime ExecutionDate { get; set; }
        public List<KurzanTaskStatistic> KurzanStatistics { get; set; } = new List<KurzanTaskStatistic>();
        public List<ChaosTaskStatistic> ChaosStatistics { get; set; } = new List<ChaosTaskStatistic>();

        public List<KurzanStatDisplay> KurzanStats { get; set; } = new List<KurzanStatDisplay>();
        public List<ChaosStatDisplay> ChaosStats { get; set; } = new List<ChaosStatDisplay>();

        public void Compute()
        {
            KurzanStats = KurzanStatistics.Where(x=> x.StartDate.Date == ExecutionDate.Date).GroupBy(x => new { x.ChaosLevel, x.Map }).Select(x =>
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
}