namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Userchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsAdmin", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "Admin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Admin", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "IsAdmin");
        }
    }
}
