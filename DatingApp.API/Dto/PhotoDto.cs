using System;

namespace DatingApp.API.Dto
{
    public class PhotoDto : PhotoBaseDto
    {
        public int Id { get; set; }
        public bool IsMain { get; set; }
    }
}