using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationSystem.Data;
using AuthenticationSystem.Model;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult<User> Post(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }
    }
}