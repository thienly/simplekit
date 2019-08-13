using System;

namespace SimpleKit.Domain
{
    public interface IAuditable
    {
        DateTime CreatedDate { get; set; }
        int  CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        int UpdatedBy { get; set; }
    }
}