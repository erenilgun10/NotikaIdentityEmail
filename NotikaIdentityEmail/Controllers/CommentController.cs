using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Enum;
using NotikaIdentityEmail.Models.CommentViewModels;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Services;

namespace NotikaIdentityEmail.Controllers
{
    public class CommentController(EmailContext context, UserManager<AppUser> userManager) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> UserComment(int page = 1)
        {
            const int pageSize = 6;
            page = page < 1 ? 1 : page;

            var commentQuery = context.Comments.Include(x => x.AppUser).OrderByDescending(x => x.CommentDate);
            var totalCount = await commentQuery.CountAsync();
            var comments = await commentQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var viewModel = new CommentViewModel
            {
                Comments = comments,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UserCommentList(int pageNumber = 1, int pageSize = 6)
        {
            var query = context.Comments
                               .Include(c => c.AppUser)
                               .OrderByDescending(c => c.CommentDate);

            var totalCount = await query.CountAsync();

            var comments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new CommentViewModel
            {
                Comments = comments,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(model);
        }

        [HttpGet]
        public PartialViewResult CreateComment()
        {
            return PartialView();

        }


        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentViewModel model)
        {
            if ( model.Comment is not null)
            {
                var userName = User?.Identity?.Name;
                if (string.IsNullOrEmpty(userName))
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return NotFound();
                }

                var comment = new Comment
                {
                    AppUserId = user.Id,
                    CommentDetail = model.Comment.CommentDetail,
                    CommentDate = DateTime.UtcNow,
                    CommentStatus = StatusEnum.passive,
                };
                if (comment is { AppUserId: not null })
                {
                    context.Comments.Add(comment);
                    await context.SaveChangesAsync();
                }

                return RedirectToAction("UserComment");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment is not null)
            {
                context.Comments.Remove(comment);
            }
            return RedirectToAction("UserComment");



        }


        [HttpPost]
        public async Task<IActionResult> UpdateCommentStatus(int commentId, StatusEnum status, int pageNumber = 1)
        {
            var comment = await context.Comments.FindAsync(commentId);
            if (comment is null)
                return RedirectToAction("UserCommentList");

            comment.CommentStatus = status;
            await context.SaveChangesAsync();

            return RedirectToAction("UserCommentList");
        }




    }
}
