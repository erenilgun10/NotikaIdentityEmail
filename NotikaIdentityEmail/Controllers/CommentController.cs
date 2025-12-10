using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.CommentViewModels;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Services;

namespace NotikaIdentityEmail.Controllers
{
    public class CommentController(EmailContext context) : Controller
    {

        [HttpGet]
        public IActionResult UserComment()
        {

            var comments = context.Comments.Include(x=>x.AppUser).ToList();

            return View(comments);
        }

        [HttpGet]
        public IActionResult UserCommentList()
        {
            var userList = context.Comments.Include(x => x.AppUser).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            
            if (ModelState.IsValid && model.Comment is not null)
            {
                
                context.Comments.Add(model.Comment);
                await context.SaveChangesAsync();
                return RedirectToAction("UserComment");
            }
            return View(model);


        }



    }
}
