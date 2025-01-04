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
        Middle,
        Thumb1,
        Thumb2
    }

    public class MouseUtility
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public const int MOUSEEVENT_XDOWN = 0x0080;
        public const int MOUSEEVENT_XUP = 0x0100;

        public const int MOUSE_XBUTTON1 = 0x0001;
        public const int MOUSE_XBUTTON2 = 0x0002;


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

        internal void SetPosition(OpenCvSharp.Point p)
        {
            _mm.SetPosition(p.X, p.Y);
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

        public void ClickTopScreen(OpenCvSharp.Point centerScreen)
        {
            ClickPosition(centerScreen.X, 5, 500, MouseButtons.Left);
        }

        public void ClickCenterScreen(OpenCvSharp.Point centerPoint)
        {
            ClickPosition(centerPoint, 500, MouseButtons.Right);
        }
        internal void ClickPosition(OpenCvSharp.Point position, int waitAfter = 0, MouseButtons mb = MouseButtons.Left, double performanceModifier = 1)
        {
            ClickPosition(position.X, position.Y, waitAfter, mb, performanceModifier);
        }
        internal void ClickPosition(double x, double y, int waitAfter = 0, MouseButtons mb = MouseButtons.Left, double performanceModifier = 1)
        {
            ClickPosition((int)x, (int)y, waitAfter, mb, performanceModifier);
        }


        internal void ClickPosition(int x, int y, int waitAfter = 0, MouseButtons mb = MouseButtons.Left, double performanceModifier = 1)
        {
            SetPosition(x, y);
            Sleep.SleepMs(50, 100);
            _mm.Click((MouseButton)mb);
            if (waitAfter > 0)
                Sleep.SleepMs(waitAfter, waitAfter + 100, performanceModifier);
        }

        internal void ClickPosition(string x, string y, int waitAfter = 0, MouseButtons mb = MouseButtons.Left, double performanceModifier = 1)
        {
            SetPosition(x, y);
            Sleep.SleepMs(50, 100);
            if (mb == MouseButtons.Thumb1 || mb == MouseButtons.Thumb2)
            {
                SendNativeMouseClick(Int32.Parse(x), Int32.Parse(y), mb);
            }
            else
            {
                _mm.Click((MouseButton)mb);
            }

            if (waitAfter > 0)
                Sleep.SleepMs(waitAfter, waitAfter + 100, performanceModifier);
        }

        private void SendNativeMouseClick(int x, int y, MouseButtons mb)
        {
            if (mb != MouseButtons.Thumb1 && mb != MouseButtons.Thumb2)
                return;
            int dwData = MOUSE_XBUTTON1;
            if (mb == MouseButtons.Thumb2)
                dwData = MOUSE_XBUTTON2;

            mouse_event(MOUSEEVENT_XDOWN, x, y, dwData, 0);
            Sleep.SleepMs(20, 50);
            mouse_event(MOUSEEVENT_XUP, x, y, dwData, 0);
        }

        internal void ClickPosition(string coordinates, int waitAfter = 0, MouseButtons mb = MouseButtons.Left)
        {
            var positions = coordinates.Split(',');
            ClickPosition(positions[0], positions[1], waitAfter, mb);
        }

        internal void Scroll(ScrollDirection direction, int numTimes, int delay = 300)
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
