using Chaotic.Tasks.Una;
using Chaotic.User;
using Chaotic.Utilities;
using DeftSharp.Windows.Input.Keyboard;
using DeftSharp.Windows.Input.Mouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks
{
    public class UnaTasks
    {

        public static List<string> AvailableUnaTasks = new List<string>()
        {
            UnaTaskNames.NotSet,
            UnaTaskNames.ElgaciaLeap,
            UnaTaskNames.ElgaciaShard,
            UnaTaskNames.GhostShipLeap,
            UnaTaskNames.PlecciaShard,
            UnaTaskNames.MokoMarketLeap,
            UnaTaskNames.VoldisLeap,
            UnaTaskNames.VoldisShard,
            UnaTaskNames.SKurzanLeap,
            UnaTaskNames.NKurzanShard
        };

        private UserSettings _settings;
        private readonly MouseUtility _mouse;
        private readonly KeyboardUtility _kb;
        private readonly ResourceHelper _r;
        private readonly UITasks _uiTasks;
        private readonly AppLogger _logger;

        public UnaTasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UITasks uiTasks, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _uiTasks = uiTasks;
            _logger = logger;
        }
        public int AcceptDailies(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            int dailiesAccepted = 0; 

            if (!character.RunUnas)
                return dailiesAccepted;

            _mouse.ClickPosition(_r["AdventureMenu"], 1000);
            _mouse.ClickPosition(_r["UnaTask"], 1500);
            _mouse.ClickPosition(_r["UnaDaily"], 1000);
            _mouse.ClickPosition(_r["UnaDailyDropdown"], 300);
            _mouse.ClickPosition(_r["UnaDailyDropdownFavorite"], 500);


            //TODO: Possibly replace with image recognition for the accept button and doing it 1-n times.

            var acceptMatches = IP.LocateOnScreen(Utility.ImageResourceLocation("accept_button.png", _settings.Resolution),
                IP.ConvertStringCoordsToRect(_r["UnaDailyRegion"]), .95);

            foreach (var acceptMatch in acceptMatches.Matches)
            {
                _mouse.ClickPosition(acceptMatch.Center.X, acceptMatch.Center.Y, 500);
            }

            //_mouse.ClickPosition(_r["UnaDailyAccept1"], 500);
            //_mouse.ClickPosition(_r["UnaDailyAccept2"], 500);
            //_mouse.ClickPosition(_r["UnaDailyAccept3"], 500);

            _kb.Press(Key.Escape);

            return acceptMatches.Matches.Count;
        }

        public bool AcceptWeeklies(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();

            _mouse.ClickPosition(_r["AdventureMenu"], 1000);
            _mouse.ClickPosition(_r["UnaTask"], 1500);
            _mouse.ClickPosition(_r["UnaWeekly"], 1000);
            _mouse.ClickPosition(_r["UnaWeeklyDropdown"], 300);
            _mouse.ClickPosition(_r["UnaWeeklyDropdownFavorite"], 500);

            var acceptMatches = IP.LocateOnScreen(Utility.ImageResourceLocation("accept_button.png", _settings.Resolution),
                IP.ConvertStringCoordsToRect(_r["UnaWeeklyRegion"]), .95);
            foreach (var acceptMatch in acceptMatches.Matches)
            {
                _mouse.ClickPosition(acceptMatch.Center.X, acceptMatch.Center.Y, 500);
            }

            _kb.Press(Key.Escape);

            return true;
        }


        private void RunDailyUna(UserUnaTask userTask)
        {
            BackgroundProcessing.ProgressCheck();
            if (userTask != null && userTask.UnaName != UnaTaskNames.NotSet)
            {
                var task = UnaTask.Create(userTask.UnaName, _uiTasks, _mouse, _kb, _r, _settings, _logger);
                if (task != null && task.GetType() != typeof(NotSet))
                    task.RunUna(userTask.BifrostPosition);
            }
        }
        private void RunLopangUnas(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            var task = UnaTask.Create(UnaTaskNames.Lopang, _uiTasks, _mouse, _kb, _r, _settings, _logger);
            if (task != null && task.GetType() == typeof(Lopang))
            {
                ((Lopang)task).RunShushireOnly = character.RunLopangShushireOnly;
                task.RunUna(1);
            }
                

        }

        public bool RunDailies(UserCharacter character)
        {
            if (!character.RunUnas)
                return true;

            if (character.RunLopangUnas)
            {
                RunLopangUnas(character);
            }
            else
            {
                RunDailyUna(character.FirstUnaTask);
                RunDailyUna(character.SecondUnaTask);
                RunDailyUna(character.ThirdUnaTask);
            }

            return true;
        }


    }
}
