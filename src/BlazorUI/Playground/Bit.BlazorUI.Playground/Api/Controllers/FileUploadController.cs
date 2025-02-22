﻿using Microsoft.AspNetCore.Mvc;

namespace Bit.BlazorUI.Playground.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class FileUploadController : ControllerBase
{
    private readonly string BasePath;

    public FileUploadController(IConfiguration Configuration)
    {
        BasePath = Configuration["UploadPath"];
    }

    [RequestSizeLimit(11 * 1024 * 1024 /*11 MB*/)]
    [HttpPost]
    public async Task<IActionResult> UploadStreamedFile(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null)
        {
            ModelState.AddModelError("File", $"The request couldn't be processed (Error 1).");
            return BadRequest(ModelState);
        }

        using var requestStream = file.OpenReadStream();

        if (Directory.Exists(BasePath) is false)
        {
            Directory.CreateDirectory(BasePath);
        }

        var path = Path.Combine(BasePath, file.FileName);

        if (System.IO.File.Exists(path) is false)
        {
            using var targetStream = System.IO.File.Create(path);
            if (cancellationToken.IsCancellationRequested is false)
                await requestStream.CopyToAsync(targetStream, cancellationToken);
        }
        else
        {
            using var targetStream = System.IO.File.Open(path, FileMode.Append);
            if (cancellationToken.IsCancellationRequested is false)
                await requestStream.CopyToAsync(targetStream, cancellationToken);
        }

        return Ok();
    }

    [HttpDelete]
    public IActionResult RemoveFile(string fileName)
    {
        var path = Path.Combine(BasePath, fileName);
        if (!System.IO.File.Exists(path)) return NotFound();

        System.IO.File.Delete(path);
        return Ok();
    }
}
