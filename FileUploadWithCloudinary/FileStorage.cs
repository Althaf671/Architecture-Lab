using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace FileUploadWithCloudinary;

// Buat class untuk menampung setting konfigurasi
internal sealed class CloudinarySetting
{
    public string CloudName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;
}

// Buat class untuk setup dan membuat instance Cloudinary 
internal sealed class CloudinarySetup
{
    // Buat sebuah field untuk DI
    private readonly Cloudinary _cloudinary;

    // Buat method untuk setup dan membuat instance 
    public CloudinarySetup(IOptions<CloudinarySetting> options)
    {
        // Buat Account 
        var acc = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
        );

        // Masukan ke DI
        _cloudinary = new Cloudinary(acc);
    }

    // Buat field yang return DI agar dapat diakses dari luar
    internal Cloudinary Instance => _cloudinary;
}

// Buat class untuk menampung semua method yang terkait dengan file upload
internal sealed class CloudinaryService
{
    // Buat field untuk menampung DI
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryService> _logger;
    
    // Lakukan DI
    public CloudinaryService(
        Cloudinary cloudinary,
        ILogger<CloudinaryService> logger)
    {
        _cloudinary = cloudinary;
        _logger = logger;
    }

    // Buat method untuk melakukan upload secara async
    internal async Task<ImageUploadResult> UploadResultAsync(string filename, Stream image)
    {
        _logger.LogInformation("Memulai uploading file...");

        // buat var baru berisi class ImageUploadParams dan masukan params nya
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filename, image),
            Folder = "Testing",
            Transformation = new Transformation().Height(500).Width(500).Crop("fill")
        };

        // Panggil method internal cloudinary dan simpan ke result
        var result = await _cloudinary.UploadAsync(uploadParams);

        _logger.LogInformation("Berhasil uploading. PublicId: {PublicId}, URL: {Url}", 
            result.PublicId, 
            result.SecureUrl);

        // Return result
        return result;
    }
}