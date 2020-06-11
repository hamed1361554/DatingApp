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
        private readonly IDatingRepository datingRepo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private readonly Cloudinary cloudinary;
        private readonly IPhotoRepository photoRepo;

        public PhotosController(IDatingRepository datingRepo, IPhotoRepository photoRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.photoRepo = photoRepo;
            this.datingRepo = datingRepo;
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
            var photoFromRepo = await this.photoRepo.GetPhoto(id);
            return Ok(this.mapper.Map<PhotoForReturnDto>(photoFromRepo));
        }

        [HttpPost]
        public async Task<IActionResult> AddUserPhoto(int userId, [FromForm] PhotoForCreateDto photoForCreateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.datingRepo.GetUser(userId);

            var file = photoForCreateDto.File;

            if (file == null || file.Length <= 0)
                return BadRequest("Could not add zero-length photo!");

            ImageUploadResult uploadResult;
            await using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = await this.cloudinary.UploadAsync(uploadParams);
            }

            if (!string.IsNullOrWhiteSpace(uploadResult.Error?.Message))
                return BadRequest("Upload image encountered error.");

            photoForCreateDto.Url = uploadResult.Url.ToString();
            photoForCreateDto.PublicId = uploadResult.PublicId;

            var photo = this.mapper.Map<Photo>(photoForCreateDto);
            if (!userFromRepo.Photos.Any(u => u.IsMain)) photo.IsMain = true;

            userFromRepo.Photos.Add(photo);
            if (await this.datingRepo.SaveAll()) return CreatedAtRoute("GetPhoto",
                new { userId, id = photo.Id },
                this.mapper.Map<PhotoForReturnDto>(photo));

            return BadRequest("Could not add photo!");
        }

        [HttpPost("{id}/setmain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.datingRepo.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await this.photoRepo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("Photo is already the main one.");

            var currentMain = await this.photoRepo.GetMainPhoto(userId);

            currentMain.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await this.datingRepo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.datingRepo.GetUser(userId);
            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await this.photoRepo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("Photo is the main one. Therefore it can not be deleted.");

            if (!string.IsNullOrWhiteSpace(photoFromRepo.PublicId))
            {
                var result = await this.cloudinary.DestroyAsync(new DeletionParams(photoFromRepo.PublicId));

                if (result.Result != "ok")
                    return BadRequest("Photo could not be deleted from cloud.");
            }

            this.datingRepo.Delete<Photo>(photoFromRepo);

            if (await this.datingRepo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete photo");
        }
    }
}