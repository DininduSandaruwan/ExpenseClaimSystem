using Microsoft.AspNetCore.Mvc;

namespace ExpenseClaimSystem.BlazorServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public AttachmentsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        [RequestSizeLimit(10_000_000)] // 10MB limit
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Create uploads folder inside wwwroot
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Prevent filename conflicts
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{uniqueFileName}";

                return Ok(new UploadResult
                {
                    FileName = uniqueFileName,
                    FilePath = relativePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"File upload failed: {ex.Message}");
            }
        }
    }

    public class UploadResult
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
