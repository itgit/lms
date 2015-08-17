namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activities : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Activities", name: "Group_Id", newName: "GroupId");
            RenameIndex(table: "dbo.Activities", name: "IX_Group_Id", newName: "IX_GroupId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Activities", name: "IX_GroupId", newName: "IX_Group_Id");
            RenameColumn(table: "dbo.Activities", name: "GroupId", newName: "Group_Id");
        }
    }
}
