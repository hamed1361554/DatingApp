using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface ISessionRepository
    {
         Session Create(User user);

         Task<Session> Login(Session session, string token);

         Task<Session> Logout(int sessionId);

         Task<Session> Update(int sessionId);
    }
}