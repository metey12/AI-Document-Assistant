<img width="2559" height="1267" alt="Ekran görüntüsü 2026-01-13 205214" src="https://github.com/user-attachments/assets/81ac49f1-1568-40c6-a135-f7c04a1ae9ef" /># 🧠 AI Document Assistant (RAG Workspace)

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-pgvector-336791.svg)
![Gemini AI](https://img.shields.io/badge/AI-Google%20Gemini-orange.svg)

**AI Document Assistant**, kullanıcıların PDF belgeleriyle doğal dilde etkileşime girmesini sağlayan, **RAG (Retrieval-Augmented Generation)** mimarisine sahip modern bir web uygulamasıdır. Google Gemini modellerini ve vektör veritabanlarını kullanarak belgeleri analiz eder, özetler ve soruları yanıtlar.

<img width="2559" height="1267" alt="Ekran görüntüsü 2026-01-13 205214" src="https://github.com/user-attachments/assets/ad68ba92-0f5d-482a-8cc4-cca279dd8e42" />
<img width="2559" height="1267" alt="Ekran görüntüsü 2026-01-13 205006" src="https://github.com/user-attachments/assets/259545ae-e77c-4e28-8671-75d42538c766" />


## 🌟 Temel Özellikler

* **📄 Akıllı Doküman Analizi:** Yüklenen PDF belgeleri otomatik olarak parçalanır (chunking) ve analiz edilir.
* **🤖 RAG Mimarisi:** Sorulan sorulara, belgenin içeriğine dayalı ve bağlamsal cevaplar verilir.
* **📍 Akıllı Atıf Sistemi:** Yapay zeka, verdiği cevabın belgenin **hangi sayfasında** geçtiğini referans gösterir.
* **🎓 Otomatik Sınav (Quiz) Modu:** Yüklenen belgeden otomatik olarak çoktan seçmeli sorular oluşturur ve kullanıcının bilgisini test eder.
* **💾 Kalıcı Hafıza:** PostgreSQL ve **pgvector** eklentisi kullanılarak vektör verileri kalıcı olarak saklanır.
* **🎨 Modern Dark UI:** Göz yormayan, SaaS standartlarında profesyonel karanlık tema arayüzü.

## 🛠️ Teknoloji Yığını (Tech Stack)

* **Backend:** ASP.NET Core 8.0 (MVC)
* **AI Model:** Google Gemini 1.5/2.5 Flash & Text Embedding 004
* **Veritabanı:** PostgreSQL (Docker üzerinde `pgvector/pgvector:pg16` imajı)
* **ORM:** Entity Framework Core
* **PDF İşleme:** UglyToad.PdfPig
* **Frontend:** HTML5, CSS3 (Custom Dark Theme), Bootstrap 5, Vanilla JS

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin.

### Gereksinimler
* .NET 8.0 SDK
* Docker Desktop (Veritabanı için)
* Google AI Studio API Key

### 1. Repoyu Klonlayın
```bash
git clone [https://github.com/metey12/AI-Document-Assistant.git](https://github.com/metey12/AI-Document-Assistant.git)
cd AI-Document-Assistant
```

### 2. Veritabanını Ayağa Kaldırın (Docker)
PostgreSQL ve pgvector eklentisini içeren konteyneri başlatın:
```bash
docker run --name rag_db -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 -d pgvector/pgvector:pg16
```

### 3. API Anahtarını Ayarlayın
`Services/GeminiService.cs` dosyasını açın ve API anahtarınızı girin:
```csharp
private readonly string _apiKey = "BURAYA_GEMINI_API_KEY_GELECEK";
```

### 4. Veritabanını Oluşturun (Migration)
```bash
dotnet ef database update
```

### 5. Projeyi Çalıştırın
```bash
dotnet run
```
Tarayıcınızda `http://localhost:5xxx` adresine gidin.

## 📸 Kullanım Senaryosu

1.  **PDF Yükle:** Sol menüden bir ders notu veya kitap bölümü yükleyin.
2.  **Özet:** Sistem belgeyi analiz edip kısa bir özet çıkaracaktır.
3.  **Sohbet:** "Bu belgedeki ana fikir nedir?" gibi sorular sorun. Sistem sayfa numarasıyla birlikte cevap verecektir.
4.  **Quiz:** "Sınav Modu" butonuna basarak yapay zekanın hazırladığı sorularla kendinizi test edin.

## 🤝 Katkıda Bulunma

Pull request'ler kabul edilir. Büyük değişiklikler için lütfen önce tartışmak amacıyla bir issue açınız.

## 📄 Lisans

[MIT](https://choosealicense.com/licenses/mit/)
