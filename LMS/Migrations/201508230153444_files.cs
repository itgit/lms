namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class files : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "FileName", c => c.String(nullable: false));
            AlterColumn("dbo.Files", "FileExtension", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "FileExtension", c => c.String());
            AlterColumn("dbo.Files", "FileName", c => c.String());
        }
    }
}
