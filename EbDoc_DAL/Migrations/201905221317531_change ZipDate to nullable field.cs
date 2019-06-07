namespace EbDoc_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeZipDatetonullablefield : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Document", "Zip_Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Document", "Zip_Date", c => c.DateTime(nullable: false));
        }
    }
}
