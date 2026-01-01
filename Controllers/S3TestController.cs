using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Services.Implementations;

public class S3TestController : Controller
{
    private readonly S3Service _s3Service;

    public S3TestController(S3Service s3Service)
    {
        _s3Service = s3Service;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Content("No file selected");

        var url = await _s3Service.UploadAsync(file, "test");

        return Content($"Upload success ✅<br/>URL: {url}");
    }
}