using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using ProductMgt.ApplicationService.Commands.Product;
using ProductMgt.ApplicationService.Queries.Product;
using ProductMgtServices.Dtos;
using ProductMgtServices.Helper;

namespace ProductMgtServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private IMapper _mapper;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly string[] _permittedExtensions = { ".png" };
        private readonly long _fileSizeLimit = 2000000;
        private readonly string _targetFilePath = Directory.GetCurrentDirectory();
        private ILogger<ProductController> _logger;
        public ProductController(IMediator mediator, IMapper mapper, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ProductGetAllResponse> AllProducts([FromQuery] ProductQueryParameter productQueryParameter)
        {
            var result = await _mediator.Send(new ProductQuery()
            {
                PageIndex = productQueryParameter.PageIndex,
                RecordsPerPage = productQueryParameter.RecordsPerPage
            });
            var response = new ProductGetAllResponse();
            response.PageIndex = productQueryParameter.PageIndex;
            response.RecordsPerPage = productQueryParameter.RecordsPerPage;
            response.TotalItems = result.TotalItems;
            response.Items = result.Products.Select(_mapper.Map<ProductResponseItem>).ToList();
            return response;
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductAddedDto productDto)
        {
            var productAddedCommandResult =
                await _mediator.Send(new ProductAddedCommand(productDto.Name, productDto.Price));
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ProductUpdatedDto productDto)
        {
            var productUpdatedCommandResult = await _mediator.Send(new ProductUpdatedCommand()
            {
                Id = productDto.Id,
                Price = productDto.Price
            });
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] ProductDeletedDto dto)
        {
            var productDeletedCommandResult = await _mediator.Send(new ProductDeletedCommand()
            {
                Id = dto.Id
            });
            return Ok();
        }

        [HttpPost]
        [Route("Media")]
        public async Task<IActionResult> AddProductMedia()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File",
                    $"The request couldn't be processed (Error 1).");
                // Log error

                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader =
                    ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    // This check assumes that there's a file
                    // present without form data. If form data
                    // is present, this method immediately fails
                    // and returns the model error.
                    if (!MultipartRequestHelper
                        .HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File",
                            $"The request couldn't be processed (Error 2).");
                        // Log error

                        return BadRequest(ModelState);
                    }
                    else
                    {
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                            contentDisposition.FileName.Value);
                        var trustedFileNameForFileStorage = Path.GetRandomFileName();

                        // **WARNING!**
                        // In the following example, the file is saved without
                        // scanning the file's contents. In most production
                        // scenarios, an anti-virus/anti-malware scanner API
                        // is used on the file before making the file available
                        // for download or for use by other systems. 
                        // For more information, see the topic that accompanies 
                        // this sample.

                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState,
                            _permittedExtensions, _fileSizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        using (var targetStream = System.IO.File.Create(
                            Path.Combine(_targetFilePath, trustedFileNameForFileStorage)))
                        {
                            await targetStream.WriteAsync(streamedFileContent);

                            _logger.LogInformation(
                                "Uploaded file '{TrustedFileNameForDisplay}' saved to " +
                                "'{TargetFilePath}' as {TrustedFileNameForFileStorage}",
                                trustedFileNameForDisplay, _targetFilePath,
                                trustedFileNameForFileStorage);
                        }
                    }
                }

                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            return Ok();
        }
    }
}