namespace SimpleKit.Domain.Identity
{
    public interface IIdentityWithId<TId>
    {
        TId Id { get;}
    }
}