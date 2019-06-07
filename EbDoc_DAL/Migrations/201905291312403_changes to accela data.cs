namespace EbDoc_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changestoacceladata : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ACCELA_ID", "RecordId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ACCELA_ID", "RecordId", c => c.Long(nullable: false));
        }
    }
}
