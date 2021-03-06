using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepo;
        private readonly ISessionRepository sessionRepo;
        private readonly IConfiguration config;
        private readonly IMapper mapper;
        private readonly IPhotoRepository photoRepo;

        public AuthController(IAuthRepository authRepo, IPhotoRepository photoRepo, ISessionRepository sessionRepo, IConfiguration config, IMapper mapper)
        {
            this.photoRepo = photoRepo;
            this.mapper = mapper;
            this.authRepo = authRepo;
            this.sessionRepo = sessionRepo;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await this.authRepo.UserExists(userForRegisterDto.UserName)) return BadRequest("User name already exists.");

            var userToCreate = this.mapper.Map<User>(userForRegisterDto);

            var createdUser = await this.authRepo.Register(userToCreate, userForRegisterDto.Password);

            return CreatedAtRoute("GetUser",
            new
            {
                Controller = "Users",
                id = createdUser.Id
            },
            this.mapper.Map<UserForDetailedDto>(createdUser));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await this.authRepo.Login(userForLoginDto.UserName, userForLoginDto.Password);

            if (userFromRepo == null) return Unauthorized();

            Session session = this.sessionRepo.Create(userFromRepo);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName),
                new Claim(ClaimTypes.Authentication, session.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(Convert.ToInt32(this.config.GetSection("AppSettings:SessionLifeTime").Value)),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            string token = tokenHandler.WriteToken(securityToken);

            await this.sessionRepo.Login(session, token);

            var userToReturn = this.mapper.Map<UserForListDto>(userFromRepo);
            userToReturn.PhotoUrl = (await this.photoRepo.GetMainPhoto(userToReturn.Id))?.Url;

            return Ok(new
            {
                token = token,
                info = userToReturn
            });
        }
    }
}