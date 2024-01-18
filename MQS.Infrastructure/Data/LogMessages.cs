using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQS.Infrastructure.Data
{
    public static class LogMessages
    {
        private static readonly List<string> messages = new List<string>();
        private static readonly object lockObject = new object();

        public static void AddMessage(string message)
        {
            lock (lockObject)
            {
                messages.Add(message);
            }
        }

        public static IEnumerable<string> GetMessages()
        {
            lock (lockObject)
            {
                return messages.ToList();
            }
        }
    }
}
