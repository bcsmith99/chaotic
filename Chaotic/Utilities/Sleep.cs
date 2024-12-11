using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Utilities
{
    internal class Sleep
    {
        internal static void SleepMs(int minMs, int maxMs, double multiplier = 1)
        {
            var time = Random.Shared.Next((int)Math.Round(minMs * multiplier), (int)Math.Round(maxMs * multiplier));

            Thread.Sleep(time);
        }
    }
}
