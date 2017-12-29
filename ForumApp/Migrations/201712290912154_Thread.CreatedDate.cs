namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThreadCreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "CreatedDate", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Threads", "CreatedDate");
        }
    }
}
