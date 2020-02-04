using System;
using MediatR;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductUpdatedCommand: IRequest<ProductUpdatedCommandResult>
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
    }
}