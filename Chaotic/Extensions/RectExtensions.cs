using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Extensions
{
    public static class RectExtensions
    {
        public static Rect TopRegion(this Rect rect)
        {
            return new Rect(rect.Left, rect.Top, rect.Width, rect.Height / 2);
        }

        public static Rect TopRightRegion(this Rect rect)
        {
            return new Rect((rect.Left + rect.Right) / 2, rect.Top, rect.Width / 2, rect.Height / 2);
        }
        public static Rect TopLeftRegion(this Rect rect)
        {
            return new Rect(rect.Left, rect.Top, rect.Width / 2, rect.Height / 2);
        }
        public static Rect BottomLeftRegion(this Rect rect)
        {
            return new Rect(rect.Left, (rect.Top + rect.Bottom) / 2, rect.Width / 2, rect.Height / 2);
        }
        public static Rect BottomRightRegion(this Rect rect)
        {
            return new Rect((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2, rect.Width / 2, rect.Height / 2);
        }
    }
}
