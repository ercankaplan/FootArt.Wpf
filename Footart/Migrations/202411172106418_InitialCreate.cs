namespace Footart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudyDatas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Gender = c.String(),
                        Optime = c.DateTime(nullable: false),
                        FileName = c.String(),
                        DataType = c.Int(nullable: false),
                        Side = c.String(),
                        StudyDataValue = c.Double(nullable: false),
                        StudyDataText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudyDataPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudyDataId = c.Guid(nullable: false),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudyDatas", t => t.StudyDataId, cascadeDelete: true)
                .Index(t => t.StudyDataId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudyDataPoints", "StudyDataId", "dbo.StudyDatas");
            DropIndex("dbo.StudyDataPoints", new[] { "StudyDataId" });
            DropTable("dbo.StudyDataPoints");
            DropTable("dbo.StudyDatas");
        }
    }
}
