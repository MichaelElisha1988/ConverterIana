using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;
using System.Runtime.InteropServices;

namespace ConverterIana
{

    class TZItem
    {
        public String TZWindowsId { get; set; }
        public String TZIanaId { get; set; }
        public TimeSpan GMTTZ { get; set; }
        public String IanaToString() { return ("(GMT " + GMTTZ + ") " + TZIanaId); }
        public String WindowsToString() { return "(GMT " + GMTTZ + ") " + TZWindowsId; }
    }
    class FinalTZList
    {
        public List<TZItem> ClassTZList = new List<TZItem>();

        public List<(String, TimeSpan)> FinalList = new List<(String, TimeSpan)>();
        public List<(String, TimeSpan)> OutPutList()
        {
            for(int j = 0; j< ClassTZList.Count; j++)
            {
                FinalList.Add((ClassTZList[j].IanaToString(), ClassTZList[j].GMTTZ));
            }
            return FinalList;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            
            // Testing Counter** int i = 1;


            List<TZItem> TZList = new List<TZItem>();
            ReadOnlyCollection<TimeZoneInfo> TZ = TimeZoneInfo.GetSystemTimeZones();

            foreach (TimeZoneInfo item in TZ)
            {

                /*            WINDOWS CONVERTER              */
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    String CutFromTOIana = TZConvert.WindowsToIana(item.Id).ToString();
                    int indexTO = CutFromTOIana.LastIndexOf('/') + 1;
                    int lenTO = CutFromTOIana.Length - indexTO;


                    try
                    {
                        TZList.Add(new TZItem() { TZIanaId = TZConvert.WindowsToIana(item.Id).Substring(indexTO, lenTO), TZWindowsId = item.Id, GMTTZ = item.BaseUtcOffset });
                    }
                    catch (System.InvalidTimeZoneException)
                    {
                        continue;
                    }
                }

                /*            LINUX CONVERTER              */
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    String CutFromIana = item.Id.ToString();
                    int index = CutFromIana.LastIndexOf('/') + 1;
                    int len = CutFromIana.Length - index;


                    try
                    {
                        if (TZList.FirstOrDefault(tz => tz.TZWindowsId.Contains(TZConvert.IanaToWindows(item.Id))) != null)
                            TZList.Add(new TZItem() { TZIanaId = item.Id.Substring(index, len), TZWindowsId = "No Mutch Exists", GMTTZ = item.BaseUtcOffset });
                        else
                            TZList.Add(new TZItem() { TZIanaId = item.Id.Substring(index, len), TZWindowsId = TZConvert.IanaToWindows(item.Id), GMTTZ = item.BaseUtcOffset });
                    }
                    catch (System.InvalidTimeZoneException)
                    {
                        continue;
                    }
                }

            }

            /*                  Final Mothod        ****"OutPutList()"****                  *item1 =  String               *item = GMT Time        */
            FinalTZList TZfinal = new FinalTZList() { ClassTZList = TZList };
            List<(String, TimeSpan)> ListTZFinal = TZfinal.OutPutList();
            for (int j = 0; j < ListTZFinal.Count; j++)
            {
                Console.WriteLine(ListTZFinal[j].Item1+" :: "+ ListTZFinal[j].Item2);
            }





            /*                 Testing Code                    */
            //foreach (TZItem obj in TZList)
            //{
            //    Console.WriteLine(i+"."+obj.IanaToString());
            //    i++;
            //}
            //Console.WriteLine("                    ***********************                      ");
            //i = 1;
            //foreach (TZItem obj in TZList)
            //{
            //    Console.WriteLine(i + "." + obj.WindowsToString());
            //    i++;
            //}

            //Console.WriteLine("                    ***********************                      ");


            //Console.WriteLine("Enter Name Location: ");
            //String LocationName = Console.ReadLine();
            //var CorrectLocationTime = TZList.FirstOrDefault(tz => tz.TZWindowsId.Contains(LocationName) || tz.TZIanaId.Contains(LocationName));
            //if (CorrectLocationTime != null)
            //{
            //    Console.WriteLine("{0} :: {1}", CorrectLocationTime.IanaToString(), DateTime.UtcNow + CorrectLocationTime.GMTTZ);
            //}
            //else
            //    Console.WriteLine("Incorrect location");

        }
    }
}
