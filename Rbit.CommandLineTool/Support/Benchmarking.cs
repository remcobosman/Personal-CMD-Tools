using System;
using System.Diagnostics;

namespace Rbit.CommandLineTool.Support
{
    internal class Benchmarking
    {
        private readonly Stopwatch stopwatch;

        internal Benchmarking()
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }

        internal TimeSpan Remaining(int totalCount, int doneCount)
        {
            long average = (this.stopwatch.ElapsedMilliseconds / doneCount);
            int remaining = totalCount - (doneCount);
            return new TimeSpan(0, 0, 0, 0, (int)(remaining * average));
        }
    }
}
