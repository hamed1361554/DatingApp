using System;

namespace DatingApp.API.Dto
{
    public class PhotoBaseDto
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
    }
}