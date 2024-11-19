using Chaotic.Tasks;
using Chaotic.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Chaotic.Utilities
{
    public enum LogDetailLevel
    {
        None = 0,
        Debug = 1,
        Info = 2,
        Summary = 3
    }
    public class LogEntry
    {
        public DateTime LogDate { get; set; }
        public LogDetailLevel Level { get; set; }
        public string Entry { get; set; } = "";

        public LogEntry()
        {

        }
    }
    public class SessionLog
    {
        public List<LogEntry> AllLogEntries { get; set; }
        public ObservableCollection<LogEntry> LogEntries { get; set; }
        public LogDetailLevel CurrentLoggingLevel { get; set; }
        public SessionLog(LogDetailLevel logLevel)
        {
            AllLogEntries = new List<LogEntry>();
            LogEntries = new ObservableCollection<LogEntry>();
            CurrentLoggingLevel = logLevel;
        }

        public void RefreshVisibleEntries()
        {
            LogEntries.Clear();
            if (CurrentLoggingLevel != LogDetailLevel.None)
            {
                foreach (var entry in AllLogEntries.Where(x => x.Level >= CurrentLoggingLevel))
                    LogEntries.Add(entry);
            }
        }
    }

    public class AppLogger
    {
        private readonly SessionLog _log;
        private readonly SessionStatistics _statistics;
        private string USER_STATISTICS_PATH = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().Location).LocalPath) + "\\userStatistics.json";

        public AppLogger(SessionLog log, SessionStatistics statistics)
        {
            _log = log;
            _statistics = statistics;
        }

        public void Log(LogDetailLevel level, string message)
        {
            if (level == LogDetailLevel.Debug)
                Debug.WriteLine(message);

            var newEntry = new LogEntry { LogDate = DateTime.Now, Level = level, Entry = message };
            _log.AllLogEntries.Add(newEntry);
            if (_log.CurrentLoggingLevel != LogDetailLevel.None && level >= _log.CurrentLoggingLevel)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    _log.LogEntries.Add(newEntry);
                });
            }
        }

        public void AddStatisticEntry(TaskStatistic statistic)
        {
            _statistics.Statistics.Add(statistic);
            _statistics.Save(USER_STATISTICS_PATH);
        }
    }
}
