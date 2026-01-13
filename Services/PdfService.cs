using UglyToad.PdfPig;

namespace RagProject.Services
{
    public record PdfPageContent(int PageNumber, string Text);

    public class PdfService
    {
        public List<PdfPageContent> GetTextChunksFromPdf(Stream pdfStream)
        {
            var chunks = new List<PdfPageContent>();
            
            using (var pdf = PdfDocument.Open(pdfStream))
            {
                foreach (var page in pdf.GetPages())
                {
                    var text = page.Text;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        chunks.Add(new PdfPageContent(page.Number, text));
                    }
                }
            }
            return chunks;
        }
    }
}