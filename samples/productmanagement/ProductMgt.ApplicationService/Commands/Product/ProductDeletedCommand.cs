using System;
using MediatR;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductDeletedCommand: IRequest<ProductDeletedCommandResult>
    {
        public Guid Id { get; set; }
    }
}