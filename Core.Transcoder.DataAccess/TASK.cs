//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class TASK
    {
        public int PK_ID_TASK { get; set; }
        public Nullable<int> FK_ID_USER { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string FILE_URL { get; set; }
        public Nullable<int> FK_ID_FORMAT_BASE { get; set; }
        public Nullable<int> FK_ID_FORMAT_TO_CONVERT { get; set; }
        public Nullable<bool> IS_PAID { get; set; }
        public Nullable<int> THREAD_ID { get; set; }
        public Nullable<int> SERVER_ID { get; set; }
        public Nullable<System.DateTime> DATE_BEGIN_CONVERSION { get; set; }
        public Nullable<System.DateTime> DATE_END_CONVERSION { get; set; }
        public Nullable<int> FK_ID_PARENT_TASK { get; set; }
        public string FILE_URL_TEMP { get; set; }
        public string FILE_URL_DESTINATION { get; set; }
        public Nullable<int> LENGTH { get; set; }
        public Nullable<int> FK_ID_PARAM_SPLIT { get; set; }
    }
}
