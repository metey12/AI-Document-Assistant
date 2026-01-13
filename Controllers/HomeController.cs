using Microsoft.AspNetCore.Mvc;
using RagProject.Models;
using RagProject.Services;
using RagProject.Data;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace RagProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly PdfService _pdfService;
        private readonly GeminiService _geminiService;
        private readonly VectorSearchService _vectorSearchService;
        private readonly AppDbContext _context;

        public HomeController(PdfService pdfService, GeminiService geminiService, VectorSearchService vectorSearchService, AppDbContext context)
        {
            _pdfService = pdfService;
            _geminiService = geminiService;
            _vectorSearchService = vectorSearchService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Lütfen bir dosya seçin." });

            try
            {
                var allChunks = _context.DocumentChunks.ToList();
                _context.DocumentChunks.RemoveRange(allChunks);
                await _context.SaveChangesAsync();

                using var stream = file.OpenReadStream();
                var pdfPages = _pdfService.GetTextChunksFromPdf(stream);

                foreach (var page in pdfPages)
                {
                    if (page.Text.Length < 50) continue;

                    var embeddingArray = await _geminiService.GetEmbeddingAsync(page.Text);

                    var chunk = new DocumentChunk
                    {
                        FileName = file.FileName,
                        PageNumber = page.PageNumber,
                        Text = page.Text,
                        Embedding = new Vector(embeddingArray)
                    };

                    _context.DocumentChunks.Add(chunk);
                }

                await _context.SaveChangesAsync();

                string fullText = string.Join(" ", pdfPages.Take(5).Select(p => p.Text));
                string summaryHtml = await _geminiService.GenerateSummaryAsync(fullText);

                return Json(new
                {
                    success = true,
                    message = $"{pdfPages.Count} sayfa analiz edildi ve veritabanına kaydedildi.",
                    summary = summaryHtml
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion(string question)
        {
            if (string.IsNullOrEmpty(question))
                return Json(new { answer = "Lütfen bir soru yazın." });

            try
            {
                var questionEmbedding = await _geminiService.GetEmbeddingAsync(question);

                var bestMatch = await _vectorSearchService.FindMostSimilarAsync(questionEmbedding);

                if (bestMatch == null)
                    return Json(new { answer = "Veritabanında eşleşen bir bilgi bulunamadı. Lütfen önce PDF yükleyin." });

                string contextWithPage = $"[Sayfa {bestMatch.PageNumber}]: {bestMatch.Text}";

                var answer = await _geminiService.GenerateAnswerAsync(question, contextWithPage);

                return Json(new
                {
                    answer = answer,
                    context = bestMatch.Text,
                    page = bestMatch.PageNumber
                });
            }
            catch (Exception ex)
            {
                return Json(new { answer = "Sistem hatası: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearDatabase()
        {
            var all = _context.DocumentChunks.ToList();
            _context.DocumentChunks.RemoveRange(all);
            await _context.SaveChangesAsync();
            return Json(new { message = "Veritabanı temizlendi." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz()
        {
            try
            {
                var chunks = await _context.DocumentChunks.Take(10).ToListAsync();

                if (chunks.Count == 0)
                    return Json(new { success = false, message = "Önce bir PDF yüklemelisiniz." });

                string fullText = string.Join(" ", chunks.Select(c => c.Text));

                string quizJson = await _geminiService.GenerateQuizAsync(fullText);

                return Json(new { success = true, quizData = quizJson });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }
    }
}