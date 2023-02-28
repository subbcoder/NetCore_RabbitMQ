using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guid.Core.Interfaces
{
    public interface IWorker
    {
        void Run();
    }
    public interface IRunner : IDisposable
    {
        void Run(TimerCallback waitCallback);
    }
}
