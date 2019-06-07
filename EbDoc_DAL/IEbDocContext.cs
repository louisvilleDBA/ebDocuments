using System;
using System.Data.Entity;

namespace EbDoc_DAL
{
   public interface IEbDocContext : IDisposable
    {
        DbSet<Model.Record> Records { get; }
        int SaveChanges();
        void MarkAsModified(Model.Record record);

        DbSet<Model.Document> Documents { get; }
        void MarkAsMoified(Model.Document document);

        DbSet<Model.ACCELA_ID> ACCELA_IDs { get; }
    }
}
