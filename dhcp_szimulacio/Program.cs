using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dhcp_szimulacio
{
    class Program
    {
        static List<string> excluded = new List<string>();
        static Dictionary<string, string> dhcp = new Dictionary<string, string>();
        static Dictionary<string, string> reserved = new Dictionary<string, string>();

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
        static void Main(string[] args)
        {
            BeolvasExcluded();
            BeolvasDictionary(dhcp,"dhcp.csv");
            BeolvasDictionary(reserved, "reserved.csv");
            
            foreach (var e in reserved)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("\nVége...");
            Console.ReadLine();
        }
    }
}
