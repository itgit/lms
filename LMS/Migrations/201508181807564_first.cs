namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Files", "IsShared", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Files", "IsShared", c => c.Boolean(nullable: false));
        }
    }
}
