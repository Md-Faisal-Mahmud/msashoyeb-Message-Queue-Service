using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQS.Application.Services
{
    public interface IMessageQueueService
    {
        void ProcessMessages();
    }
}
