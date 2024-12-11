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
    public class Lopang : UnaTask
    {
        public bool RunShushireOnly { get; set; } = false;
        public Lopang(UITasks uiTask, MouseUtility mouse, KeyboardUtility kb, ResourceHelper r, UserSettings settings, AppLogger logger) : base(uiTask, mouse, kb, r, settings, logger)
        {

        }
        protected override void ExecuteTask()
        {
            RunLopangRoute();

            if (!RunLopangLocation(2)) return;
            if (RunShushireOnly) return;
            if (!RunLopangLocation(4)) return;
            if (!RunLopangLocation(5)) return;
        }

        public bool RunLopangLocation(int bifrostPoint)
        {
            if (BifrostToPoint(bifrostPoint))
            {
                _kb.Press(Key.G, 1500);
                _kb.ShiftPress(Key.G, 1500);
                _kb.ShiftPress(Key.G, 3000);
                return true;
            }

            return false;
        }

        public void RunLopangRoute()
        {
            _mouse.ClickCenterScreen(_r);

            var routePositions = IP.ConvertPointArray(_r["UnaLopang_Route"]);

            _kb.Press(Key.G, 500);
            _kb.Press(Key.G, 500);

            if (RunShushireOnly)
                return; 

            _mouse.ClickPosition(routePositions[0], 1800, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[1], 1500, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[2], 1500, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[3], 2000, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[4], 2100, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[5], 2000, MouseButtons.Right);

            //Sleep.SleepMs(1000, 1200);
            _kb.Press(Key.G, 500);
            _kb.Press(Key.G, 500);

            _mouse.ClickPosition(routePositions[6], 1700, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[7], 1700, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[8], 1700, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[9], 1700, MouseButtons.Right);
            _mouse.ClickPosition(routePositions[10], 2000, MouseButtons.Right);

            _kb.Press(Key.G, 500);
            _kb.Press(Key.G, 500);

            Sleep.SleepMs(1000, 1500);
        }
    }
}
