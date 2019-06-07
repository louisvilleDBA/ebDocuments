using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data;

using System.Data.Entity.ModelConfiguration.Conventions;

using System.ComponentModel.DataAnnotations.Schema;
using EbDoc_DAL.Model;
using EntityState = System.Data.Entity.EntityState;

namespace EbDoc_DAL
{
    public class EbDocContext : DbContext, IEbDocContext
    {
        public EbDocContext() : base("name=DB_ConnectionString") { }

        public DbSet<Record> Records { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ACCELA_ID> ACCELA_IDs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public void MarkAsModified(Record record)
        {
            Entry(record).State = EntityState.Modified;
        }

        public void MarkAsMoified(Document document)
        {
            Entry(document).State = EntityState.Modified;
        }
    }
}
