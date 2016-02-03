﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.WindowsService
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new TranscoderWindowsService() 
            //};
            //ServiceBase.Run(ServicesToRun);

            // POUR DEBUG
            TranscoderWindowsService transcoder = new TranscoderWindowsService();
            transcoder.StartDebug();
        }

    }
}
