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
    [UnaTask(UnaTaskNames.PraeteriaSolar)]
    public class PraeteriaSolar : UnaTask
    {
        public PraeteriaSolar(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ApplicationResources r, UserSettings settings, AppLogger logger)
       : base(uiTask, mouse, kb, r, settings, logger)
        {

        }
        public override void ExecuteTask()
        {
            _mouse.ClickCenterScreen(_r.CenterScreen);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[0], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[0], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[0], 1500, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[0], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[0], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[0], 1500, MouseButtons.Left);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[1], 3500, MouseButtons.Right);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[2], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[2], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[2], 1500, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[2], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[2], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[2], 1500, MouseButtons.Left);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[3], 2500, MouseButtons.Right);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[4], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[4], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[4], 1500, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[4], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[4], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[4], 1500, MouseButtons.Left);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[5], 3500, MouseButtons.Right);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[6], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[6], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[6], 1500, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[6], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[6], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[6], 1500, MouseButtons.Left);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[7], 3500, MouseButtons.Right);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[8], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[8], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[8], 1500, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[8], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[8], 300, MouseButtons.Left);
            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[8], 1500, MouseButtons.Left);

            _mouse.ClickPosition(_r.PraeteriaSolarClickpoints[9], 3500, MouseButtons.Right);

            _kb.Press(Key.G, 1500);
            _kb.ShiftPress(Key.G, 1000);

            var completeButton = IP.LocateCenterOnScreen(Utility.ImageResourceLocation("complete_button.png", _settings.Resolution), _r.Complete, .95);
            if (completeButton.Found)
                _mouse.ClickPosition(completeButton.CenterX, completeButton.CenterY, 3000);
            else
                _kb.Press(Key.G, 3000);

        }
    }
}
