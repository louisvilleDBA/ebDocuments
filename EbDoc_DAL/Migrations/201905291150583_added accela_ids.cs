namespace EbDoc_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedaccela_ids : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ACCELA_ID",
                c => new
                    {
                        B1_ALT_ID = c.String(nullable: false, maxLength: 128),
                        B1_PER_GROUP = c.String(nullable: false),
                        B1_PER_TYPE = c.String(nullable: false),
                        B1_PER_SUB_TYPE = c.String(nullable: false),
                        B1_PER_CATEGORY = c.String(nullable: false),
                        IS_CLOSED = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.B1_ALT_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ACCELA_ID");
        }
    }
}
