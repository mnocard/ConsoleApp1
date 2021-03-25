using System;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Введите имя пользователя и нажмите клавишу Enter:");
            string result = Console.ReadLine();
            string path = Directory.GetCurrentDirectory() + "\\config.txt";
            string destinationath;

            if (File.Exists(path))
            {
                var temp = File.ReadAllLines(path);
                destinationath = temp[0];
            }
            else
            {
                Console.WriteLine("Отсутствует файл конфигурации, содержащий целевой путь сохранения данных.");
                return;
            }

            ComputerInfo computerInfo = new ComputerInfo();

            Console.WriteLine("Имя машины  -  " + Environment.MachineName);
            Console.WriteLine("Имя пользователя  -  " + Environment.UserName);

            computerInfo.CurrentName = "Введенное пользователем имя";
            computerInfo.CurrentNameContent = result;

            computerInfo.MachineName = "Имя машины";
            computerInfo.MachineNameContent = Environment.MachineName;
            computerInfo.UserName = "Имя пользователя";
            computerInfo.UserNameContent = Environment.UserName;

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ComputerInfo));

            destinationath = destinationath + result + ".xml";
            FileStream file = File.Create(destinationath);

            writer.Serialize(file, computerInfo);
            file.Close();

            try
            {
                ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");

                foreach (ManagementObject obj in myProcessorObject.Get())
                {
                    Console.WriteLine("Процессор  -  " + obj["Name"]);
                    result += "\nПроцессор  -  " + obj["Name"];
                }

                ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");

                foreach (ManagementObject obj in myVideoObject.Get())
                {
                    Console.WriteLine("Видеокарта  -  " + obj["Name"]);
                    result += "\nВидеокарта  -  " + obj["Name"];
                }

                ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

                foreach (ManagementObject obj in myOperativeSystemObject.Get())
                {
                    Console.WriteLine("Операционная система  -  " + obj["Caption"]);
                    Console.WriteLine("Версия ОС  -  " + obj["Version"]);
                    result += "\nОперационная система  -  " + obj["Caption"];
                    result += "\nВерсия ОС  -" + obj["Version"];
                }

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                UInt64 Capacity = 0;
                foreach (ManagementObject WniPART in searcher.Get())
                {
                    Capacity += Convert.ToUInt64(WniPART.Properties["Capacity"].Value);
                    Console.WriteLine("Установленная оперативка  -  " + Capacity / 1024 / 1024);
                    result += "\nУстановленная оперативка  -  " + Capacity / 1024 / 1024;
                }

                ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

                foreach (ManagementObject queryObj in baseboardSearcher.Get())
                {
                    Console.WriteLine("Материнка  -  " + queryObj["Manufacturer"].ToString() + " " + queryObj["Product"].ToString() + " " + queryObj["SerialNumber"].ToString());
                    result += "\nМатеринка  -  " + queryObj["Manufacturer"].ToString() + " " + queryObj["Product"].ToString() + " " + queryObj["SerialNumber"].ToString();
                }

                var host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine("IP  -  " + ip.ToString());
                        result += "\nIP  -  " + ip.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                result += "Непредвиденная ошибка!\n" + e.Message;
            }

            Console.ReadKey();
        }
    }
}
