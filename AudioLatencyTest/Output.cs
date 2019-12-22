using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioLatencyTest
{
    static class Output
    {
        public static event Action<string> OnLogging;
        public static void WriteLine(object line = null)
        {
            OnLogging?.Invoke(line?.ToString());
            Console.WriteLine(line);
        }
    }
}
