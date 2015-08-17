namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfile3 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Files", name: "Group_Id", newName: "GroupId");
            RenameIndex(table: "dbo.Files", name: "IX_Group_Id", newName: "IX_GroupId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Files", name: "IX_GroupId", newName: "IX_Group_Id");
            RenameColumn(table: "dbo.Files", name: "GroupId", newName: "Group_Id");
        }
    }
}
