using System.Net.Http.Headers;

namespace ProductMgt.Infrastructure.UserContext
{
    public class UserContext
    {
        public int UserId { get; set; }
    }

    public interface IUserContextFactory
    {
        UserContext Create();
    }

    public class UserContextFactory : IUserContextFactory
    {
        public UserContext Create()
        {
            return new UserContext()
            {
                UserId =  999
            };
        }
    }
}