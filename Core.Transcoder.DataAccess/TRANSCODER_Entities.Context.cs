﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Core.Transcoder.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TRANSCODEREntities : DbContext
    {
        public TRANSCODEREntities()
            : base("name=TRANSCODEREntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TRACE> TRACE { get; set; }
        public virtual DbSet<CONFIG> CONFIG { get; set; }
        public virtual DbSet<PARAM_SPLIT> PARAM_SPLIT { get; set; }
        public virtual DbSet<PARAM_TASK_STATUS> PARAM_TASK_STATUS { get; set; }
        public virtual DbSet<USER> USER { get; set; }
        public virtual DbSet<FORMAT> FORMAT { get; set; }
        public virtual DbSet<FORMAT_TYPE> FORMAT_TYPE { get; set; }
        public virtual DbSet<TASK> TASK { get; set; }
    }
}
