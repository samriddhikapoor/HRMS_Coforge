using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Authorize]
    public class AIChatController : Controller
    {
        private readonly IAIChatService _aiChatService;

        public AIChatController(
            IAIChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        // GET: AIChat
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: AIChat/Ask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ask(
            string category,
            string question)
        {
            if (string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(question))
            {
                TempData["ChatMessage"] =
                    "Please select a valid question.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                var answer =
                    _aiChatService.GetAnswer(
                        category,
                        question);

                ViewBag.Category = category;
                ViewBag.Question = question;
                ViewBag.Answer = answer;

                return View("Index");
            }
            catch (Exception)
            {
                TempData["ChatMessage"] =
                    "Something went wrong while processing your question.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}