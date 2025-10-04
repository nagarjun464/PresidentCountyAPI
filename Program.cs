using Google.Cloud.Firestore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PresidentCounty API",
        Version = "v1",
        Description = "CRUD API for managing President County candidates in Firestore (GCP)."
    });
});

// ---------- Firestore Setup ----------
var projectId = builder.Configuration["GoogleCloud:ProjectId"]
    ?? throw new InvalidOperationException("Missing GoogleCloud:ProjectId in appsettings.json");

var credentialsPath = builder.Configuration["GoogleCloud:CredentialsPath"];
if (!string.IsNullOrEmpty(credentialsPath))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
}

builder.Services.AddSingleton(_ => FirestoreDb.Create(projectId));

// ---------- Add Controllers ----------
builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // allow all origins (for dev; restrict later if needed)
              .AllowAnyMethod()      // GET, POST, PUT, DELETE
              .AllowAnyHeader();     // all headers
    });
});


var app = builder.Build();
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// ---------- Middleware ----------
if (app.Environment.IsDevelopment() || true)  // allow in production too
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PresidentCounty API v1");
        c.RoutePrefix = string.Empty; // ? Serve Swagger at "/"
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();


app.MapControllers();

// Health Check
app.MapGet("/healthz", () => Results.Ok("API is running"));

app.Run();
