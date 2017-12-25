namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _Username : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastName", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.Users", "Username", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.Users", "Surname");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Surname", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.Users", "Username");
            DropColumn("dbo.Users", "LastName");
        }
    }
}
