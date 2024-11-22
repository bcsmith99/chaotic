using Chaotic.Tasks;
using Chaotic.Tasks.Chaos;
using Chaotic.Tasks.Chaos.Class;
using Chaotic.Tasks.Chaos.Kurzan;
using Chaotic.Tasks.Una;
using Chaotic.User;
using Chaotic.Utilities;
using DeftSharp.Windows.Input.Keyboard;
using DeftSharp.Windows.Input.Mouse;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Chaotic\\";
            _busy = new ManualResetEvent(false);

            if (!Directory.Exists(appDataDirectory))
                Directory.CreateDirectory(appDataDirectory);

            _settings = new UserSettings();
            _settings = _settings.Read(USER_SETTINGS_PATH);

            _statistics = new SessionStatistics();
            //_statistics = _statistics.Read(USER_STATISTICS_PATH);

            Log = new SessionLog(_settings.LogDetailLevel);
            _logger = new AppLogger(Log, Statistics);

            InitializeComponent();

            _mouse = new MouseUtility();
            _kb = new KeyboardUtility();

            InitializeTasks();

        }

        private List<string> _Resolutions = new List<string> { "3440x1440", "2560x1440" };
        private List<Tuple<string, int>> _GemLevels = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>("Level 3+", 3),
            new Tuple<string, int>("Level 4+", 4),
            new Tuple<string, int>("Level 5+", 5),
            new Tuple<string, int>("Level 6+", 6),
            new Tuple<string, int>("Level 7+", 7),
            new Tuple<string, int>("Level 8+", 8),
            new Tuple<string, int>("Level 9+", 9),
        };

        private List<Tuple<string, LogDetailLevel>> _LogLevels = new List<Tuple<string, LogDetailLevel>>()
        {
            new Tuple<string, LogDetailLevel>("None", LogDetailLevel.None),
            new Tuple<string, LogDetailLevel>("Debug", LogDetailLevel.Debug),
            new Tuple<string, LogDetailLevel>("Info", LogDetailLevel.Info),
            new Tuple<string, LogDetailLevel>("Summary", LogDetailLevel.Summary)
        };

        private ResourceHelper _r;
        private UITasks _uit;
        private GuildTasks _gt;
        private UnaTasks _ut;
        private ChaosTasks _ct;
        private readonly MouseUtility _mouse;
        private readonly KeyboardUtility _kb;
        private SessionLog _log;
        private readonly AppLogger _logger;


        public SessionLog Log
        {
            get { return _log; }
            set
            {
                if (_log != null)
                    _log.LogEntries.CollectionChanged -= LogEntriesChanged;

                _log = value;
                _log.LogEntries.CollectionChanged += LogEntriesChanged;
                OnPropertyChanged();
            }
        }


        public List<String> Resolutions { get { return _Resolutions; } set { _Resolutions = value; } }
        public List<Tuple<string, int>> GemLevels { get { return _GemLevels; } set { _GemLevels = value; } }
        public List<Tuple<string, LogDetailLevel>> LogLevels { get { return _LogLevels; } set { _LogLevels = value; } }
        private List<int> _AvailableBifrosts = Enumerable.Range(1, 6).ToList();
        public List<int> AvailableBifrosts { get { return _AvailableBifrosts; } }
        public List<String> AvailableClasses { get { return UserCharacter.AllClasses; } }
        public List<String> AvailableUnaTaskNames { get { return UnaTasks.AvailableUnaTasks; } }

        private List<int> _AvailableChaosLevels = new List<int>()
        {
            1415,
            1445,
            1475,
            1490,
            1520,
            1540,
            1560,
            1580,
            1600,
            1610,
            1640,
            1660
        };

        public List<int> AvailableChaosLevels { get { return _AvailableChaosLevels; } }
        private List<int> _AvailableIndexes = Enumerable.Range(1, 30).ToList();
        public List<int> AvailableIndexes { get { return _AvailableIndexes; } }
        private UserCharacter? _CurrentDailySelectedChar;
        public UserCharacter? CurrentDailySelectedChar
        {
            get { return _CurrentDailySelectedChar; }
            set
            {
                _CurrentDailySelectedChar = value;
                OnPropertyChanged();
            }
        }

        private LogEntry? _CurrentLogEntry;
        public LogEntry? CurrentLogEntry
        {
            get { return _CurrentLogEntry; }
            set
            {
                _CurrentLogEntry = value;
                OnPropertyChanged();
            }
        }

        private UserCharacter? _CurrentSelectedEditChar;
        public UserCharacter? CurrentSelectedEditChar
        {
            get { return _CurrentSelectedEditChar; }
            set
            {
                _CurrentSelectedEditChar = value;
                OnPropertyChanged();
            }
        }

        private bool _taskRunning;
        public bool TaskRunning
        {
            get { return this._taskRunning; }
            set
            {
                this._taskRunning = value;
                OnPropertyChanged();
            }
        }
        private UserSettings _settings;
        private SessionStatistics _statistics;

        public UserSettings UserSettings
        {
            get { return this._settings; }
            set
            {
                this._settings = value;
                OnPropertyChanged();
            }
        }

        public SessionStatistics Statistics
        {
            get { return this._statistics; }
            set
            {
                this._statistics = value;
                OnPropertyChanged();
            }
        }


        private string USER_STATISTICS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Chaotic\\userStatistics.json";


        //System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().Location).LocalPath) + "\\userStatistics.json";

        private string USER_SETTINGS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Chaotic\\userSettings.json";
        //System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().Location).LocalPath) + "\\userSettings.json";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



        private void InitializeTasks()
        {
            _r = new ResourceHelper("Chaotic.Resources." + UserSettings.Resolution);
            _uit = new UITasks(UserSettings, _mouse, _kb, _r, _logger, _busy);
            _gt = new GuildTasks(UserSettings, _mouse, _kb, _r, _logger, _busy);
            _ut = new UnaTasks(UserSettings, _mouse, _kb, _r, _uit, _logger, _busy);
            _ct = new ChaosTasks(UserSettings, _mouse, _kb, _r, _uit, _logger, _busy);
        }

        private DateTime _currentWorkStartTime;
        private BackgroundWorker _bw;
        private readonly ManualResetEvent _busy;

        private bool ShouldAcceptWeeklies()
        {
            bool accept = false;
            DateTime lastReset;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday)
                lastReset = DateTime.Today.Date;
            else
            {
                DateTime today = DateTime.Today.Date;
                int diff = DateTime.Today.DayOfWeek - DayOfWeek.Wednesday;
                DateTime lastWednesday = diff > 0 ? today.AddDays(-1 * diff) : today.AddDays(-1 * (7 + diff));
                lastReset = lastWednesday;
            }

            if (!_settings.LastWeeklyReset.HasValue || lastReset > _settings.LastWeeklyReset)
                accept = true;

            return accept;
        }

        private void Resolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _r = new ResourceHelper("Chaotic.Resources." + UserSettings.Resolution);
            SaveUserSettings();

            InitializeTasks();
        }

        private void SaveUserSettings()
        {
            UserSettings.Save(USER_SETTINGS_PATH);
        }

        private bool RunCharacterDailyRotation(UserCharacter character, bool shouldAcceptWeeklies = false)
        {
            _mouse.ClickCenterScreen(_r);

            bool success = true;
            success = _gt.PerformGuildTasks(character);
            int dailiesAccepted = _ut.AcceptDailies(character);
            if (dailiesAccepted > 0)
                success = success && _ut.RunDailies(character);

            if (success && shouldAcceptWeeklies)
                success = success & _ut.AcceptWeeklies(character);

            if (success && character.RunChaos)
                success = _ct.RunChaos(character);

            _uit.ClearOngoingQuests();

            Sleep.SleepMs(1000, 2000);

            if (success && UserSettings.EnableAura)
            {
                if (UserSettings.RepairGear)
                    success = success && _uit.AuraRepair();

                if (success && (UserSettings.MoveHoningMaterials || UserSettings.MoveGems))
                {
                    success = success && _uit.OpenInventoryManagement();
                    if (UserSettings.MoveHoningMaterials)
                        success = success && _uit.MoveHoningMaterials();
                    if (UserSettings.MoveGems && !character.IsMain)
                        success = success && _uit.MoveGems();
                    success = success && _uit.CloseInventoryManagement();
                }
            }
            return success;
        }

        private void RunCharacterUnas_Click(object sender, RoutedEventArgs e)
        {
            Action a = () =>
            {
                _mouse.ClickCenterScreen(_r);
                if (CurrentDailySelectedChar != null)
                    _ut.RunDailies(CurrentDailySelectedChar);
            };
            CreateBackgroundWorker(a);
        }

        private void RunCharacterChaos_Click(object sender, RoutedEventArgs e)
        {
            Action a = () =>
            {
                if (CurrentDailySelectedChar != null)
                    _ct.RunChaos(CurrentDailySelectedChar);
            };
            CreateBackgroundWorker(a);
        }

        private void RunCharacterDailies_Click(object sender, RoutedEventArgs e)
        {
            Action a = () =>
            {
                if (CurrentDailySelectedChar != null)
                {
                    var success = RunCharacterDailyRotation(CurrentDailySelectedChar);
                    if (success)
                        _logger.Log(LogDetailLevel.Summary, "Character Daily Rotation Complete");
                }
            };

            CreateBackgroundWorker(a);
        }

        private void RunAllDailyRotation_Click(object sender, RoutedEventArgs e)
        {
            Action a = () =>
            {
                Thread.Sleep(1000);

                bool acceptWeeklies = ShouldAcceptWeeklies();

                var charsToRun = UserSettings.Characters.Where(x => x.IsCharSelected).ToList();

                for (int i = 0; i < charsToRun.Count; i++)
                {
                    var character = charsToRun[i];
                    var success = RunCharacterDailyRotation(character, acceptWeeklies);

                    if (!success)
                        break;

                    if (i == charsToRun.Count - 1)
                        break;

                    if (_uit.SwapCharacters(charsToRun[i + 1]))
                    {
                        var backInTown = _uit.InAreaCheck();
                        if (!backInTown)
                            break;
                    }
                }

                if (acceptWeeklies)
                {
                    UserSettings.LastWeeklyReset = DateTime.Now.Date;
                    SaveUserSettings();
                }
            };

            CreateBackgroundWorker(a);
        }


        private void SaveUserSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveUserSettings();
        }

        private void CancelUserSettings_Click(object sender, RoutedEventArgs e)
        {
            UserSettings = UserSettings.Read(USER_SETTINGS_PATH);
        }

        private void AddNewCharacter_Click(object sender, RoutedEventArgs e)
        {
            var newChar = new UserCharacter()
            {
                ClassName = "Unset",
                CharacterIndex = UserSettings.Characters.Count == 0 ? 1 : UserSettings.Characters.Max(x => x.CharacterIndex) + 1,
                ChaosLevel = 1660
            };
            CurrentSelectedEditChar = newChar;
            UserSettings.Characters.Add(newChar);
        }

        private void DeleteCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSelectedEditChar != null)
            {
                UserSettings.Characters.Remove(CurrentSelectedEditChar);
                CurrentSelectedEditChar = null;
            }
        }

        private void CaptureSkills_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSelectedEditChar != null)
            {

                foreach (var skill in CurrentSelectedEditChar.Skills.AllSkills)
                {
                    if (skill == null) continue;

                    var region = ImageProcessing.ConvertStringCoordsToRect(_r[$"Skill_{skill.SkillKey}"]);

                    if (skill.IsAwakening && CurrentSelectedEditChar.ChaosLevel >= 1640)
                        region = ImageProcessing.ConvertStringCoordsToRect(_r[$"Skill_Hyper{skill.SkillKey}"]);

                    if (skill.SkillKey == "T" && CurrentSelectedEditChar.ChaosLevel < 1640)
                        continue;

                    var skillScreenshot = ImageProcessing.CaptureScreen(region);

                    if (skillScreenshot != null)
                    {

                        using (var ms = new MemoryStream())
                        {
                            skillScreenshot.Save(ms, ImageFormat.Png);
                            var sig64 = Convert.ToBase64String(ms.GetBuffer());
                            skill.SkillImageEncoded = sig64;
                        }
                    }
                }
            }
        }

        private void StartWorker()
        {
            _blocked = false;
            _busy.Set();

            if (_bw != null && !_bw.IsBusy)
            {
                //_logger.Log(LogDetailLevel.Debug, "Listening for pause");
                _kb.Listen(Key.Pause, () =>
                {
                    _logger.Log(LogDetailLevel.Debug, "Pause pressed");
                    TogglePause();
                    //CancelWorker();
                });
                _bw.RunWorkerAsync();
                TaskRunning = true;
            }
        }

        private bool _blocked = false;
        private DateTime _currentWorkEndTime;

        private void TogglePause()
        {
            if (_blocked)
                _busy.Set();
            else
                _busy.Reset();

            _blocked = !_blocked;
        }

        private void CancelWorker()
        {
            if (_bw.IsBusy)
            {
                _logger.Log(LogDetailLevel.Debug, "Cancelling worker");
                _bw.CancelAsync();
                TaskRunning = false;

                // Unblock worker so it can see that
                _busy.Set();
            }
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _currentWorkEndTime = DateTime.Now; 
            _logger.WriteSessionWorkCompleted(_currentWorkStartTime, _currentWorkEndTime);
            //_logger.Log(LogDetailLevel.Debug, "Finished Work");
            _kb.StopListening(Key.Pause);
            TaskRunning = false;
        }

        private void CreateBackgroundWorker(Action workToDo)
        {
            _currentWorkStartTime = DateTime.Now;
            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += (o, e) =>
            {
                try
                {
                    workToDo();
                }
                catch (WorkStoppedException)
                {
                    _logger.Log(LogDetailLevel.Debug, "Work Stopped");
                }
            };
            _bw.RunWorkerCompleted += WorkCompleted;
            StartWorker();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Action a = () =>
            {

                var donate = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("donate_button.png", _settings.Resolution), confidence: .65);
                if (donate.Found)
                    _logger.Log(LogDetailLevel.Info, $"Donate found. Confidence : {donate.MaxConfidence}");



                //_logger.AddStatisticEntry(new ChaosTaskStatistic()
                //{
                //    ChaosLevel = 1540,
                //    Class = "Balh",
                //    CharacterIdentifier = Guid.NewGuid(),
                //    StartDate = DateTime.Now,
                //    StatisticType = "ChaosDungeon",
                //    TaskOutcome = "Success",
                //    TotalDuration = new TimeSpan(0, 0, 300),
                //    Floor1Duration = new TimeSpan(0, 0, 50),
                //    Floor2Duration = new TimeSpan(0, 0, 100),
                //    Floor3Duration = new TimeSpan(0, 0, 150)
                //});
            };

            CreateBackgroundWorker(a);
        }

        private void LogDetailLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _log.CurrentLoggingLevel = UserSettings.LogDetailLevel;
            _log.RefreshVisibleEntries();
        }

        private void LogView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            if (VisualTreeHelper.GetChildrenCount(listView) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(listView, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }


        private void LogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CurrentLogEntry = _log.LogEntries.LastOrDefault();
        }
    }
}