using Chaotic.Statistics;
using Chaotic.Tasks;
using Chaotic.Tasks.Chaos;
using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chaotic
{
    /// <summary>
    /// Interaction logic for StatisticsControl.xaml
    /// </summary>
    public partial class StatisticsControl : UserControl, INotifyPropertyChanged
    {
        private SessionStatistics _statistics;
        private string USER_STATISTICS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Chaotic\\userStatistics.json";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public StatisticsControl()
        {
            _statistics = new SessionStatistics();
            ClassStatistics = new ObservableCollection<ClassStatistic>();
            DateStatistics = new ObservableCollection<DateStatistic>();
            TypeStatistics = new ObservableCollection<TypeStatistic>();

            LoadStatistics();

            InitializeComponent();

            IsClassGrouping = true;
        }


        private ObservableCollection<TypeStatistic> _typeStatistics;
        public ObservableCollection<TypeStatistic> TypeStatistics
        {
            get { return _typeStatistics; }
            set
            {
                _typeStatistics = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ClassStatistic> _classStatistics;
        public ObservableCollection<ClassStatistic> ClassStatistics
        {
            get { return _classStatistics; }
            set
            {
                _classStatistics = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DateStatistic> _dateStatistics;
        public ObservableCollection<DateStatistic> DateStatistics
        {
            get { return _dateStatistics; }
            set
            {
                _dateStatistics = value;
                OnPropertyChanged();
            }
        }


        private bool _isClassGrouping;
        public bool IsClassGrouping
        {
            get { return _isClassGrouping; }
            set
            {
                _isClassGrouping = value;
                LoadStatistics();
                OnPropertyChanged();
            }
        }


        private bool _isTypeGrouping;
        public bool IsTypeGrouping
        {
            get { return _isTypeGrouping; }
            set
            {
                _isTypeGrouping = value;
                LoadStatistics();
                OnPropertyChanged();
            }
        }


        private bool _isDateGrouping;
        public bool IsDateGrouping
        {
            get { return _isDateGrouping; }
            set
            {
                _isDateGrouping = value;
                LoadStatistics();
                OnPropertyChanged();
            }
        }



        private void LoadStatistics()
        {
            _statistics = _statistics.Read(USER_STATISTICS_PATH);
            if (IsClassGrouping)
                LoadClassStatistics();
            else if (IsTypeGrouping)
                LoadTypeStatistics();
            else if (IsDateGrouping)
                LoadDateStatistics();
        }

        private void LoadDateStatistics()
        {
            DateStatistics.Clear();
            var dates = _statistics.Statistics.Select(x => x.StartDate.Date).Distinct();

            var statistics = dates.Select(x =>
            {
                var stat = new DateStatistic()
                {
                    ExecutionDate = x,

                    SuccessRate = Math.Round((((double)_statistics.Statistics.Where(y => y.StartDate.Date == x && y.TaskOutcome == TaskOutcomes.Success).Count() / (double)_statistics.Statistics.Where(y => y.StartDate.Date == x).Count()) * 100), 2),
                    KurzanStatistics = _statistics.Statistics.Where(y => y.GetType() == typeof(KurzanTaskStatistic) && y.StartDate.Date == x).Cast<KurzanTaskStatistic>().ToList(),
                    ChaosStatistics = _statistics.Statistics.Where(y => y.GetType() == typeof(ChaosTaskStatistic) && y.StartDate.Date == x).Cast<ChaosTaskStatistic>().ToList()
                };
                stat.Compute();
                return stat;
            });

            foreach (var stat in statistics.OrderByDescending(x => x.ExecutionDate))
            {
                DateStatistics.Add(stat);
            }
        }

        private void LoadTypeStatistics()
        {
            TypeStatistics.Clear();
            var stat = new TypeStatistic()
            {
                SuccessRate = Math.Round((((double)_statistics.Statistics.Where(y => y.TaskOutcome == TaskOutcomes.Success).Count() / (double)_statistics.Statistics.Count()) * 100), 2),
                KurzanSuccessRate = Math.Round((((double)_statistics.Statistics.Count(y => y.GetType() == typeof(KurzanTaskStatistic) && y.TaskOutcome == TaskOutcomes.Success) / (double)_statistics.Statistics.Count(y => y.GetType() == typeof(KurzanTaskStatistic))) * 100), 2),
                ChaosSuccessRate = Math.Round((((double)_statistics.Statistics.Count(y => y.GetType() == typeof(ChaosTaskStatistic) && y.TaskOutcome == TaskOutcomes.Success) / (double)_statistics.Statistics.Count(y => y.GetType() == typeof(ChaosTaskStatistic))) * 100), 2),
                KurzanStatistics = _statistics.Statistics.Where(y => y.GetType() == typeof(KurzanTaskStatistic)).Cast<KurzanTaskStatistic>().ToList(),
                ChaosStatistics = _statistics.Statistics.Where(y => y.GetType() == typeof(ChaosTaskStatistic)).Cast<ChaosTaskStatistic>().ToList()
            };
            stat.Compute();

            TypeStatistics.Add(stat);
        }

        private void LoadClassStatistics()
        {
            ClassStatistics.Clear();
            var classes = UserCharacter.AllClasses;
            var statistics = UserCharacter.AllClasses.OrderBy(x => x).Select(x =>
            {
                var stat = new ClassStatistic()
                {
                    ClassName = x,
                    SuccessRate = Math.Round((((double)_statistics.Statistics.Where(y => y.Class == x && y.TaskOutcome == TaskOutcomes.Success).Count() / (double)_statistics.Statistics.Where(y => y.Class == x).Count()) * 100), 2),
                    KurzanStatistics = _statistics.Statistics.Where(y => y.GetType() == typeof(KurzanTaskStatistic) && y.Class == x).Cast<KurzanTaskStatistic>().ToList(),
                    ChaosStatistics = _statistics.Statistics.Where(y => y.GetType() == typeof(ChaosTaskStatistic) && y.Class == x).Cast<ChaosTaskStatistic>().ToList()
                };

                stat.Compute();
                return stat;
            });

            foreach (var stat in statistics)
                ClassStatistics.Add(stat);
        }

        //private void LoadStatistics_Click(object sender, RoutedEventArgs e)
        //{
        //    LoadStatistics();
        //}
    }
}
