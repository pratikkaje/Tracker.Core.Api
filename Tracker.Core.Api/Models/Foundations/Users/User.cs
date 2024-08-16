using System;
using System.ComponentModel.DataAnnotations;

namespace Tracker.Core.Api.Models.Foundations.Users
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(300)]
        public string UserName { get; set; }
        [MaxLength(400)]
        public string Name { get; set; }
        [Required]
        [MaxLength(400)]
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
