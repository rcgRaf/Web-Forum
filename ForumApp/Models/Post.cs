namespace ForumApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        
        public int AuthorId { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }

        public int Votes { get; set; }

        public DateTime postedDate { get; set; } = DateTime.UtcNow;

        
        public int ThreadId { get; set; }

        public virtual User User { get; set; }

        public virtual Thread Thread { get; set; }
    }
}
