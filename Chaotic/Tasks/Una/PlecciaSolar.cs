using Chaotic.Resources;
using Chaotic.User;
using Chaotic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IP = Chaotic.Utilities.ImageProcessing;

namespace Chaotic.Tasks.Una
{
    public class PlecciaSolar : UnaTask
    {

        public PlecciaSolar(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
       : base(uiTask, mouse, kb, r, settings, logger)
        {

        }
        public override void ExecuteTask()
        {
            _mouse.ClickCenterScreen(_r.CenterScreen);

            _kb.Press(Key.G, 5000);
            _kb.Press(Key.G, 5000);
            _kb.Press(Key.G, 5000);

            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[0], 2500, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[1], 2000, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[2], 2000, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[3], 10000, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[4], 5000, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[5], 2000, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[6], 2000, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[7], 1500, MouseButtons.Right);
            _mouse.ClickPosition(_r.PlecciaSolarClickpoints[8], 2000, MouseButtons.Right);

            _kb.Press(Key.G, 4000);

            _kb.ShiftPress(Key.G, 1000);

            var completeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), _r.Complete, .95);
            if (completeButton.Found)
                _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 3000);
            else
                _kb.Press(Key.G, 3000);
        }
    }
}
