namespace EbDoc_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIs_MissingtoDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Document", "Is_Missing", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Document", "Is_Missing");
        }
    }
}
