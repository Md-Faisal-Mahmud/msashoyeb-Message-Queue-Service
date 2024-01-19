using log4net;
using Microsoft.Ajax.Utilities;
using MQS.Application.Services;
using MQS.Infrastructure.Data;
using MQS.Infrastructure.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MQS.Web.Controllers
{
    public class MessageQueueController : Controller
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly ILog _logger = LogManager.GetLogger(typeof(MessageQueueController));
        public MessageQueueController() { }
        public MessageQueueController(IMessageQueueService messageQueueService)
        {
            _messageQueueService = messageQueueService;
        }

        public ActionResult Dashboard()
        {
            try
            {
                ViewBag.LogMessages = LogMessages.GetMessages();
                return View();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return View("Error");
            }
        }

        public ActionResult StartQueueProcessing()
        {
            try
            {
                Task.Run(() => _messageQueueService.ProcessMessages());
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return View("Error");
            }
        }

        public ActionResult GetMessages()
        {
            try
            {
                var logMessages = LogMessages.GetMessages();
                return PartialView("_LogMessagesPartial", logMessages);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return View("Error");
            }
        }

        public ActionResult ClearAllMessages()
        {
            try
            {
                LogMessages.ClearAllMessages();
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return View("Error");
            }
        }
    }
}