namespace ForumApp.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ForumContext : DbContext
    {
        public ForumContext()
            : base("name=ForumContext")
        {
        }

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<SentVote> SentVotes { get; set; }
        public virtual DbSet<Thread> Threads { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<Thread>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Thread>()
                .HasMany(e => e.Posts)
                .WithRequired(e => e.Thread)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Topic>()
                .HasMany(e => e.Threads)
                .WithRequired(e => e.Topic)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsFixedLength();

            modelBuilder.Entity<User>()
                .HasMany(e => e.Posts)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.AuthorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Threads)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.AuthorId)
                .WillCascadeOnDelete(false);
        }
    }
}
