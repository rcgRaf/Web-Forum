namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Posts", new[] { "AuthorId" });
            DropIndex("dbo.Posts", new[] { "ThreadId" });
            AlterColumn("dbo.Posts", "AuthorId", c => c.Int(nullable: false));
            AlterColumn("dbo.Posts", "ThreadId", c => c.Int(nullable: false));
            CreateIndex("dbo.Posts", "AuthorId");
            CreateIndex("dbo.Posts", "ThreadId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Posts", new[] { "ThreadId" });
            DropIndex("dbo.Posts", new[] { "AuthorId" });
            AlterColumn("dbo.Posts", "ThreadId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Posts", "AuthorId", c => c.Int(nullable: false, identity: true));
            CreateIndex("dbo.Posts", "ThreadId");
            CreateIndex("dbo.Posts", "AuthorId");
        }
    }
}
