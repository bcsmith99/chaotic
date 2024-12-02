using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaotic.Utilities
{
    internal static class BackgroundProcessing
    {
        private static BackgroundWorker? _bw;
        private static ManualResetEvent? _mre;
        private static DoWorkEventArgs? _dwe;

        public static void SetVariables(BackgroundWorker bw, ManualResetEvent mre, DoWorkEventArgs dwe)
        {
            _bw = bw;
            _mre = mre;
            _dwe = dwe;
        }

        public static void ProgressCheck()
        {
            if (_bw != null && _mre != null && _dwe != null)
            {
                _mre.WaitOne();
                if (_bw.CancellationPending)
                {
                    _dwe.Cancel = true;
                    throw new BackgroundCancellationException();
                }
            }
        }
    }

    public class BackgroundCancellationException : Exception
    {

    }
}
