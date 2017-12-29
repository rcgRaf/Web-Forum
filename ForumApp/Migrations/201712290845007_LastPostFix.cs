namespace ForumApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastPostFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "LastPost", c => c.DateTime());
            DropColumn("dbo.Topics", "LastPost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Topics", "LastPost", c => c.DateTime());
            DropColumn("dbo.Threads", "LastPost");
        }
    }
}
