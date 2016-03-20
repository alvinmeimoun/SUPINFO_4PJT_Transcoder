using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service.Enums
{
    public static class EnumManager
    {

        public enum PARAM_TASK_STATUS
        {
            A_FAIRE = 1,
            EN_COURS = 2,
            EFFECTUE = 3,
            ERREUR = 4,
            DECOUPE = 5,
            A_REASSEMBLER = 6,
            SPLIT_EN_COURS = 7
        }
        public enum CONFIG
        {
            MAXLENGTHWITHOUTSPLIT = 1
        }

        public enum FORMAT_TYPE
        {
            AUDIO = 1,
            VIDEO = 2,
            VIDEOTOAUDIO = 3
        }

        
    }
    public class StringManager
    {
        public const string PAIEMENT_ACCEPTE = "Demande";
        public const string CONVERSION_TERMINEE = "ConversionTerminee";
    }
}
