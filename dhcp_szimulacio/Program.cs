using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace dhcp_szimulacio
{
    class Program
    {
        static List<string> excluded = new List<string>();
        static Dictionary<string, string> dhcp = new Dictionary<string, string>();
        static Dictionary<string, string> reserved = new Dictionary<string, string>();
        static List<string> commands = new List<string>();

        static void BeolvasList(List<string>l,string filename)
        {
            try
            {
                StreamReader fajl = new StreamReader(filename);
                try
                {
                    while (!fajl.EndOfStream)
                    {
                        l.Add(fajl.ReadLine());
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
                finally
                {
                    fajl.Close();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        static void BeolvasExcluded()
        {
            try
            {
                StreamReader file = new StreamReader("excluded.csv");
                try
                {
                    while (!file.EndOfStream)
                    {
                        excluded.Add(file.ReadLine());
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                finally 
                {
                    file.Close();
                }

                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string CimEggyelNo(string cim)
        {
            /* cim = 192.168.10.100
             * return = 192.168.10.101
             * szétvágni '.'
             * az utolsót int-é konvertálni és egyet hozzáadni
             * (255 ne lépjük túl)
             * összefűzni string-é
             */
            string[] adat = cim.Split('.');
            int okt4 = int.Parse(adat[3]);
            if (okt4<255)
            {
                okt4++;
            }
            return adat[0]+"."+adat[1]+"."+adat[2]+"."+okt4.ToString();

        }

        static void BeolvasDictionary(Dictionary<string,string> d,string fajlnev)
        {
            try
            {
                StreamReader file = new StreamReader(fajlnev);
                while (!file.EndOfStream)
                {
                    string[] adatok = file.ReadLine().Split(';');
                    d.Add(adatok[0],adatok[1]);
                }
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Feladat(string parancs)
        {
            /* parancs = "request;D19313570A82"
             * először csak "request" paranccsal foglalkozunk
             * Megnézzük,hogy "request"-e?
             * Ki kell szedni a MAC címet a parancsból
             */
            if (parancs.Contains("request"))
            {
                string[] a = parancs.Split(';');
                string mac = a[1];

                if (dhcp.ContainsKey(mac))
                {
                    Console.WriteLine($"DHCP {mac} --> {dhcp[mac]}");
                }
                else
                {
                    if (reserved.ContainsKey(mac))
                    {
                        Console.WriteLine($"Reserved {mac} --> {reserved[mac]}");
                        dhcp.Add(mac,reserved[mac]);
                    }
                    else
                    {
                        string indulo = "192.168.10.100";
                        int okt4 = 100;
                        while (okt4 < 200 && ( dhcp.ContainsValue(indulo)
                            || reserved.ContainsValue(indulo) || excluded.Contains(indulo)))
                        {
                            okt4++;
                            indulo = CimEggyelNo(indulo);
                        }

                        if (okt4 < 200)
                        {
                            Console.WriteLine($"Kiosztott {mac} --> {indulo}");
                            dhcp.Add(mac,indulo);
                        }
                        else
                        {
                            Console.WriteLine($"{mac} nincs IP.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Nem oké.");
            }
        }

        static void Feladatok()
        {
            foreach (var command in commands)
            {
                Feladat(command);
            }
        }

        static void Main(string[] args)
        {
            BeolvasExcluded();
            BeolvasList(excluded,"excluded.csv");
            BeolvasList(commands, "test.csv");
            BeolvasDictionary(dhcp,"dhcp.csv");
            BeolvasDictionary(reserved, "reserved.csv");

            Feladatok();

            //foreach (var e in commands)
            //{
            //    Console.WriteLine(e);
            //}

            Console.WriteLine("\nVége...");
            Console.ReadLine();
        }
    }
}
