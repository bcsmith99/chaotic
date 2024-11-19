using DeftSharp.Windows.Input.Mouse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Utilities
{
    public enum MouseButtons
    {
        Left,
        Right,
        Middle
    }

    public class MouseUtility
    {


        public enum ScrollDirection
        {
            Up,
            Down
        }
        private readonly MouseManipulator _mm;

        public MouseUtility()
        {
            this._mm = new MouseManipulator();
        }

        internal void SetPosition(int x, int y)
        {
            _mm.SetPosition(x, y);
        }

        internal void SetPosition(string x, string y)
        {
            SetPosition(Int32.Parse(x), Int32.Parse(y));
        }

        internal void SetPosition(string coordinates)
        {
            var positions = coordinates.Split(',');
            SetPosition(positions[0], positions[1]);
        }

        public void ClickTopScreen(ResourceHelper rh)
        {
            var positions = rh["CenterScreen"].Split(',');
            ClickPosition(positions[0], "5", 500, MouseButtons.Left);
        }

        public void ClickCenterScreen(ResourceHelper rh)
        {
            ClickPosition(rh["CenterScreen"], 500, MouseButtons.Right);
        }
        internal void ClickPosition(OpenCvSharp.Point position, int waitAfter = 0, MouseButtons mb = MouseButtons.Left)
        {
            ClickPosition(position.X, position.Y, waitAfter, mb);
        }
        internal void ClickPosition(double x, double y, int waitAfter = 0, MouseButtons mb = MouseButtons.Left)
        {
            ClickPosition((int)x, (int)y, waitAfter, mb);
        }


        internal void ClickPosition(int x, int y, int waitAfter = 0, MouseButtons mb = MouseButtons.Left)
        {
            SetPosition(x, y);
            Sleep.SleepMs(50, 100);
            _mm.Click((MouseButton)mb);
            if (waitAfter > 0)
                Sleep.SleepMs(waitAfter, waitAfter + 100);
        }

        internal void ClickPosition(string x, string y, int waitAfter = 0, MouseButtons mb = MouseButtons.Left)
        {
            SetPosition(x, y);
            Sleep.SleepMs(50, 100);
            _mm.Click((MouseButton)mb);
            if (waitAfter > 0)
                Sleep.SleepMs(waitAfter, waitAfter + 100);
        }

        internal void ClickPosition(string coordinates, int waitAfter = 0, MouseButtons mb = MouseButtons.Left)
        {
            var positions = coordinates.Split(',');
            ClickPosition(positions[0], positions[1], waitAfter, mb);
        }



        internal void Scroll(ScrollDirection direction, int numTimes, int delay = 400)
        {
            var scrollAmount = direction == ScrollDirection.Up ? 1 : -1;

            while (numTimes > 0)
            {
                _mm.Scroll(scrollAmount);
                if (delay > 0)
                    Sleep.SleepMs(delay, delay + 50);
                numTimes--;
            }
        }
    }
}
