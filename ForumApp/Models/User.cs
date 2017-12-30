namespace ForumApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            using (var set =new ForumContext())

            Posts = new HashSet<Post>();
            Threads = new HashSet<Thread>();

            

        }

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public byte[] Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Email { get; set; }

        public bool Admin { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required]
        [StringLength(20)]
        public string City { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Thread> Threads { get; set; }

        public int TotalPosts => Posts.Count;

    }
}
