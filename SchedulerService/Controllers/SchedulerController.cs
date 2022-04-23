using Microsoft.AspNetCore.Mvc;

namespace SchedulerService.Controllers;

[ApiController]
[Route("scheduler")]
public class SchedulerController : ControllerBase
{
    [HttpPost]
    public ActionResult UploadDocumentToS3(IFormFile file)
    {
        if (file is null || file.Length <= 0)
            return BadRequest("File is required for upload... Input not valid");
        
        return Ok();
    }
}