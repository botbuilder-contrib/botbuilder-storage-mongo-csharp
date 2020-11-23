using System;
using System.Linq;

namespace StateManagementBot
{
    public class ProgressData
    {
        public Progress Current { get; set; }

        public bool AdvanceIf(Func<bool> condition)
        {
            if (!condition()) { return false; }
            
            var max = Enum.GetValues(typeof(Progress)).Cast<int>().Max();

            if ((int)Current == max) { return false; }

            Current = (Progress)((int)Current++);
            return true;
        }
    }
}
