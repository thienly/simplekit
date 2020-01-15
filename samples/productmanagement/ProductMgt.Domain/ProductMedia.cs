using SimpleKit.Domain.Entities;

namespace ProductMgt.Domain
{
    public class ProductMedia : EntityWithId<long>
    {
        protected ProductMedia()
        {
            
        }
        public ProductMedia(long productId, ProductMediaType productMediaType, string relativePath)
        {
            ProductId = productId;
            MediaType = productMediaType;
            RelativePath = relativePath;
        }

        public long ProductId { get; set; }
        public ProductMediaType MediaType { get; set; }
        public string RelativePath { get; set; }
    }

    public enum ProductMediaType
    {
        Images,
        Documents
    }
}