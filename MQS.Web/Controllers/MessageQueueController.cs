using Microsoft.Ajax.Utilities;
using MQS.Application.Services;
using MQS.Infrastructure.Data;
using MQS.Infrastructure.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MQS.Web.Controllers
{
    public class MessageQueueController : Controller
    {
        MessageQueueService _messageQueueService = new MessageQueueService();
        //    private readonly IMessageQueueService _messageQueueService;
        //    public MessageQueueController() { }
        //    public MessageQueueController(IMessageQueueService messageQueueService)
        //    {
        //        _messageQueueService = messageQueueService;
        //    }

        public ActionResult Dashboard()
        {
            ViewBag.LogMessages = LogMessages.GetMessages();
            return View();
        }

        public ActionResult StartQueueProcessing()
        {
            Task.Run(() => _messageQueueService.ProcessMessages());
            return RedirectToAction("Dashboard");
        }

        public ActionResult GetMessages()
        {
            var logMessages = LogMessages.GetMessages();
            return PartialView("_LogMessagesPartial", logMessages);
        }

        public ActionResult ClearAllMessages()
        {
            LogMessages.ClearAllMessages();
            return RedirectToAction("Dashboard");
        }
    }
}