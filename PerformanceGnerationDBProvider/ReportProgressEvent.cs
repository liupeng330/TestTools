using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceGnerationDBProvider
{
    public class ReportProgressEventArgs: EventArgs
    {
        public int Progress { get; private set; }

        public ReportProgressEventArgs(int progress)
        {
            this.Progress = progress;
        }
    }
}
