using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace pingercop
{
    class Program
    {
        static void Main(string[] args)
        {
            Ping ping = new Ping();
            PingReply reply;

            //AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal); 
            //IntPtr token;
            //LogonUser("mslc", "mslc", "m$lc123#", LogonType.LOGON32_LOGON_BATCH, LogonProvider.LOGON32_PROVIDER_DEFAULT);
            //WindowsIdentity identity = new WindowsIdentity(token);
            //WindowsImpersonationContext context = identity.Impersonate();
            
            Console.WriteLine("Pinging...");
            bool loop = true;

            var contents = File.ReadAllText(@"C:\Users\mslc\Desktop\ips.csv").Split('\n');
            var csv = from line in contents select line.Split(',').ToArray();
            int notCompleted = 0;

            string[] inet = new string[100];
            string[] gids = new string[100];
            string[] isdone = new string[100];

            int i = 0;
            foreach (var row in csv.Skip(0))
            {
                inet[i] = row[0].Trim();
                gids[i] = row[1].Trim();
                isdone[i] = "0";
                i++;
            }


            while (loop)
            {

                for (i = 0; i < inet.Length; i++)
                {

                    if (inet[i] == null) break;

                    string ip = inet[i];
                    string gid = gids[i];

                    if (isdone[i].ToString().Trim().Equals("0"))
                    {
                        Console.WriteLine("Trying: " + ip);
                        reply = ping.Send(ip, 5);
                        if (reply.Status.ToString().Equals("Success"))
                        {
                            int errorStatus = 0;
                            Console.WriteLine(reply.Status.ToString() + " connecting to " + ip);
                            string fileName = gid + ".zip";
                            try
                            {
                                File.Copy(Path.Combine(@"C:\Users\mslc\Desktop\codes", fileName), Path.Combine(@"\\" + ip.ToString() + @"\Users\mslc\Desktop", fileName), true);
                            }
                            catch (Exception e)
                            {
                                errorStatus = 1;
                            }

                            if (errorStatus == 0 && File.Exists(Path.Combine(@"\\" + ip.ToString() + @"\Users\mslc\Desktop", fileName)))
                            {
                                Console.WriteLine("Copied " + gid + "to " + ip);
                                isdone[i] = "1";
                            }
                        }
                    }

                } //end of for loop

                int count1 = 0, count2 = 0;
                for (int j = 0; j < isdone.Length; j++)
                {
                    if (isdone[j] == null) break;

                    if (isdone[j].ToString().Trim().Equals("1"))
                    {
                        count2++;
                    }

                    count1++;
                    //  Console.WriteLine(isdone[j]);
                }
                // Console.WriteLine(count1 + " " + count2);
                if (count1 == count2)
                {
                    loop = false;
                }


            } //end of while loop

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();

        }
    }
}
