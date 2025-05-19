using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;

namespace UIOMatic.Front.API.Controllers
{
    [ApiController]
    [Route("Object")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            // Validate file type
            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("Only image files are allowed");
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsDir = System.IO.Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";
                var filePath = System.IO.Path.Combine(uploadsDir, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the relative path
                return Ok(new { path = $"/uploads/{fileName}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 