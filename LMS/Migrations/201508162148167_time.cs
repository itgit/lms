namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class time : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "StartTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Activities", "EndTime", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.Activities", "StartTimeHours");
            DropColumn("dbo.Activities", "StartTimeMinutes");
            DropColumn("dbo.Activities", "EndTimeHours");
            DropColumn("dbo.Activities", "EndTimeMinutes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activities", "EndTimeMinutes", c => c.Int(nullable: false));
            AddColumn("dbo.Activities", "EndTimeHours", c => c.Int(nullable: false));
            AddColumn("dbo.Activities", "StartTimeMinutes", c => c.Int(nullable: false));
            AddColumn("dbo.Activities", "StartTimeHours", c => c.Int(nullable: false));
            DropColumn("dbo.Activities", "EndTime");
            DropColumn("dbo.Activities", "StartTime");
        }
    }
}
