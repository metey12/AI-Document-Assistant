using Newtonsoft.Json;
using System.Text;

namespace RagProject.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "gemini api key";

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent?key={_apiKey}";

            var payload = new
            {
                model = "models/text-embedding-004",
                content = new { parts = new[] { new { text = text } } }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, jsonContent);

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);

            if (result.error != null)
            {
                string msg = result.error.message;
                throw new Exception($"Gemini Embedding Hatası: {msg}");
            }

            if (result.embedding == null)
            {
                throw new Exception("Gemini embedding verisi döndürmedi (null).");
            }

            var values = result.embedding.values.ToObject<float[]>();
            return values;
        }

        public async Task<string> GenerateAnswerAsync(string question, string context)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
            var prompt = $"Aşağıdaki bilgilere dayanarak soruyu cevapla.\n\nBilgi: {context}\n\nSoru: {question}";

            var payload = new
            {
                contents = new[]
                {
            new { parts = new[] { new { text = prompt } } }
        }
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, jsonContent);

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);


            if (result.error != null)
            {
                return $"API Hatası: {result.error.message}";
            }

            if (result.candidates == null || result.candidates.Count == 0)
            {
                return "Gemini cevap üretemedi (Aday cevap yok). Güvenlik filtresine takılmış olabilir.";
            }

            if (result.candidates[0].content == null)
            {
                return "Model içerik döndürmedi (Content null).";
            }


            return result.candidates[0].content.parts[0].text;
        }

        public async Task<string> GenerateSummaryAsync(string text)
        {
            // Metin çok uzunsa başından 10.000 karakteri alalım (Token tasarrufu ve hız için)
            string safeText = text.Length > 10000 ? text.Substring(0, 10000) : text;

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var prompt = $@"
    Aşağıdaki metni analiz et ve şu formatta HTML olarak çıktı ver (sadece div içeriği ver, html/body etiketleri olmasın):
    
    <div class='alert alert-info'>
      <h5 class='alert-heading'>📄 Belge Analizi</h5>
      <p><strong>Belge Türü:</strong> [Örn: Akademik Makale, Fatura, Roman, Sözleşme vb.]</p>
      <p><strong>Özet:</strong> [Metnin 2-3 cümlelik kısa özeti]</p>
      <hr>
      <h6>Ana Konu Başlıkları:</h6>
      <ul>
        <li>[Madde 1]</li>
        <li>[Madde 2]</li>
        <li>[Madde 3]</li>
      </ul>
    </div>

    İncelenecek Metin:
    {safeText}";

            var payload = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } }
            };

            try
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                return result.candidates[0].content.parts[0].text;
            }
            catch
            {
                return "<div class='alert alert-warning'>Özet çıkarılamadı.</div>";
            }
        }

        public async Task<string> GenerateQuizAsync(string context)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            // Prompt çok önemli: Kesinlikle JSON dönmeye zorluyoruz.
            var prompt = $@"
    Aşağıdaki metne dayanarak 5 adet çoktan seçmeli soru hazırla.
    Çıktı SADECE geçerli bir JSON formatında olmalı, markdown ('```json') kullanma.
    
    JSON Formatı şu şekilde olmalı:
    [
      {{
        ""question"": ""Soru metni buraya"",
        ""options"": [""A şıkkı"", ""B şıkkı"", ""C şıkkı"", ""D şıkkı""],
        ""correctAnswer"": 0 (Doğru şıkkın 0 tabanlı indexi: 0=A, 1=B gibi)
      }}
    ]

    Kaynak Metin:
    {context.Substring(0, Math.Min(context.Length, 15000))} 
    ";

            var payload = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } }
            };

            try
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string rawText = result.candidates[0].content.parts[0].text;

                // Bazen Gemini ```json ile sarmalar, onu temizleyelim
                rawText = rawText.Replace("```json", "").Replace("```", "").Trim();

                return rawText;
            }
            catch (Exception ex)
            {
                return "[]";
            }
        }
    }
}