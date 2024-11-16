using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Footart.Models;

namespace Footart.Utilities
{
    public static class DBHelper
    {
        private static string rootPath = @"C:\Footart\";
        private static object lockObject = new object();
        private static string currentFileName = string.Empty;

        private static StreamWriter currentFileStream = null;
        private static string sHeaders = "FileName@Name@Surname@Gender@RightOrLeft@SideOrBack@LAA@LAA_X1@LAA_Y1@LAA_X2@LAA_Y2@LAA_X3@LAA_Y3@Width@Width_X1@Width_Y1@Width_X2@Width_Y2@Optime";
        public static void CreateDataFile(string pRootPath, string fileName)
        {
            if (fileName != string.Empty)
            {
                lock (lockObject)
                {
                    currentFileName = fileName;
                    rootPath = pRootPath;


                    if (!System.IO.File.Exists(pRootPath + fileName + ".txt"))
                    {
                        
                        currentFileStream = new StreamWriter(rootPath + currentFileName + ".txt", true);

                        

                        currentFileStream.WriteLine(sHeaders);
                        currentFileStream.Flush();
                        currentFileStream.Close();
                    }
                }
            }

        }


        public static void WriteDataFile(FootData hd)
        {


            string sFormat = "{0}";
            for (int i = 1; i < sHeaders.Split("@".ToArray()).Count(); i++)
            {
                sFormat += "@{" + i.ToString() + "}";
            }

            currentFileStream = new StreamWriter(rootPath + currentFileName + ".txt", true);


            currentFileStream.WriteLine(string.Format(sFormat,
                                                               hd.FileName,
                                                               hd.Name,
                                                               hd.Surname,
                                                               hd.Gender,
                                                               hd.RightOrLeft,
                                                               hd.SideOrBack,
                                                               hd.LAA,
                                                               hd.LAA_X1,
                                                               hd.LAA_Y1,
                                                               hd.LAA_X2,
                                                               hd.LAA_Y2,
                                                               hd.LAA_X3,
                                                               hd.LAA_Y3,
                                                               hd.Width,
                                                               hd.Width_X1,
                                                               hd.Width_Y1,
                                                               hd.Width_X2,
                                                               hd.Width_Y2,
                                                               hd.Optime

           ));

            currentFileStream.Flush();
            currentFileStream.Close();

        }

        public static List<FootData> ReadDataFile(string fileName)
        {

            List<FootData> dataList = new List<FootData>();

            string currentLine = string.Empty;
            char[] seperator = "@".ToArray();
            string[] columns = null;

            bool isHeader = true;

            using (System.IO.StreamReader sr = new System.IO.StreamReader(rootPath + currentFileName + ".txt"))
            {

                while (true)
                {

                    currentLine = sr.ReadLine();
                    if (string.IsNullOrEmpty(currentLine))
                        break;

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    columns = currentLine.Split(seperator);

                    FootData hd = new FootData()
                    {
                        FileName = columns[0],
                        Name = string.IsNullOrEmpty(columns[1]) ? "" : columns[1],
                        Surname = string.IsNullOrEmpty(columns[2]) ? "" : columns[2],
                        Gender = Convert.ToByte(columns[3]),
                        RightOrLeft = string.IsNullOrEmpty(columns[4]) ? (byte)0 : Convert.ToByte(columns[4]),
                        SideOrBack = string.IsNullOrEmpty(columns[5]) ? (byte)0 : Convert.ToByte(columns[5]),
                        LAA = string.IsNullOrEmpty(columns[6]) ? 0 : Convert.ToDouble(columns[6]),
                        LAA_X1 = string.IsNullOrEmpty(columns[7]) ? 0 : Convert.ToDouble(columns[7]),
                        LAA_Y1 = string.IsNullOrEmpty(columns[8]) ? 0 : Convert.ToDouble(columns[8]),
                        LAA_X2 = string.IsNullOrEmpty(columns[9]) ? 0 : Convert.ToDouble(columns[9]),
                        LAA_Y2 = string.IsNullOrEmpty(columns[10]) ? 0 : Convert.ToDouble(columns[10]),
                        LAA_X3 = string.IsNullOrEmpty(columns[11]) ? 0 : Convert.ToDouble(columns[11]),
                        LAA_Y3 = string.IsNullOrEmpty(columns[12]) ? 0 : Convert.ToDouble(columns[12]),
                        Width = string.IsNullOrEmpty(columns[13]) ? 0 : Convert.ToDouble(columns[13]),
                        Width_X1 = string.IsNullOrEmpty(columns[14]) ? 0 : Convert.ToDouble(columns[14]),
                        Width_Y1 = string.IsNullOrEmpty(columns[15]) ? 0 : Convert.ToDouble(columns[15]),
                        Width_X2 = string.IsNullOrEmpty(columns[16]) ? 0 : Convert.ToDouble(columns[16]),
                        Width_Y2 = string.IsNullOrEmpty(columns[17]) ? 0 : Convert.ToDouble(columns[17]),
                        Optime = string.IsNullOrEmpty(columns[18]) ? DateTime.Today : Convert.ToDateTime(columns[18]),


                    };

                    dataList.Add(hd);

                }

                
                return dataList;

            }

        }
    }
}
