using Guid.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guid.Core.Services
{
    public class MessageReceiver : IWorker
    {
        private readonly IReceiver receiver;
        public MessageReceiver(IReceiver receiver)
        {
            this.receiver = receiver;
        }
        public void Run()
        {
            receiver.Receive("Arcanys");
        }
    }
}
