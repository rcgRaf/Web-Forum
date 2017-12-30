namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class closedTopics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "IsClosed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Threads", "IsClosed");
        }
    }
}
