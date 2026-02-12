using AuthenticationSystem.Data;
using AuthenticationSystem.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> Get()
        {
            var users = _context.Users.ToList();

            if (!users.Any())
            {
                return NotFound("There are no registered users.");
            }

            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(usersDTO);
        }

        [HttpGet("{id:int}")]
        public ActionResult<UserDTO> Get(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id.ToString());

            if (user is null)
            {
                return NotFound($"User {id} not found");
            }

            var userDTO = new UserDTO
            {
              UserName = user.UserName!,
              Email = user.Email!  
            };

            //return Ok(user);
            return Ok(userDTO);
        }

        [HttpPut("id:int")]
        public ActionResult<UserDTO> Put(int id, UserDTO userDTO)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id.ToString());

            if (user.Id != id.ToString())
            {
                return NotFound($"User {id} not found");
            }
            
            

            //_context.Entry(user).State = EntityState.Modified;
            //_context.SaveChanges();

            // return Ok(user);
            return Ok();

        }

        [HttpDelete]
        public ActionResult<UserDTO> Delete(int id)
        {
            //var user = _context.Users.FirstOrDefault(x => x.Id == id);

            //if (user is null)
            //{
                //return NotFound($"User {id} not found");
            //}

            //_context.Users.Remove(user);
            //_context.SaveChanges();

            //return Ok(user);
            return Ok();
        }
    }
}