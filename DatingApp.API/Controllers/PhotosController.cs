using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private readonly Cloudinary cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.cloudinaryConfig = cloudinaryConfig;

            Account cloudinaryAccount = new Account(this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret);

            cloudinary = new Cloudinary(cloudinaryAccount);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await this.repo.GetPhoto(id);
            return Ok(this.mapper.Map<PhotoForReturnDto>(photoFromRepo));
        }

        [HttpPost]
        public async Task<IActionResult> AddUserPhoto(int userId, [FromForm]PhotoForCreateDto photoForCreateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.repo.GetUser(userId);

            var file = photoForCreateDto.File;
            ImageUploadResult uploadResult;

            if (file == null || file.Length <= 0)
                return BadRequest("Could not add zero-length photo!");

            await using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = this.cloudinary.Upload(uploadParams);
            }

            photoForCreateDto.Url = uploadResult.Url.ToString();
            photoForCreateDto.PublicId = uploadResult.PublicId;

            var photo = this.mapper.Map<Photo>(photoForCreateDto);
            if (!userFromRepo.Photos.Any(u => u.IsMain)) photo.IsMain = true;

            userFromRepo.Photos.Add(photo);
            if (await this.repo.SaveAll()) return CreatedAtRoute("GetPhoto", 
                new {userId, id = photo.Id},
                this.mapper.Map<PhotoForReturnDto>(photo));

            return BadRequest("Could not add photo!");
        }
    }
}