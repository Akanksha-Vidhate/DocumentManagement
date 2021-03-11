namespace DocumentManagementApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentDB_V3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "User_Id", c => c.Int());
            CreateIndex("dbo.Documents", "User_Id");
            AddForeignKey("dbo.Documents", "User_Id", "dbo.AppUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "User_Id", "dbo.AppUsers");
            DropIndex("dbo.Documents", new[] { "User_Id" });
            DropColumn("dbo.Documents", "User_Id");
        }
    }
}
