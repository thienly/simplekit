using SimpleKit.Domain.Entities;

namespace ProductMgt.Domain
{
    public class Category : AggregateRootWithId<long>
    {
        private string _name;

        public Category(string name)
        {
            _name = name;
        }
        public string Name => _name;
    }
}