using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IPhotoRepository
    {
         Task<Photo> GetPhoto(int id);
         Task<IEnumerable<Photo>> GetPhotos(int userId);
         Task<Photo> GetMainPhoto(int userId);
    }
}