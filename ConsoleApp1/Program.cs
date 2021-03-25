using System;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Имя машины  -  " + Environment.MachineName);
            Console.WriteLine("Имя пользователя  -  " + Environment.UserName);

            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                Console.WriteLine("Процессор  -  " + obj["Name"]);
            }

            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");

            foreach (ManagementObject obj in myVideoObject.Get())
            {
                Console.WriteLine("Видеокарта  -  " + obj["Name"]);
            }

            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                Console.WriteLine("Операционная система  -  " + obj["Caption"]);
                Console.WriteLine("Версия ОС  -  " + obj["Version"]);
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            UInt64 Capacity = 0;
            foreach (ManagementObject WniPART in searcher.Get())
            {
                Capacity += Convert.ToUInt64(WniPART.Properties["Capacity"].Value);
                Console.WriteLine("Установленная оперативка  -  " + Capacity / 1024 / 1024);
            }

            ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject queryObj in baseboardSearcher.Get())
            {
                Console.WriteLine("Материнка  -  " + queryObj["Manufacturer"].ToString() + " " + queryObj["Product"].ToString() + " " + queryObj["SerialNumber"].ToString());
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("IP  -  " + ip.ToString());
                }
            }

            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();

        }
    }
}
