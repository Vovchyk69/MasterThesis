using Microsoft.AspNetCore.Mvc;
using SchedulerService.DataParsing;
using SchedulerService.GeneticAlgorithm;
using SchedulerService.PocoObjects;
using Shared.AmazonS3;

namespace SchedulerService.Controllers;

[ApiController]
[Route("scheduler")]
public class SchedulerController: ControllerBase
{
    private readonly IAwsConfiguration _configuration;
    private readonly IAwsStorage _awsStorage;
    private readonly IFile _fileParser;

    public SchedulerController(
        IAwsConfiguration configuration,
        IFile parser)
    {
        _configuration = configuration;
        _awsStorage = new AwsStorage(
            _configuration.AwsAccessKey, 
            _configuration.AwsSecretAccessKey, 
            _configuration.Region,
            _configuration.BucketName);
        
        _fileParser = parser;
    }
    
    [HttpPost]
    public async Task<ActionResult> Execute([FromBody]Payload payload)
    {
        var file = await _awsStorage.DownloadFileAsync(payload.FileName);
        var parsedFile = _fileParser.Parse(file);

        var alg = new GeneticAlgorithm<Schedule>(new Schedule(parsedFile));
        alg.Run();
        
        return Ok((ResponsePayload)alg.Result);
    }
}