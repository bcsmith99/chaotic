using Chaotic.Resources;
using Chaotic.Tasks.Chaos.Class;
using Chaotic.User;
using Chaotic.Utilities;
using OpenCvSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks
{
    public class CubeTasks
    {
        private UserSettings _settings;
        private MouseUtility _mouse;
        private KeyboardUtility _kb;
        private ApplicationResources _r;
        private UITasks _uiTasks;
        private AppLogger _logger;

        public CubeTasks(UserSettings settings, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UITasks uiTasks, AppLogger logger)
        {
            _settings = settings;
            _mouse = mouse;
            _kb = kb;
            _r = r;
            _uiTasks = uiTasks;
            _logger = logger;

            CenterScreen = _r.CenterScreen;
            MoveToPoint = CenterScreen;
            ClickableRegion = _r.ClickableRegion;
            ClickableOffset = _r.ClickableOffset;
        }

        public Point MoveToPoint { get; private set; }
        public Rect ClickableRegion { get; }
        public Point CenterScreen { get; }
        public Point ClickableOffset { get; }


        public bool RunCube(UserCharacter character)
        {
            BackgroundProcessing.ProgressCheck();
            var success = true;

            if (character == null)
                return true;

            var cc = ChaosClass.Create(_settings, character, _r, _kb, _mouse, _logger);

            while (true)
            {
                var completeCheck = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("cube_floor_complete.png", _settings.Resolution), confidence: .7);
                if (completeCheck.Found)
                    MoveToNextFloor();

                cc.UseAbilities(CenterScreen, 4);
            }

            return success;
        }

        private void MoveToNextFloor()
        {
            throw new NotImplementedException();
        }
    }
}
