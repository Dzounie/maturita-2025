using Microsoft.AspNetCore.Authorization; //token using
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController(IWebHostEnvironment env) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("list")]
        public ActionResult<List<string>> GetImagePaths()
        {
            var path = Path.Combine(env.WebRootPath, "nahledovky");
            if (!Directory.Exists(path))
                return NotFound("Složka neexistuje");

            var files = Directory.GetFiles(path)
                .Select(f => "nahledovky/" + Path.GetFileName(f))
                .ToList();

            return Ok(files);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // max 10 MB
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Soubor je prázdný");

            var folderPath = Path.Combine(env.WebRootPath, "nahledovky");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, file.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Ok($"nahledovky/{file.FileName}");
        }

    }
}