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
    
    public partial class TRACE
    {
        public int PK_ID_TRACE { get; set; }
        public int FK_ID_TASK { get; set; }
        public int FK_ID_SERVER { get; set; }
        public string METHOD { get; set; }
        public string DESCRIPTION { get; set; }
        public string TYPE { get; set; }
    }
}
