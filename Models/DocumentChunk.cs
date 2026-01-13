using Pgvector; // Vektör tipi için
using System.ComponentModel.DataAnnotations.Schema;

namespace RagProject.Models
{
    public class DocumentChunk
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Text { get; set; }
        public int PageNumber { get; set; }

        [Column(TypeName = "vector(768)")] // Gemini embedding boyutu 768'dir
        public Vector Embedding { get; set; }
    }
}