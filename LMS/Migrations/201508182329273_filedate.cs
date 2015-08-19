namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class filedate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FileDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "FileDate");
        }
    }
}
