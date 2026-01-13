using RagProject.Services;
using Microsoft.EntityFrameworkCore;
using RagProject.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = "Host=localhost;Database=RagDb;Username=postgres;Password=mysecretpassword";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseVector())); // Vektör desteğiyle bağla
// Add services to the container.
builder.Services.AddControllersWithViews();
// Program.cs içine ekle:
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<VectorSearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
