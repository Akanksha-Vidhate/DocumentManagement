using DocumentManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DocumentManagementApi.DataAccess
{
    public class DocumentDbContext: DbContext
    {
        public DocumentDbContext(): base("name=DocumentDBConnectionString")
        {
            Database.SetInitializer<DocumentDbContext>(new CreateDatabaseIfNotExists<DocumentDbContext>());
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}