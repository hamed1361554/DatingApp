using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext context;
        
        public PhotoRepository(DataContext context)
        {
            this.context = context;

        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await this.context.Photos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Photo>> GetPhotos(int userId)
        {
            return await this.context.Photos.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Photo> GetMainPhoto(int userId)
        {
            return await this.context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain == true);
        }
    }
}