namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fileextensions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FileExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "FileExtension");
        }
    }
}
