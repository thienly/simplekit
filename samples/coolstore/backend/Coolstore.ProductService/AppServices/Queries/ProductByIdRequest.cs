using System;
using Coolstore.ProductService.Domains;
using MediatR;

namespace Coolstore.ProductService.AppServices.Queries
{
    public class ProductByIdRequest
    {
        public Guid Id { get; set; }
        
    }

    public class ProductByIdResponse : IRequest<ProductByIdRequest>
    {
        public Product Type { get; set; }
    }
}