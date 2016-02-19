using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using Core.Transcoder.Service;

namespace Core.Transcoder.FFmpegWrapper
{

    // SOURCE : https://github.com/spoiledtechie/FFMpeg.Net.git
    /// <summary>
    /// 
    /// </summary>
    public static class FFMpegService
    {
        public static string FFMPEGExecutableFilePath;
        public static string FullPath = ConfigurationManager.AppSettings["FullFFMPEGExecutableFilePath"];
        //@"D:\Projets\TestsAssetsTranscoder\FFmpeg\bin\ffmpeg.exe";
        private const int MaximumBuffers = 25;

        public static Queue<string> PreviousBuffers = new Queue<string>();

        static FFMpegService()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            //var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            //if (currentDirectory == null)
            //    return;

            FFMPEGExecutableFilePath = new CONFIG_Service().GetConfigValueByName("FFMPEGExecutableFilePath");
                //currentDirectory + ConfigurationManager.AppSettings["FullFFMPEGExecutableFilePath"];
        }


        public static string Execute(string inputFilePath)
        {
            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Input file path cannot be null");
            }

            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = inputFilePath
            };

            return Execute(parameters);
        }

        public static string Execute(string inputFilePath, string outputOptions, string outputFilePath)
        {
            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Input file path cannot be null");
            }

            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Output file path cannot be null");
            }

            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = inputFilePath,
                OutputOptions = outputOptions,
                OutputFilePath = outputFilePath,
            };

            return Execute(parameters);

        }

        public static string Execute(string inputFilePath, string outputOptions)
        {
            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Input file path cannot be null");
            }

            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = inputFilePath,
                OutputOptions = outputOptions
            };

            return Execute(parameters);
        }

        public static string Execute(FFMPEGParameters parameters)
        {
            if (String.IsNullOrWhiteSpace(FFMPEGExecutableFilePath))
            {
                throw new ArgumentNullException("Path to FFMPEG executable cannot be null");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("FFMPEG parameters cannot be completely null");
            }

            using (Process ffmpegProcess = new Process())
            {
                ProcessStartInfo info = new ProcessStartInfo(FFMPEGExecutableFilePath)
                {
                    Arguments = parameters.ToString(),
                    WorkingDirectory = Path.GetDirectoryName(FFMPEGExecutableFilePath),
                    FileName = FFMPEGExecutableFilePath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    LoadUserProfile = true
                };
                ffmpegProcess.StartInfo = info;
                
                ffmpegProcess.Start();
                string processOutput = ffmpegProcess.StandardError.ReadToEnd();
                ffmpegProcess.WaitForExit();
                PreviousBuffers.Enqueue(processOutput);
                lock (PreviousBuffers)
                {
                    while (PreviousBuffers.Count > MaximumBuffers)
                    {
                        PreviousBuffers.Dequeue();
                    }
                }

                return processOutput;
            }

        }

    }

}
