using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Microsoft.Win32;

namespace ChangeWallpaper
{
    // DELETE File.Delete(@"\\Test\C$\Users\Public\purrrty.jpg");
    class Program
    {
        private static ArrayList computers = new ArrayList();
        private static ArrayList images = new ArrayList();
        private const int IMAGECOUNT = 71;

        static void Main(string[] args)
        {
            collectComputers(args);
            //computers.Add("Test");
            collectResources(computers.Count);
            //collectResources(1);
            
            saveToDestination();
            // apply wallpaper changes to computers
            applyWallpapers();

            //clean up
            foreach (string computer in computers)
            {
                File.Delete(@"\\" + computer + @"\C$\Windows\System32\rundll.bat");
                File.Delete(@"\\" + computer + @"\C$\Windows\System32\refresh.ps1");
            }

            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\rundll.bat");
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\refresh.ps1");
        }

        private static void saveToDestination()
        {
            int index = 0;

            foreach (string computer in computers)
            {
                if (computer.ToUpper().Equals((System.Environment.MachineName).ToUpper()))
                {
                    // save locally
                    (images[index++] as Bitmap).Save(Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures) + @"\purrrty.jpg");
                }
                else
                {
                    (images[index++] as Bitmap).Save(@"\\" + computer + @"\C$\Users\Public\Pictures\purrrty.jpg");
                }
            }
        }

        private static void collectComputers(string[] args)
        {
            if (args.Length != 0)
            {
                foreach (string computer in args)
                {
                    computers.Add(computer);
                }
            }
            else
            {
                computers.Add("W8-PLEE");
                computers.Add("W8-RFreeman");
                computers.Add("W8-JLam");
                computers.Add("W8-NNguyen");
            }

            // Verify computer are online, removes from list if not
            verify();
        }

        private static void verify()
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            ArrayList listToRemove = new ArrayList();

            foreach (string computer in computers)
            {
                try
                {
                    System.Net.NetworkInformation.PingReply reply = ping.Send(computer);
                    if (reply.Status != System.Net.NetworkInformation.IPStatus.Success)
                    {
                        listToRemove.Add(computer);
                    }
                }
                catch (System.Net.NetworkInformation.PingException)
                {
                    listToRemove.Add(computer);
                }
            }

            if (listToRemove.Count > 0)
            {
                foreach (string computer in listToRemove)
                {
                    computers.Remove(computer);
                }
            }
        }

        private static void collectResources(int size)
        {
            // Pick n random numbers for n computers
            int[] randomNums = getRandomNums(size);

            // Get the photo that matches the name pattern "n".jpeg
            getResources(randomNums);
        }

        private static int[] getRandomNums(int size)
        {
            int[] temp = new int[size];
            Random r = new Random();

            while(size-- > 0)
                temp[size] = r.Next(0, IMAGECOUNT);

            return temp;
        }

        private static void getResources(int[] indexes)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream;
            Bitmap image;

            string[] resources = myAssembly.GetManifestResourceNames();

            //foreach (int index in indexes)
            for(int i = 0; i < indexes.Length; i++)
            {
                try
                {
                    myStream = myAssembly.GetManifestResourceStream(resources[indexes[i]]);
                    image = new Bitmap(myStream);
                    images.Add(image);
                }
                catch
                {
                    // index might reference file instead of image, new random number
                    indexes[i] = (new Random()).Next(0 , IMAGECOUNT);
                    i--;
                }
            }

            // copy files needed
            foreach (string computer in computers)
            {
                if (computer.ToUpper().Equals((System.Environment.MachineName).ToUpper()))
                {
                    // copy locally
                    myStream = myAssembly.GetManifestResourceStream("ChangeWallpaper.rundll.bat");
                    using (var fileStream = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\rundll.bat"))
                    {
                        myStream.CopyTo(fileStream);
                    }

                    myStream = myAssembly.GetManifestResourceStream("ChangeWallpaper.refresh.ps1");
                    using (var fileStream = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\refresh.ps1"))
                    {
                        myStream.CopyTo(fileStream);
                    }
                }
                else
                {
                    myStream = myAssembly.GetManifestResourceStream("ChangeWallpaper.rundll.bat");
                    using (var fileStream = File.Create(@"\\" + computer + @"\C$\Windows\System32\rundll.bat"))
                    {
                        myStream.CopyTo(fileStream);
                    }

                    myStream = myAssembly.GetManifestResourceStream("ChangeWallpaper.refresh.ps1");
                    using (var fileStream = File.Create(@"\\" + computer + @"\C$\Windows\System32\refresh.ps1"))
                    {
                        myStream.CopyTo(fileStream);
                    }

                    myStream = myAssembly.GetManifestResourceStream("ChangeWallpaper.owexec.exe");
                    using (var filestream = File.Create(@"\\" + computer + @"\C$\Windows\System32\owexec.exe"))
                    {
                        myStream.CopyTo(filestream);
                    }
                }
            }
        }

        private static void applyWallpapers()
        {

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            foreach (string computer in computers)
            {
                if (computer.ToUpper().Equals((System.Environment.MachineName).ToUpper()))
                {
                    // run locally
                    System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\rundll.bat");
                }
                else
                {
                    // Refresh Current User registry
                    // owexec.exe -nowait -k rundll.bat -copy -c Test
                    startInfo.Arguments = @"/c C:\Windows\System32\owexec.exe -nowait -k rundll.bat -c " + computer + @"""";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
            }
            System.Threading.Thread.Sleep(5000);
        }
    }
}
