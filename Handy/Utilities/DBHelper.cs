using Handy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handy.Utilities
{
    public static class DBHelper
    {
        private static string rootPath = @"C:\Handy\";
        private static object lockObject = new object();
        private static string currentFileName = string.Empty;

        private static StreamWriter currentFileStream = null;

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

                        string sHeaders = @"FileName@Name@Surname@Gender@Dominant@RightOrLeft@Thumb_X@Thumb_Y@Index_X@Index_Y@Middle_X@Middle_Y@Ring_X@Ring_Y@Little_X@Little_Y@Optime@P1@P2@P3@P4@P5@FS1@FS2@FS3@FS4@TL@IFL@MFL@RFL@LFL";

                        currentFileStream.WriteLine(sHeaders);
                        currentFileStream.Flush();
                        currentFileStream.Close();
                    }
                }
            }

        }


        public static void WriteDataFile(HandData hd)
        {


            string sFormat = "{0}";
            for (int i = 1; i < 31; i++)
            {
                sFormat += "@{" + i.ToString() + "}";
            }

            currentFileStream = new StreamWriter(rootPath + currentFileName + ".txt", true);


            currentFileStream.WriteLine(string.Format(sFormat,
                                                               hd.FileName,
                                                               hd.Name,
                                                               hd.Surname,
                                                               hd.Gender,
                                                               hd.Dominant,
                                                               hd.RightOrLeft,

                                                               hd.Thumb_X,
                                                               hd.Thumb_Y,

                                                               hd.Index_X,
                                                               hd.Index_Y,

                                                               hd.Middle_X,
                                                               hd.Middle_Y,

                                                               hd.Ring_X,
                                                               hd.Ring_Y,

                                                               hd.Little_X,
                                                               hd.Little_Y,

                                                               hd.Optime,

                                                               hd.P1,
                                                               hd.P2,
                                                               hd.P3,
                                                               hd.P4,
                                                               hd.P5,

                                                               hd.FS1,
                                                               hd.FS2,
                                                               hd.FS3,
                                                               hd.FS4,

                                                               hd.TL,
                                                               hd.IFL,
                                                               hd.MFL,
                                                               hd.RFL,
                                                               hd.LFL
           ));

            currentFileStream.Flush();
            currentFileStream.Close();

        }

        public static List<HandData> ReadDataFile(string fileName)
        {

            List<HandData> dataList = new List<HandData>();

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

                    HandData hd = new HandData()
                    {
                        FileName = columns[0],
                        Name = string.IsNullOrEmpty(columns[1]) ? "" : columns[1],
                        Surname = string.IsNullOrEmpty(columns[2]) ? "" : columns[2],
                        Gender = Convert.ToByte(columns[3]),
                        Dominant = Convert.ToByte(columns[4]),
                        RightOrLeft = Convert.ToByte(columns[5]),

                        Thumb_X = Convert.ToDouble(columns[6]),
                        Thumb_Y = Convert.ToDouble(columns[7]),

                        Index_X = Convert.ToDouble(columns[8]),
                        Index_Y = Convert.ToDouble(columns[9]),

                        Middle_X = Convert.ToDouble(columns[10]),
                        Middle_Y = Convert.ToDouble(columns[11]),

                        Ring_X = Convert.ToDouble(columns[12]),
                        Ring_Y = Convert.ToDouble(columns[13]),

                        Little_X = Convert.ToDouble(columns[14]),
                        Little_Y = Convert.ToDouble(columns[15]),

                        Optime = Convert.ToDateTime(columns[16]),

                        P1 = Convert.ToDouble(columns[17]),
                        P2 = Convert.ToDouble(columns[18]),
                        P3 = Convert.ToDouble(columns[19]),
                        P4 = Convert.ToDouble(columns[20]),
                        P5 = Convert.ToDouble(columns[21]),

                        FS1 = Convert.ToDouble(columns[22]),
                        FS2 = Convert.ToDouble(columns[23]),
                        FS3 = Convert.ToDouble(columns[24]),
                        FS4 = Convert.ToDouble(columns[25]),

                        TL = Convert.ToDouble(columns[26]),
                        IFL = Convert.ToDouble(columns[27]),
                        MFL = Convert.ToDouble(columns[28]),
                        RFL = Convert.ToDouble(columns[29]),
                        LFL = Convert.ToDouble(columns[30])

                    };

                    dataList.Add(hd);

                }

                
                return dataList;

            }

        }
    }
}
