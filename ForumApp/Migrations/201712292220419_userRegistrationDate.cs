namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userRegistrationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "RegisteredDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "RegisteredDate");
        }
    }
}
