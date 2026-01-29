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
    [Route("UserController")]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

    }
}