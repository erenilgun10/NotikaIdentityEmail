using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
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
using System.Text.Json;

namespace NotikaIdentityEmail.Controllers
{
    public class CommentController(EmailContext context, UserManager<AppUser> userManager, IConfiguration config) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> UserComment(int page = 1)
        {
            const int pageSize = 6;
            page = page < 1 ? 1 : page;

            var commentQuery = context.Comments.Include(x => x.AppUser).Where(x => x.CommentStatus == StatusEnum.active).OrderByDescending(x => x.CommentDate);
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
        [Authorize(Roles ="Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment is not null)
            {
                context.Comments.Remove(comment);
                await context.SaveChangesAsync();

            }
            return RedirectToAction("UserCommentList");
        }



        [Authorize(Roles = "Admin")]
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




        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentViewModel model)
        {
            if (model.Comment is not null)
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
                };

                using var client = new HttpClient();
                var apiKey = "";
                var apiKey = config["HuggingFace:ApiKey"];
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                try
                {
                    var translateRBody = new
                    {
                        inputs = comment.CommentDetail,
                    };
                    var translateJson = JsonSerializer.Serialize(translateRBody);
                    var translateContent = new StringContent(translateJson, System.Text.Encoding.UTF8, "application/json");
                    var translateResponse = await client.PostAsync(config["HuggingFace:TranslateUrl"], translateContent);
                    var translateResponseString = await translateResponse.Content.ReadAsStringAsync();
                    string englishText = comment.CommentDetail ?? "";
                    if (translateResponseString.TrimStart().StartsWith("["))
                    {
                        var translateDoc = JsonDocument.Parse(translateResponseString);
                        englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString() ?? comment.CommentDetail ?? "";
                    }

                    var toxicRequestBody = new
                    {
                        inputs = englishText
                    };

                    var toxicJson = JsonSerializer.Serialize(toxicRequestBody);

                    var toxicContent = new StringContent(toxicJson, System.Text.Encoding.UTF8, "application/json");
                    var toxicResponse = await client.PostAsync(config["HuggingFace:ToxicUrl"], toxicContent);
                    var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();
                    if (toxicResponseString.TrimStart().StartsWith("["))
                    {
                        var toxicDoc = JsonDocument.Parse(toxicResponseString);
                        foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
                        {
                            string label = item.GetProperty("label").GetString() ?? "";
                            double score = item.GetProperty("score").GetDouble();
                            if (label == "toxic" && score >= 0.5)
                            {
                                comment.CommentStatus = StatusEnum.Toxic;
                            }
                        }
                    }
                    else
                    {
                        comment.CommentStatus = StatusEnum.Waiting;
                    }
                }
                catch
                {

                }

                if (comment is { AppUserId: not null })
                {
                    context.Comments.Add(comment);
                    await context.SaveChangesAsync();
                }

                return RedirectToAction("UserComment");
            }
            return View(model);

        }







    }
}
