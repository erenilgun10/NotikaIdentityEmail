using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController(EmailContext context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> CategoryList()
        {
            var token = Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["error"] = "Giriş yapmalısınız.";
                return RedirectToAction("UserLogin", "Login");
            }
            var categories = await context.Categories.OrderByDescending(c => c.CategoryId).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (category is not null)
            {
                category.CategoryStatus = category.CategoryStatus;

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "Kategori başarıyla oluşturuldu!";
                return RedirectToAction("CategoryList");
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Geçersiz kategori ID!";
                return RedirectToAction("CategoryList");
            }

            var category = await context.Categories.FindAsync(id);
            if (category is null)
            {
                TempData["Error"] = "Kategori bulunamadı!";
                return RedirectToAction("CategoryList");
            }

            var hasMessages = await context.Messages.AnyAsync(m => m.CategoryId == id);
            if (hasMessages)
            {
                TempData["Error"] = "Bu kategori kullanımda olduğu için silinemez!";
                return RedirectToAction("CategoryList");
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            TempData["Success"] = "Kategori başarıyla silindi!";
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Geçersiz kategori ID!";
                return RedirectToAction("CategoryList");
            }

            var category = await context.Categories.FindAsync(id);
            if (category is null)
            {
                TempData["Error"] = "Kategori bulunamadı!";
                return RedirectToAction("CategoryList");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await context.Categories.FindAsync(model.CategoryId);
            if (category is null)
            {
                TempData["Error"] = "Kategori bulunamadı!";
                return RedirectToAction("CategoryList");
            }

            category.CategoryName = model.CategoryName;
            category.CategoryIconUrl = model.CategoryIconUrl;
            category.CategoryLabelFormat = model.CategoryLabelFormat;
            category.CategoryStatus = model.CategoryStatus;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            TempData["Success"] = "Kategori başarıyla güncellendi!";
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCategoryStatus(int categoryId)
        {
            var category = await context.Categories.FindAsync(categoryId);
            if (category is null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            category.CategoryStatus = !category.CategoryStatus;
            await context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Kategori durumu güncellendi!",
                isActive = category.CategoryStatus
            });
        }
    }
}