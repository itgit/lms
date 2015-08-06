namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "GroupId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "GroupId");
        }
    }
}
