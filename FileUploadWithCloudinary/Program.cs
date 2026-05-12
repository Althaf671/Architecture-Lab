using FileUploadWithCloudinary;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// Cloudinary Setup and Service Registry
builder.Services.Configure<CloudinarySetting>(
    builder.Configuration.GetSection("Cloudinary"));

// Buat DI lifetime satu kali saat app pertama jalan
builder.Services.AddSingleton<CloudinarySetup>();

builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<CloudinarySetup>().Instance);

// Registry Cloudinary
builder.Services.AddScoped<CloudinaryService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// API endpoint
app.MapPost("/upload", async (IFormFile file, CloudinaryService storage) =>
{
    using var stream = file.OpenReadStream();   
    var result = await storage.UploadResultAsync(file.FileName, stream);

    if (result.Error != null)
    {
        return Results.BadRequest(result.Error.Message);
    }

    return Results.Ok(new{ Url = result.SecureUrl });
})
.DisableAntiforgery();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();


app.Run();

