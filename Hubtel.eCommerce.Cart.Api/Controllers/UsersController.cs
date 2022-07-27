#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;

using System.Net;
using Hubtel.eCommerce.Cart.Api.Services;
using Hubtel.eCommerce.Cart.Api.Filters;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ValidationActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        protected readonly IUsersService _usersService;
        protected readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            var users = await _usersService.GetUsers(page, pageSize);

            if (users.Items.Count <= 0)
            {
                _logger.LogInformation($"[{DateTime.Now}] GET: api/Users: No user found.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "No user found.",
                    Data = users
                });
            }

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Ok",
                Data = users
            });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _usersService.GetSingleUser(id);

            if (user == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] GET: api/Users/{id}: User not found.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Found.",
                Data = user
            });
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutUser(long id, UserPostDTO user)
        {
            _usersService.ValidateSentUser(user);

            string logMessage;

            //if (id != user.Id)
            //{
            //    logMessage = "Invalid User or Id.";
            //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/Users/{id}: {logMessage}");

            //    return BadRequest(new ApiResponseDTO
            //   {
            //        Status = (int)HttpStatusCode.BadRequest,
            //        Message = logMessage,
            //        Data = user
            //    });
            //}

            try
            {
                _usersService.UpdateUser(id, user);
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Users/{id}: User updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_usersService.UserExists(id))
                {
                    logMessage = "User not found.";
                    _logger.LogInformation($"[{DateTime.Now}] PUT: api/Users/{id}: {logMessage}");
                        
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = logMessage,
                        Data = user
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult PostUser(UserPostDTO user)
        {
            user.Name.Trim();
            user.PhoneNumber.Trim();

            _usersService.ValidateSentUser(user);

            if (_usersService.UserExists(user.PhoneNumber))
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/Users: " +
                    $"User with phone number {user.PhoneNumber} already exists.");

                return Conflict(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Message = "User already exists.",
                    Data = user
                });
            }

            var newUser = _usersService.CreateUser(user);

            _logger.LogInformation($"[{DateTime.Now}] POST: api/Users: User with phone number {user.PhoneNumber} created successfully.");

            return CreatedAtAction("GetUser", new { id = newUser.Id }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "User created successfully.",
                Data = newUser
            });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _usersService.RetrieveUser(id);

            if (user == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Users/{id}: User does not exist. Cannot delete.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                });
            }

            _usersService.DeleteUser(user);

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Users/{id}: User deleted successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "User deleted usccessfully."
            });

            //return NoContent();
        }
    }
}
