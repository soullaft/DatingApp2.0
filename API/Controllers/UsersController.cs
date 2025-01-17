using API.Data;
using API.Data.Cache;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(DataContext dataContext, CacheManager cacheManager) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsersAsync() => Ok(await dataContext.Users.ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUserAsync(int id)
        {
            if (cacheManager.TryGetValue(id, out AppUser? user)) return Ok(user);
            
            user = await dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound($"User with id {id} doesn't exist");
            }

            cacheManager.SetValue(user.Id, user);

            return Ok(user);
        }
    }
}
