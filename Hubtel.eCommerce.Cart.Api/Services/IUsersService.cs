using System;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface IUsersService : IControllerService
    {
        public void ValidateGetUsersQueryString(int page, int pageSize);

        public Task<Pagination<User>> GetUsers(int page, int pageSize);

        public Task<User> GetSingleUser(long id);

        public Task UpdateUser(long id, UserPostDTO user);

        public Task<User> CreateUser(UserPostDTO user);

        public Task<User> RetrieveUser(long id);

        public Task DeleteUser(User user);

        public void ValidateSentUser(UserPostDTO user);

        public bool UserExists(long id);

        public bool UserExists(string phoneNumber);
    }
}

