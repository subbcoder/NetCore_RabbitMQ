using Guid.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guid.Core.Services
{
    public class TimedMessageSender : IWorker, IDisposable
    {
        private readonly ISender sender;
        private readonly IRunner runner;
        private int counter = 0;
        public TimedMessageSender(IRunner runner, ISender sender)
        {
            this.sender = sender;
            this.runner = runner;
        }

        public void Run()
        {
            runner.Run(DoSelectedWork);
            Console.ReadLine();
        }

        /// 
        /// Initialize specific senders
        /// 
        /// 
        private void DoSelectedWork(object state)
        {

            Parallel.Invoke(
              () => sender.Send("royland", "Arcanys", counter + " " + System.Guid.NewGuid().ToString()),
              () => sender.Send("allan", "Arcanys", counter + " " + System.Guid.NewGuid().ToString()),
              () => sender.Send("rafael", "Arcanys", counter + " " + System.Guid.NewGuid().ToString()),
              () => sender.Send("fred", "Arcanys", counter + " " + System.Guid.NewGuid().ToString()),
              () => sender.Send("hans", "Arcanys", counter + " " + System.Guid.NewGuid().ToString())
              );
            counter++;
        }

        public void Dispose()
        {
            runner?.Dispose();
        }

    }
}
