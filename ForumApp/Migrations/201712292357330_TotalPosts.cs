namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TotalPosts : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "TotalPosts");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "TotalPosts", c => c.Int(nullable: false));
        }
    }
}
