using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController(EmailContext context) : Controller
    {
        private readonly EmailContext _context = context;


        public IActionResult Inbox()
        {
            var resp = _context.Messages.Where(x => x.ReceiverEmail == "49@email.com").ToList();
            return View(resp);
        }

        public IActionResult Sentbox()
        {
            var resp = _context.Messages.Where(x => x.SenderEmail == "49@email.com").ToList();
            return View(resp);
        }

        public IActionResult MessageDetail()
        {
            var resp = _context.Messages.Where(x => x.MessageId == 1).FirstOrDefault();
            return View(resp);
        }

        //Yeni Mesaj Gönderme Ekranı
        [HttpGet]
        public IActionResult NewMessage()
        {
            return View();
        }

        //Yeni Mesaj Gönderme İşlemi
        [HttpPost]
        public IActionResult CreateMessage()
        {
            return View();
        }




    }
}
