using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks.Una
{
    [UnaTask(UnaTaskNames.PlecciaShard)]
    public class PlecciaShard : UnaTask
    {
        public PlecciaShard(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UserSettings settings, AppLogger logger)
            : base(uiTask, mouse, kb, r, settings, logger)
        {

        }
        protected override void ExecuteTask()
        {
            _kb.Press(Key.Enter, 50);
            _kb.Press(Key.OemQuestion);
            _kb.TypeString("joy");
            _kb.Press(Key.Enter);
            Sleep.SleepMs(8000, 10000);

            var clickPoints = IP.ConvertPointArray(_r["PlecciaShard_Clickpoints"]);

            _mouse.ClickPosition(clickPoints[0], 3000, MouseButtons.Right);

            _kb.Press(Key.G, 1500);

            _kb.Hold(Key.LeftShift);
            _kb.Press(Key.G, 500);
            _kb.Release(Key.LeftShift);

            var completeRegion = IP.ConvertStringCoordsToRect(_r["Complete_Region"]);

            var completeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), completeRegion, .95);
            if (completeButton.Found)
                _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 3000);
            else
                _kb.Press(Key.G, 3000);

        }
    }
}
