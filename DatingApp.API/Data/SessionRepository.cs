using System;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DataContext context;

        public SessionRepository(DataContext context)
        {
            this.context = context;
        }

        public Session Create(User user)
        {
            Session session = new Session()
            {
                Id = Guid.NewGuid().ToString(),
                Status = SessionStatus.Created,
                CreatedAt = DateTime.Now,
                ClientIp = string.Empty,
                MacAddress = String.Empty,
                UserId = user.Id
            };

            return session;
        }

        public async Task<Session> Login(Session session, string token)
        {
            session.Token = token;
            session.Status = SessionStatus.Login;

            await this.context.AddAsync(session);
            await this.context.SaveChangesAsync();

            return session;
        }

        public Task<Session> Logout(int sessionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Session> Update(int sessionId)
        {
            throw new System.NotImplementedException();
        }
    }
}