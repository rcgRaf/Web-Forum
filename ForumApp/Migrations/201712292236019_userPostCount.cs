namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userPostCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "TotalPosts", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "TotalPosts");
        }
    }
}
