using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductMgt.ApplicationService.Commands.Product;
using ProductMgt.ApplicationService.Queries.Product;
using ProductMgtServices.Dtos;

namespace ProductMgtServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductDto>> AllProducts([FromQuery]ProductQueryParameter productQueryParameter)
        {
            var result = await _mediator.Send(new ProductQuery()
            {
                PageIndex = productQueryParameter.PageIndex,
                RecordsPerPage = productQueryParameter.RecordsPerPage
            });
            var productDtos = result.Products.Select(x => new ProductDto()
            {
                Name = x.Name,
                Price = x.Price
            });
            return productDtos;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]ProductDto productDto)
        {
            var productAddedCommandResult =
                await _mediator.Send(new ProductAddedCommand(productDto.Name, productDto.Price));
            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody]ProductDto productDto)
        {
            var productUpdatedCommandResult = await _mediator.Send(new ProductUpdatedCommand()
            {
                Id = productDto.Id,
                Price = productDto.Price
            });
            return Ok();
        }

        [HttpDelete]
        public Task Delete(long productId)
        {
            return Task.CompletedTask;
        }
    }
}