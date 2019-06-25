namespace EbDoc_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_has_record : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Document", "Has_Record", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Document", "Has_Record");
        }
    }
}
