namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class required : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "GroupId", "dbo.Groups");
            DropIndex("dbo.Activities", new[] { "GroupId" });
            AlterColumn("dbo.Activities", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Activities", "GroupId");
            AddForeignKey("dbo.Activities", "GroupId", "dbo.Groups", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activities", "GroupId", "dbo.Groups");
            DropIndex("dbo.Activities", new[] { "GroupId" });
            AlterColumn("dbo.Activities", "GroupId", c => c.Int());
            CreateIndex("dbo.Activities", "GroupId");
            AddForeignKey("dbo.Activities", "GroupId", "dbo.Groups", "Id");
        }
    }
}
