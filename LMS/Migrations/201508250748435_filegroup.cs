namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class filegroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "GroupId", c => c.Int());
            CreateIndex("dbo.Files", "GroupId");
            AddForeignKey("dbo.Files", "GroupId", "dbo.Groups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Files", "GroupId", "dbo.Groups");
            DropIndex("dbo.Files", new[] { "GroupId" });
            DropColumn("dbo.Files", "GroupId");
        }
    }
}
