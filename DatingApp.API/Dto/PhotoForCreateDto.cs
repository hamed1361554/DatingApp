using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Dto
{
    public class PhotoForCreateDto : PhotoBaseDto
    {
        public IFormFile File { get; set; }
        public string PublicId { get; set; }

        public PhotoForCreateDto()
        {
            this.DateAdded = DateTime.Now;
        }
    }
}