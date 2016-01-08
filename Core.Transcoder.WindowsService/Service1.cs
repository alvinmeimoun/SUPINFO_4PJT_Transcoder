using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.FFmpegWrapper;

namespace Core.Transcoder.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            FFMpegService.Execute("D:\\Projets\\Music\\HIN.wav", "-acodec libmp3lame", "D:\\Projets\\ConvertedMusic\\d.mp3");
        }

        protected override void OnStop()
        {
        }
    }
}
