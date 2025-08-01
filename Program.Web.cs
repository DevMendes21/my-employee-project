using MyEmployeeProject.DAO;
using MyEmployeeProject.Conexao;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Configurar string de conexão do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? BuildConnectionString();

ConexaoUniversal.SetConnectionString(connectionString);

// Configure Data Protection for container environments
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/data-protection-keys"))
    .SetApplicationName("MinhaEmpresa");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FuncionarioDAO>();
builder.Services.AddScoped<CargoDAO>();
builder.Services.AddScoped<DepartamentoDAO>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Ensure data protection keys directory exists
var keysDir = new DirectoryInfo("/app/data-protection-keys");
if (!keysDir.Exists)
{
    keysDir.Create();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Only use HTTPS redirection if not in container or if HTTPS is configured
if (!app.Environment.IsDevelopment() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")))
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static string BuildConnectionString()
{
    var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
    var user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
    var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "EmpresaDB";
    var sslMode = Environment.GetEnvironmentVariable("DB_SSL_MODE") ?? "None";
    
    return $"server={host};port={port};user={user};password={password};database={database};sslmode={sslMode};";
}
