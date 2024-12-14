using Chaotic.Resources;
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
    [UnaTask(UnaTaskNames.ElgaciaShard)]
    public class ElgaciaShard : UnaTask
    {
        public ElgaciaShard(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
            : base(uiTask, mouse, kb, r, settings, logger)
        {

        }

        public override void ExecuteTask()
        {
            var clickPoints = _r.ElgaciaShardClickpoints; // IP.ConvertPointArray(_r["ElgaciaShard_Clickpoints"]);
            _kb.Press(Key.G, 1000);

            _kb.ShiftPress(Key.G, 1000);

            _kb.Press(Key.G, 2000);

            _mouse.ClickPosition(clickPoints[0], 2000, MouseButtons.Right);
            _mouse.ClickPosition(clickPoints[0], 2000, MouseButtons.Right);

            _kb.Press(Key.Enter, 50);
            _kb.Press(Key.OemQuestion, 50);
            _kb.TypeString("wave");
            _kb.Press(Key.Enter);

            Sleep.SleepMs(6500, 7000);

            _kb.Press(Key.G, 7000);
            _kb.Press(Key.G, 5500);
            _kb.Press(Key.G, 5500);

            _mouse.ClickPosition(clickPoints[1], 2600, MouseButtons.Right);
            _mouse.ClickPosition(clickPoints[2], 1700, MouseButtons.Right);

            _kb.Press(Key.G, 1000);
            _kb.ShiftPress(Key.G, 2000);

            var completeRegion = _r.Complete;
            var completeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), completeRegion, .95);
            if (completeButton.Found)
            {
                Sleep.SleepMs(100, 200);
                _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 3000);
            }
            else
                _kb.Press(Key.G, 3000);
        }
    }
}
