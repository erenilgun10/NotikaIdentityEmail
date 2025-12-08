using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NotikaIdentityEmail.Models.MessageViewModels;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize]
    public class MessageController(EmailContext context, UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Inbox(int? categoryId)
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            var resp = (from msg in context.Messages
                        join ctg in context.Categories on msg.CategoryId equals ctg.CategoryId
                        join usr in context.Users on msg.SenderEmail equals usr.Email into SenderGroup
                        from usr in SenderGroup.DefaultIfEmpty()
                        where msg.ReceiverEmail == user.Email
                        select new MessageWithSenderInfoViewModel
                        {
                            MessageId = msg.MessageId,
                            CategoryId = ctg.CategoryId,
                            Subject = msg.Subject,
                            MessageDetail = msg.MessageDetail,
                            SenderEmail = msg.SenderEmail,
                            SendDate = msg.SendDate,
                            SenderFirstName = usr.FirstName ?? "Kullanıcı Bilinmiyor",
                            SenderLastName = usr.LastName ?? "",
                            CategoryName = ctg.CategoryName ?? "Kategori Yok",
                            CategoryLabelFormat = ctg.CategoryLabelFormat,
                        }).ToList();

            if (categoryId != null)
            {
                resp = [.. resp.Where(x => x.CategoryId == categoryId)];
            }

            return View(resp);
        }

        public async Task<IActionResult> Sentbox()
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            //var resp = context.Messages.Where(x => x.ReceiverEmail == user.Email).ToList();

            var resp = (from msg in context.Messages
                        join ctg in context.Categories on msg.CategoryId equals ctg.CategoryId
                        join usr in context.Users on msg.SenderEmail equals usr.Email into ReceiverGroup
                        from usr in ReceiverGroup.DefaultIfEmpty()
                        where msg.SenderEmail == user.Email

                        select new MessageWithReceiverInfoViewModel
                        {
                            MessageId = msg.MessageId,
                            Subject = msg.Subject,
                            MessageDetail = msg.MessageDetail,
                            ReceiverEmail = msg.SenderEmail,
                            SendDate = msg.SendDate,
                            ReceiverFirstName = usr.FirstName ?? "Kullanıcı Bilinmiyor",
                            ReceiverLastName = usr.LastName ?? "",
                            CategoryName = ctg.CategoryName ?? "Kategori Yok",
                            CategoryLabelFormat = ctg.CategoryLabelFormat
                        }).ToList();
            return View(resp);
        }

        public IActionResult MessageDetail(int id)
        {
            var resp = context.Messages.Where(x => x.MessageId == id).FirstOrDefault();
            if (resp == null) return NotFound();
            return View(resp);
        }

        //Yeni Mesaj Gönderme Ekranı
        [HttpGet]
        public IActionResult NewMessage()
        {
            var categories = context.Categories.ToList();
            ViewBag.v = categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString()
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewMessage(Message message)
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return NotFound();
            }

            message.SendDate = DateTime.Now;
            message.IsRead = false;
            message.SenderEmail = user.Email!;
            context.Messages.Add(message);
            context.SaveChanges();
            return RedirectToAction("Sentbox");
        }





    }
}
