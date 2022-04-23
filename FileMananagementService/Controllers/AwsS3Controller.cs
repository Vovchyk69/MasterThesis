using Microsoft.AspNetCore.Mvc;
using Shared.AmazonS3;
using Shared.RabbitMq.EventBus.Bus;
using Shared.RabbitMq.Events;

namespace FileMananagementService.Controllers;

[ApiController]
[Route("documents")]
[Produces("application/json")]
public class AwsS3Controller : ControllerBase
{
    private readonly IEventBus _eventBus;
    private readonly IAwsConfiguration _configuration;
    private readonly IAwsStorage _awsStorage;

    public AwsS3Controller(IAwsConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _awsStorage = new AwsStorage(
            _configuration.AwsAccessKey, 
            _configuration.AwsSecretAccessKey, 
            _configuration.Region,
            _configuration.BucketName);

        _eventBus = eventBus;
    }
    
    [HttpGet("{documentName}")]
    public async Task<ActionResult<FileContentResult>> GetFileFromS3(string documentName)
    {
        if (string.IsNullOrEmpty(documentName))
            return BadRequest("File name is required ...");
        
        var document = await _awsStorage.DownloadFileAsync(documentName);

        var result = File(document, "application/octet-stream", documentName);

        return Ok(result);

    }
    
    [HttpPost]
    public async Task<ActionResult<bool>> UploadDocumentToS3(IFormFile file)
    {
        if (file is null || file.Length <= 0)
            return BadRequest("File is required for upload... Input not valid");
        
        var result = await _awsStorage.UploadFileAsync(file);

        if (result) _eventBus.Publish(new FileUploadMessage(file.FileName));

        return Ok(result);
    }
    
    [HttpDelete("{documentName}")]
    public async Task <ActionResult> DeleteDocumentFromS3(string documentName)
    {
        if (string.IsNullOrEmpty(documentName))
            return BadRequest("File name is required ...");

        await _awsStorage.DeleteFileAsync(documentName);

        return Ok();

    }
}