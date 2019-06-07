namespace EbDoc_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Document",
                c => new
                    {
                        DocumentId = c.Long(nullable: false, identity: true),
                        Source_Repository = c.String(),
                        Target_Repository = c.String(),
                        MSD_path = c.String(nullable: false),
                        File_Name = c.String(nullable: false),
                        File_Size = c.Long(nullable: false),
                        ArchiveNo = c.String(),
                        File_Path = c.String(),
                        Zip_Path = c.String(),
                        Zip_Date = c.DateTime(nullable: false),
                        Metadata_Path = c.String(),
                        RecordId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("dbo.Record", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.RecordId);
            
            CreateTable(
                "dbo.Record",
                c => new
                    {
                        RecordId = c.Long(nullable: false, identity: true),
                        Hansen_Module = c.String(nullable: false),
                        Hansen_Id = c.String(nullable: false),
                        B1_ALT_ID = c.String(),
                        Group = c.String(),
                        Type = c.String(),
                        Subtype = c.String(),
                        Category = c.String(),
                        Is_Closed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Document", "RecordId", "dbo.Record");
            DropIndex("dbo.Document", new[] { "RecordId" });
            DropTable("dbo.Record");
            DropTable("dbo.Document");
        }
    }
}
