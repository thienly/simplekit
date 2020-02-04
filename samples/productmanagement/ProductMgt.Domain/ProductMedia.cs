using System;
using SimpleKit.Domain.Entities;

namespace ProductMgt.Domain
{
    public class ProductMedia : EntityWithId<long>
    {
        protected ProductMedia()
        {
            
        }
        public ProductMedia(Guid productId, ProductMediaType productMediaType, string relativePath)
        {
            ProductId = productId;
            MediaType = productMediaType;
            RelativePath = relativePath;
        }

        public Guid ProductId { get; set; }
        public ProductMediaType MediaType { get; set; }
        public string RelativePath { get; set; }
    }

    public enum ProductMediaType
    {
        Images,
        Documents
    }
}