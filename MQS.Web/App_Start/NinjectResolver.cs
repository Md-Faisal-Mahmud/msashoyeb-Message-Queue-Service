using MQS.Application.Services;
using MQS.Application.Utilities;
using MQS.Infrastructure.Services;
using MQS.Infrastructure.Utilities;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MQS.Web.App_Start
{
    public class NinjectResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectResolver()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            this._kernel.Bind<IMessageQueueService>().To<MessageQueueService>();
            this._kernel.Bind<IMessageQueueUtility>().To<MessageQueueUtility>();
        }
    }
}