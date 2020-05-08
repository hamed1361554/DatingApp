using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Models
{
    public class Session
    {
        [Key]
        public string Id { get; set; }
        public string Token { get; set; }
        public SessionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ClientIp { get; set; }
        public string MacAddress { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum SessionStatus
    {
        Created,
        Login,
        Logout
    }
}