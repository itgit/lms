namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class size : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "FileSize", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "FileSize");
        }
    }
}
