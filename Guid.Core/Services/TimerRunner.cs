using Guid.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guid.Core.Services
{
    public class TimerRunner : IRunner
    {
        private Timer timer;
        private const int interval = 5;
        public void Run(TimerCallback callback)
        {
            timer = new Timer(callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(interval));
        }
        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
