using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace XMLviewer
{
    class Program
    {
        static void Main(string[] args)
        {
            string writePath = AppDomain.CurrentDomain.BaseDirectory + "XML Обновлений " + Environment.MachineName + ".txt";
            if (!File.Exists(writePath))
            {
                Console.WriteLine("Создаю пустой txt файл");
            }
            else
            {
                Console.WriteLine("Файл XML Обновлений.txt существует, он будет перезаписан");
                File.Delete(writePath);
            }

            //http://www.outsidethebox.ms/17988/

            //регулярное выражение для поиска по маске KB
            Regex regex = new Regex(@"KB[0-9]{6,7}");
            //Regex(@"(\w{2}\d{6,7}) ?");

            //SortedSet не поддерживает повторяющиеся элементы, поэтмоу повторяющиеся элементы мы "группируем" ещё на стадии добавления
            SortedSet<string> spisok = new SortedSet<string>();

            XmlDocument xDoc = new XmlDocument();
            string path = "C:\\Windows\\servicing\\Packages\\wuindex.xml"; //путь до нашего xml
            xDoc.Load(path);

            int kol = 0; //кол-во компонентов
            int total = 0; //кол-во дочерних элементов в xml
            int total2 = 0; //кол-во полученных обновлений

            XmlNodeList name = xDoc.GetElementsByTagName("Mappings");
            foreach (XmlNode xnode in name)
            {
                //Console.WriteLine(xnode.Name);
                kol++;
                XmlNode attr = xnode.Attributes.GetNamedItem("UpdateId");
                //Console.WriteLine(attr.Value);

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    XmlNode childattr = childnode.Attributes.GetNamedItem("Package");
                    total++;
                    //Console.WriteLine(childattr.Value);

                    MatchCollection matches = regex.Matches(childattr.Value);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                            //Console.WriteLine(match.Value);
                            spisok.Add(match.Value);
                    }
                    else
                    {
                        //Console.WriteLine("Совпадений не найдено");
                    }
                }

            }

            try
            {
                StreamWriter sw = new StreamWriter(writePath);
                foreach (string element in spisok)
                {
                    //Console.WriteLine(element);
                    sw.WriteLine(element);
                    total2++;
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            //Console.WriteLine("\n");
            Console.WriteLine("Количество пакетов: " +kol);

            Console.WriteLine("Количество дочерних элементов в xml: " + total);

            Console.WriteLine("Количество KB обновлений: " + total2);

            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.Read();
        }
    }
}
