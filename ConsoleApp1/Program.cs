using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            // Файл, в котором лежит путь, куда сохранять данные, должен лежать в папке с программой и называться config.txt
            // Путь должен быть записан в этом файле первой строкой, слэша (\) в конце не надо
            string settingsPath = Directory.GetCurrentDirectory() + "\\config.txt";
            string destinationPath = "";

            try
            {
                if (!File.Exists(settingsPath)) throw new FileNotFoundException();

                destinationPath = File.ReadAllLines(settingsPath)[0];

                if (!Directory.Exists(destinationPath)) throw new DirectoryNotFoundException();

                Console.WriteLine("Введите имя пользователя и нажмите клавишу Enter:");
                var userName = Console.ReadLine();

                var data = GetData(userName);

                SaveDataToJson(data, $"{destinationPath}\\{userName} - {DateTime.Now.ToString("d")}.json");
            }
            catch (DirectoryNotFoundException)
            {
                ErrorHandler($"\nНевозможно получить доступ к папке, указанной в файле конфигурации \"config.txt\":\n{destinationPath}");
            }
            catch (FileNotFoundException)
            {
                ErrorHandler($"\nОтсутствует файл конфигурации \"config.txt\", содержащий целевой путь сохранения данных.");
            }
            catch (Exception e)
            {
                ErrorHandler($"\nНепредвиденная ошибка!\n{e.Message}");
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Обработчик ошибок. Выводит текст ошибки в консоль и дописывает его в файл errors.txt в папке с программой
        /// </summary>
        /// <param name="result">Текст ошибки</param>
        private static void ErrorHandler(string result)
        {
            Console.WriteLine(result);
            File.AppendAllText(Directory.GetCurrentDirectory() + "\\error.txt", DateTime.Now.ToString() + result);
        }

        /// <summary>
        /// Сохраняет полученный словарь данных в Json-файл по указанному пути.
        /// </summary>
        /// <param name="dict">Словарь записываемых данных</param>
        /// <param name="destinationath">Путь, по которому должен быть сохранён, файл. Должен представлять полный путь с названием файла и расширением .json</param>
        private static void SaveDataToJson(Dictionary<string, string> dict, string destinationath)
        {
            byte[] jsonUtf8Bytes;

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };

            jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(dict, options);

            File.WriteAllBytes(destinationath, jsonUtf8Bytes);

            Console.WriteLine("Данные успешно сохранены.");
        }

        /// <summary>
        /// Получает данные о компьютере и возвращает их в виде словаря "ключ-значение"
        /// </summary>
        /// <param name="result">Словарь данных о компьютере</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetData(string result)
        {
            var dict = new Dictionary<string, string>();

            dict.Add("Введенное пользователем имя", result);
            dict.Add("Имя машины", Environment.MachineName);
            dict.Add("Имя пользователя", Environment.UserName);

            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                dict.Add("Процессор", obj["Name"].ToString());
            }

            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                dict.Add("Видеокарта", obj["Name"].ToString());
            }

            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                dict.Add("Операционная система", obj["Caption"].ToString());
                dict.Add("Версия ОС", obj["Version"].ToString());
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            //UInt64 Capacity = 0;
            foreach (ManagementObject WniPART in searcher.Get())
            {
                UInt64 Capacity = Convert.ToUInt64(WniPART.Properties["Capacity"].Value);
                dict.Add("Установленная оперативка", (Capacity / 1024 / 1024).ToString());
            }

            ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject queryObj in baseboardSearcher.Get())
            {
                dict.Add("Материнка", queryObj["Manufacturer"].ToString() + " " + queryObj["Product"].ToString() + " " + queryObj["SerialNumber"].ToString());
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var i = 1;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    dict.Add("IP адрес " + i, ip.ToString());
                    i++;
                }
            }

            Console.WriteLine("Данные успешно собраны.");

            return dict;
        }
    }
}
