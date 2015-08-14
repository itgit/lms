namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "Schedule_Id", "dbo.Schedules");
            DropForeignKey("dbo.Groups", "Schedule_Id", "dbo.Schedules");
            DropIndex("dbo.Activities", new[] { "Schedule_Id" });
            DropIndex("dbo.Groups", new[] { "Schedule_Id" });
            AddColumn("dbo.Activities", "Group_Id", c => c.Int());
            CreateIndex("dbo.Activities", "Group_Id");
            AddForeignKey("dbo.Activities", "Group_Id", "dbo.Groups", "Id");
            DropColumn("dbo.Activities", "Schedule_Id");
            DropColumn("dbo.Groups", "Schedule_Id");
            DropTable("dbo.Schedules");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Groups", "Schedule_Id", c => c.Int());
            AddColumn("dbo.Activities", "Schedule_Id", c => c.Int());
            DropForeignKey("dbo.Activities", "Group_Id", "dbo.Groups");
            DropIndex("dbo.Activities", new[] { "Group_Id" });
            DropColumn("dbo.Activities", "Group_Id");
            CreateIndex("dbo.Groups", "Schedule_Id");
            CreateIndex("dbo.Activities", "Schedule_Id");
            AddForeignKey("dbo.Groups", "Schedule_Id", "dbo.Schedules", "Id");
            AddForeignKey("dbo.Activities", "Schedule_Id", "dbo.Schedules", "Id");
        }
    }
}
