using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using RagProject.Data;
using RagProject.Models;

namespace RagProject.Services
{
    public class VectorSearchService
    {
        private readonly AppDbContext _context;

        public VectorSearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentChunk?> FindMostSimilarAsync(float[] queryVector)
        {
            var vector = new Pgvector.Vector(queryVector);

            var bestMatch = await _context.DocumentChunks
                .OrderBy(x => x.Embedding!.CosineDistance(vector))
                .FirstOrDefaultAsync();

            return bestMatch;
        }
    }
}