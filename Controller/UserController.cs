using AuthenticationSystem.Data;
using AuthenticationSystem.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public UserController(AppDbContext context, IMapper mapper, UserManager<IdentityUser<int>> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize]
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

        [Authorize]
        [HttpGet("{id:int}")]
        public ActionResult<UserDTO> Get(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user is null)
            {
                return NotFound($"User {id} not found");
            }

            var userDTO = _mapper.Map<UserDTO>(user);

            return Ok(userDTO);
        }

        [Authorize]
        [HttpPut("id:int")]
        public async Task<ActionResult<UserDTO>> Put(int id, UserDTO userDTO)
        {
            var userExist = _context.Users.FirstOrDefault(x => x.Id == id);

            if (userExist is null)
            {
                return NotFound($"User {id} not found");
            }
            
            if (userDTO.Id != id)
            {
                return BadRequest($"The Ids informed is not equals");
            }

           _mapper.Map(userDTO, userExist);


            var resultUpdateUser = await _userManager.UpdateAsync(userExist);

            if (!resultUpdateUser.Succeeded)
            {
                return BadRequest(resultUpdateUser.Errors);
            }

            var userDTOResults = _mapper.Map<UserDTO>(userExist);

            return Ok(userDTOResults);
        }

        [Authorize]   
        [HttpDelete]
        public ActionResult<UserDTO> Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user is null)
            {
                return NotFound($"User {id} not found");
            }

            _userManager.DeleteAsync(user);

            var userDTO = _mapper.Map<UserDTO>(user);

            return Ok(userDTO);
        }
    }
}