namespace SimpleKit.Repository.EfCore
{
    public enum ObjectState
    {
        Added,
        Modified,
        Deleted,
        Unchanged
    }
    public interface IObjectState
    {
        ObjectState State { get; set; }
    }
}