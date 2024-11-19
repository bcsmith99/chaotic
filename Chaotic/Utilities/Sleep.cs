using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Utilities
{
    internal class Sleep
    {
        internal static void SleepMs(int minMs, int maxMs)
        {
            var time = Random.Shared.Next(minMs, maxMs);

            Thread.Sleep(time);
        }
    }
}
