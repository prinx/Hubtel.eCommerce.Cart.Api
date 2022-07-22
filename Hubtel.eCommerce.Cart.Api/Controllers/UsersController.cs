﻿#nullable disable
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

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CustomBaseController
    {
        public UsersController(CartContext context, ILogger<UsersController> logger) : base(context, logger)
        {
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            try
            {
                var query = _context.Users.AsQueryable();
                var users = await PaginationService.Paginate(query, page, pageSize);

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
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] GET: api/Users: An error happened: {e}");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

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
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] GET: api/Users/{id}: An error happened: {e}");
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserPostDTO user)
        {
            try
            {
                var logMessage = "";

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

                var updatedUser = new User
                {
                    Id = id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber
                };

                _context.Entry(updatedUser).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        logMessage = "User not found.";
                        _logger.LogInformation($"[{DateTime.Now}] PUT: api/Users/{id}: {logMessage}");
                        
                        return NotFound(new ApiResponseDTO
                        {
                            Status = (int)HttpStatusCode.NotFound,
                            Message = logMessage,
                            Data = updatedUser
                        });
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Users/{id}: User updated successfully.");

                return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] PUT: api/Users/{id}: An error happened: {e}");
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserPostDTO user)
        {
            try
            {
                user.Name.Trim();
                user.PhoneNumber.Trim();

                if (_context.Users.Any(e => e.PhoneNumber == user.PhoneNumber))
                {
                    _logger.LogInformation($"[{DateTime.Now}] POST: api/Users: User with phone number {user.PhoneNumber} already exists.");

                    return Conflict(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Message = "User already exists.",
                        Data = user
                    });
                }

                var newUser = new User
                {
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                };

                _context.Users.Add(newUser);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"[{DateTime.Now}] POST: api/Users: User with phone number {user.PhoneNumber} created successfully.");

                return CreatedAtAction("GetUser", new { id = newUser.Id }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "User created successfully.",
                    Data = newUser
                });
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] POST: api/Users: An error happened: {e}");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Users/{id}: User does not exist. Cannot delete.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "User not found."
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Users/{id}: User deleted successfully.");

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "User deleted usccessfully."
                });

                //return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] DELETE: api/Users/{id}: An error happened: {e}");
            }
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
