namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfile1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Files", "UserId");
            AddForeignKey("dbo.Files", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Files", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Files", new[] { "UserId" });
            DropColumn("dbo.Files", "UserId");
        }
    }
}
