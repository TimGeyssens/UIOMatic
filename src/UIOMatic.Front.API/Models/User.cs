using System;
using System.ComponentModel.DataAnnotations;
using UIOMatic.Attributes;
using UIOMatic.Front.API.Repositories;

namespace UIOMatic.Front.API.Models
{
    [UIOMatic("Users", "Users", "User", FolderIcon = "icon-people", ItemIcon = "icon-person",
    RepositoryType = typeof(LiteDBRepository))]
    public class User
    {
        public string Id { get; set; }

        [UIOMaticField(Name = "Username", Description = "The username rof the user", View = "textfield")]
        [UIOMaticListViewField(Name = "Username")]
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [UIOMaticField(Name= "HashedPassword", Description = "The hashed password of the user", View = "textfield")]
        [Required]
        public string HashedPassword { get; set; }

        [UIOMaticField(Name= "Email", Description = "The email of the user", View = "textfield")]
        [UIOMaticListViewField(Name = "Email")]
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [UIOMaticListViewField(Name = "CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [UIOMaticListViewField(Name = "LastLoginAt")]
        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
} 