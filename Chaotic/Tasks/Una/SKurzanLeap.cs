using Chaotic.Tasks.Una;
using Chaotic.Tasks;
using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;
using Chaotic.Resources;

namespace Chaotic.Tasks.Una
{
    [UnaTask(UnaTaskNames.SKurzanLeap)]
    public class SKurzanLeap : UnaTask
    {
        public SKurzanLeap(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
                    : base(uiTask, mouse, kb, r, settings, logger)
        {

        }


        protected override void ExecuteTask()
        {
            var clickPoints = _r.SouthKurzanLeapClickpoints;
            _kb.Press(Key.Enter, 50);
            _kb.Press(Key.OemQuestion);
            _kb.TypeString("picturepose6");
            _kb.Press(Key.Enter);
            Sleep.SleepMs(14000, 16000);

            _mouse.ClickPosition(clickPoints[0].X, clickPoints[0].Y, 2000, MouseButtons.Right);
            _mouse.ClickPosition(clickPoints[0].X, clickPoints[0].Y, 2000, MouseButtons.Right);
            _mouse.ClickPosition(clickPoints[1].X, clickPoints[1].Y, 2200, MouseButtons.Right);

            _kb.Press(Key.G, 1000);
            _kb.ShiftPress(Key.G, 400);

            var completeButton = ImageProcessing.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), 400, 950, 200, 100, .95);
            if (completeButton.Found)
            {
                Sleep.SleepMs(100, 200);
                _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 3000);
            }

        }
    }
}
