using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Footart.Models;
using Footart.Utilities;
using FootArt.Data.Entities;
using FootArt.Data;
using System.Runtime.Remoting.Contexts;
using Footart.Data;
using System.Data.Entity;

namespace Footart
{
    public enum EnumMouseMode
    {
        Idle = 0,
        Zoom = 1,
        Ratio = 2,
        FootWidth = 3,
        LAAb = 4,
        LAAs = 5,
        Calcaneal = 6,
        QAngle = 7,
        FaceRatio = 8,
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static List<FootData> _HandDataList;

        private static List<StudyData> _FootDataList;
        private static List<FaceRatioData> _FaceRatioDataList;

        public static List<FootData> HandDataList
        {
            get { return MainWindow._HandDataList; }
            set { MainWindow._HandDataList = value; }
        }

        public static List<FaceRatioData> FaceRatioDataList
        {
            get { return MainWindow._FaceRatioDataList; }
            set { MainWindow._FaceRatioDataList = value; }
        }
        public static List<StudyData> FootDataList
        {
            get { return MainWindow._FootDataList; }
            set { MainWindow._FootDataList = value; }
        }

        public static int POINT_THICKNESS = 5;
        public static SolidColorBrush ACTIVE_LINE_COLOR = Brushes.IndianRed;
        public static SolidColorBrush RATIO_LINE_COLOR = Brushes.Blue;
        public static SolidColorBrush PASSIVE_LINE_COLOR = Brushes.Gray;
        public static SolidColorBrush ACTIVE_POINT_STROKE_COLOR = Brushes.IndianRed;
        public static SolidColorBrush PASSIVE_POINT_STROKE_COLOR = Brushes.WhiteSmoke;
        //public static SolidColorBrush POINT_FORECOLOR = Brushes.IndianRed;


        public static EnumMouseMode MouseMode = EnumMouseMode.Idle;

        public static string activePath = "";
        public static string activeFilePath = "";
        public static string activeFile = "";
        public static string excelFileName = "";
        public static string connectionString = "";
        public static List<Polygon> oCurrentHandPolygons;
        public static List<Line> oCurrentHandIRlines;
        public static Polygon oCurrentPolygon;
        public static Line oTempIRLine;
        public static Line oRatioLine;
        public static Line oTempHandLine;
        public static Point kesisimNoktasi;

        public static Polyline oCurrentRatio;
        public static List<Point> FootwidthPointList;
        public static List<Point> LAAbPointList;
        public static List<Point> LAAsPointList;
        public static List<Point> CalcanealPointList;
        public static List<Point> QAnglelPointList;
        public static List<Point> FaceRatioPointList;

        //public static Double[] Alanlar = new Double[5];

        public static double imgHeight = 0;
        public static double imgWidth = 0;
        public static double imgHeightScaleRate = 0;
        public static double imgWidthScaleRate = 0;
        public SolidColorBrush[] Renkler = new SolidColorBrush[5];
        BitmapImage myBitmapImage = new BitmapImage();
        Model model;

        double dleftMargin = 0;
        double dRatio = 1;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);// "E:\\perim\\ayaklar";
        string projectName = "Proje1";
        string settingsFile = "FootartSettings";

        public static List<string> oHandPointsLabels;
        public static List<string> oIRPointsLabels;
        public static List<TextBlock> LabelList;

        public MainWindow()
        {
            InitializeComponent();
            InitialSettings();

            model = new Model(this);
            model.Path = path;
            DataContext = model;

            FileToIconConverter fic = this.Resources["fic"] as FileToIconConverter;
            fic.DefaultSize = 100;
            list.AddHandler(ListViewItem.PreviewMouseDownEvent, new MouseButtonEventHandler(list_MouseDown));


            //oCurrentRatio.MouseLeftButtonDown += new MouseButtonEventHandler(imgAltlik_MouseLeftButtonDown);



            oHandPointsLabels = new List<string>();
            oHandPointsLabels.Add("P1");
            oHandPointsLabels.Add("P2");
            oHandPointsLabels.Add("P3");
            oHandPointsLabels.Add("P4");
            oHandPointsLabels.Add("P5");
            oHandPointsLabels.Add("P6");

            oIRPointsLabels = new List<string>();
            oIRPointsLabels.Add("P1");
            oIRPointsLabels.Add("P2");
            oIRPointsLabels.Add("P3");
            oIRPointsLabels.Add("P4");
            oIRPointsLabels.Add("P5");
            oIRPointsLabels.Add("P6");

            Renkler[0] = Brushes.Blue;
            Renkler[1] = Brushes.Red;
            Renkler[2] = Brushes.Yellow;
            Renkler[3] = Brushes.Green;
            Renkler[4] = Brushes.Olive;



            _HandDataList = new List<FootData>();
            if (!string.IsNullOrEmpty(model.Path))
                DBHelper.CreateDataFile(model.Path + "\\", projectName);

            InitializeFootData();


        }

        private void InitialSettings()
        {
            string currentPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (!System.IO.File.Exists(currentPath + "\\" + settingsFile + ".txt"))
            {

                StreamWriter currentFileStream = new StreamWriter(currentPath + "\\" + settingsFile + ".txt", true);

                currentFileStream.WriteLine(path);
                currentFileStream.Flush();
                currentFileStream.Close();
            }
            else
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(currentPath + "\\" + settingsFile + ".txt"))
                {

                    while (true)
                    {
                        path = sr.ReadLine();
                        break;
                    }
                }
            }
        }

        private void InitializeFootData()
        {
            dRatio = 1;
            txtRatio.Text = "";
            txtLength.Text = "";
            btnFoot.IsEnabled = false;
            txbSurname.Text = "";
            txbName.Text = "";


            txbCalcanealValue.Text = "";
            txlLAA.Text = "";
            txlQAngle.Text = "";
            txtFaceRatioVal.Text = "";

            oCurrentRatio = new Polyline();
            FootwidthPointList = new List<Point>();
            LAAbPointList = new List<Point>();
            LAAsPointList = new List<Point>();
            CalcanealPointList = new List<Point>();
            QAnglelPointList = new List<Point>();
            FaceRatioPointList = new List<Point>();
            LabelList = new List<TextBlock>();

            oCurrentHandPolygons = new List<Polygon>() { new Polygon(), new Polygon(), new Polygon(), new Polygon(), new Polygon() };


            RefreshGrid();


        }



        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            for (int i = cnvImage.Children.Count - 1; i >= 0; i--)
            {
                if (cnvImage.Children[i] is Line || cnvImage.Children[i] is Ellipse || cnvImage.Children[i] is TextBlock || cnvImage.Children[i] is Polygon)
                {
                    cnvImage.Children.Remove(cnvImage.Children[i]);

                }

            }

            InitializeFootData();


            activeFile = ((Image)sender).Tag.ToString(); ;

            if (activeFile.EndsWith(".jpeg") || activeFile.EndsWith(".jpg") || activeFile.EndsWith(".png") || activeFile.EndsWith(".gif"))
            {
                activeFilePath = model.Path + "\\" + activeFile;
                activePath = model.Path + "\\";
                txlFileName.Text = activeFile;
                UploadImage(activeFilePath);
                Canvas.SetLeft(imgCurrent, dleftMargin);
                txtActiveFile.Text = activeFile;
            }
            else
            {
                //uyarı göster

            }
        }

        private void UploadImage(string path)
        {

            myBitmapImage = new BitmapImage(new Uri(path));
            int imgWidth = myBitmapImage.PixelWidth;
            int imgHeight = myBitmapImage.PixelHeight;
            double imgHeightScaleRate = myBitmapImage.PixelHeight / myBitmapImage.Height;
            double imgWidthScaleRate = myBitmapImage.PixelWidth / myBitmapImage.Width;
            //set image source
            imgCurrent.Width = gridMain.ColumnDefinitions[1].ActualWidth;
            imgCurrent.Height = gridMain.RowDefinitions[2].ActualHeight;
            imgCurrent.Source = myBitmapImage;
            cnvImage.Height = imgCurrent.Height;
            cnvImage.Width = imgCurrent.Width;

            txtTitle.Text = path.Split('\\').Last().Split(' ').First();


        }

        private void imgAltlik_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (imgCurrent.LayoutTransform is ScaleTransform)
            {
                var st = (ScaleTransform)imgCurrent.LayoutTransform;
                double zoom = e.Delta > 0 ? .01 : -.01;
                st.ScaleX += zoom;
                st.ScaleY += zoom;
            }
        }

        private void imgAltlik_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point p = e.GetPosition(cnvImage);

            if (MouseMode == EnumMouseMode.Ratio)
            {

                if (oCurrentRatio.Points.Count != 2)
                {
                    oCurrentRatio.Points.Add(p);
                    DrawPoint(p, RATIO_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }
                RatioChanged(sender, null);
            }

            if (MouseMode == EnumMouseMode.FootWidth)
            {

                if (FootwidthPointList.Count != 2)
                {
                    FootwidthPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }

            if (MouseMode == EnumMouseMode.LAAb)
            {

                if (LAAbPointList.Count != 5)
                {
                    LAAbPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }

            if (MouseMode == EnumMouseMode.LAAs)
            {

                if (LAAsPointList.Count != 3)
                {
                    LAAsPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }


            if (MouseMode == EnumMouseMode.Calcaneal)
            {
                //if (CalcanealPointList.Count == 1)
                //{
                //    p.X = CalcanealPointList[0].X;
                //}

                if (CalcanealPointList.Count != 4)
                {
                    CalcanealPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }


            if (MouseMode == EnumMouseMode.QAngle)
            {
                //if (QAnglelPointList.Count == 1)
                //{
                //    p.X = QAnglelPointList[0].X;
                //}

                if (QAnglelPointList.Count != 4)
                {
                    QAnglelPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }
            if (MouseMode == EnumMouseMode.FaceRatio)
            {
                //if (QAnglelPointList.Count == 1)
                //{
                //    p.X = QAnglelPointList[0].X;
                //}

                if (FaceRatioPointList.Count != 4)
                {
                    FaceRatioPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }



        }

        private void RefreshPointLabels()
        {

            for (int i = 0; i < LabelList.Count - 1; i++)
            {
                cnvImage.Children.Remove(LabelList[i]);
            }

            LabelList.Clear();

            if (MouseMode == EnumMouseMode.FootWidth && FootwidthPointList.Count > 0)
            {
                for (int i = 0; i <= FootwidthPointList.Count - 1; i++)
                {

                    TextBlock txt = new TextBlock() { Text = oHandPointsLabels[i], Foreground = Brushes.Black, FontSize = 16 };

                    Canvas.SetTop(txt, FootwidthPointList[i].Y);
                    Canvas.SetLeft(txt, FootwidthPointList[i].X);
                    cnvImage.Children.Add(txt);
                    LabelList.Add(txt);
                }



            }

            if (MouseMode == EnumMouseMode.LAAb && LAAbPointList.Count > 0)
            {
                for (int i = 0; i <= LAAbPointList.Count - 1; i++)
                {



                    TextBlock txt = new TextBlock() { Text = oIRPointsLabels[i], Foreground = Brushes.Black, FontSize = 16 };

                    Canvas.SetTop(txt, LAAbPointList[i].Y);
                    Canvas.SetLeft(txt, LAAbPointList[i].X);
                    cnvImage.Children.Add(txt);
                    LabelList.Add(txt);
                }



            }

            if (MouseMode == EnumMouseMode.LAAs && LAAsPointList.Count > 0)
            {
                for (int i = 0; i <= LAAsPointList.Count - 1; i++)
                {



                    TextBlock txt = new TextBlock() { Text = oIRPointsLabels[i], Foreground = Brushes.Black, FontSize = 16 };

                    Canvas.SetTop(txt, LAAsPointList[i].Y);
                    Canvas.SetLeft(txt, LAAsPointList[i].X);
                    cnvImage.Children.Add(txt);
                    LabelList.Add(txt);
                }



            }




        }

        private void imgAltlik_MouseMove(object sender, MouseEventArgs e)
        {

            Point p = e.GetPosition(cnvImage);
            txbCursor.Text = string.Format("{0};{1}", (int)p.X, (int)p.Y);

            if (MouseMode == EnumMouseMode.FootWidth)
            {

                if (FootwidthPointList.Count > 0 && FootwidthPointList.Count < 2)
                {
                    if (oTempHandLine != null)
                        cnvImage.Children.Remove(oTempHandLine);

                    Point lastPoint = FootwidthPointList[FootwidthPointList.Count - 1];
                    oTempHandLine = new Line();
                    DrawLine(oTempHandLine, lastPoint, p, ACTIVE_LINE_COLOR);
                }

            }
            else if (MouseMode == EnumMouseMode.Ratio)
            {
                if (oCurrentRatio.Points.Count == 1)
                {
                    if (oRatioLine != null)
                        cnvImage.Children.Remove(oRatioLine);

                    oRatioLine = new Line();
                    DrawLine(oRatioLine, oCurrentRatio.Points[0], p, RATIO_LINE_COLOR);
                }




            }
            else if (MouseMode == EnumMouseMode.LAAb)
            {
                if (LAAbPointList.Count > 0 && LAAbPointList.Count < 5)
                {
                    if (oTempIRLine != null)
                        cnvImage.Children.Remove(oTempIRLine);

                    Point lastPoint = LAAbPointList[LAAbPointList.Count - 1];
                    oTempIRLine = new Line();

                    if (LAAbPointList.Count() != 2)
                        DrawLine(oTempIRLine, lastPoint, p, ACTIVE_LINE_COLOR);
                }
            }
            else if (MouseMode == EnumMouseMode.LAAs)
            {
                if (LAAsPointList.Count > 0 && LAAsPointList.Count < 3)
                {
                    if (oTempIRLine != null)
                        cnvImage.Children.Remove(oTempIRLine);

                    Point lastPoint = LAAsPointList[LAAsPointList.Count - 1];
                    oTempIRLine = new Line();

                    //if (LAAsPointList.Count() % 2 != 0)
                    DrawLine(oTempIRLine, lastPoint, p, ACTIVE_LINE_COLOR);
                }
            }
            else if (MouseMode == EnumMouseMode.Calcaneal)
            {
                if (CalcanealPointList.Count > 0 && CalcanealPointList.Count < 4)
                {
                    if (oTempIRLine != null)
                        cnvImage.Children.Remove(oTempIRLine);

                    Point lastPoint = CalcanealPointList[CalcanealPointList.Count - 1];
                    oTempIRLine = new Line();

                    //if (CalcanealPointList.Count == 1)
                    //    p.X = CalcanealPointList[0].X;

                    if (CalcanealPointList.Count() % 2 != 0)
                        DrawLine(oTempIRLine, lastPoint, p, ACTIVE_LINE_COLOR);

                    if (CalcanealPointList.Count() == 3)
                    {
                        kesisimNoktasi = FindIntersection(CalcanealPointList[0], CalcanealPointList[1], CalcanealPointList[2], p);


                    }
                }
            }
            else if (MouseMode == EnumMouseMode.QAngle)
            {
                if (QAnglelPointList.Count > 0 && QAnglelPointList.Count < 4)
                {
                    if (oTempIRLine != null)
                        cnvImage.Children.Remove(oTempIRLine);

                    Point lastPoint = QAnglelPointList[QAnglelPointList.Count - 1];
                    oTempIRLine = new Line();

                    //if (QAnglelPointList.Count == 1)
                    //    p.X = QAnglelPointList[0].X;

                    if (QAnglelPointList.Count() % 2 != 0)
                        DrawLine(oTempIRLine, lastPoint, p, ACTIVE_LINE_COLOR);

                    if (QAnglelPointList.Count() == 3)
                    {
                        kesisimNoktasi = FindIntersection(QAnglelPointList[0], QAnglelPointList[1], QAnglelPointList[2], p);


                    }
                }
            }
            else if (MouseMode == EnumMouseMode.FaceRatio)
            {
                if (FaceRatioPointList.Count > 0 && FaceRatioPointList.Count < 4)
                {
                    if (oTempIRLine != null)
                        cnvImage.Children.Remove(oTempIRLine);

                    Point lastPoint = FaceRatioPointList[FaceRatioPointList.Count - 1];
                    oTempIRLine = new Line();

                    //if (FaceRatioPointList.Count == 1)
                    //{
                    //    p.X = FaceRatioPointList[0].X;
                    //}

                    //if (FaceRatioPointList.Count == 3)
                    //{
                    //    p.Y = FaceRatioPointList[2].Y;
                    //}

                    if (FaceRatioPointList.Count() % 2 != 0)
                        DrawLine(oTempIRLine, lastPoint, p, ACTIVE_LINE_COLOR);

                    if (FaceRatioPointList.Count() == 3)
                    {
                        kesisimNoktasi = FindIntersection(FaceRatioPointList[0], FaceRatioPointList[1], FaceRatioPointList[2], p);

                        double a = FaceRatioPointList[0].X - FaceRatioPointList[1].X;
                        double b = FaceRatioPointList[0].Y - FaceRatioPointList[1].Y;
                        double height = Math.Sqrt(a * a + b * b);

                        double c = FaceRatioPointList[2].X - p.X;
                        double d = FaceRatioPointList[2].Y - p.Y;
                        double width = Math.Sqrt(c * c + d * d);

                        dRatio =width  /height ;

                        txtFaceRatioVal.Text = dRatio.ToString("N2");


                    }
                }
            }





        }

        private void RefreshPolygons()
        {


            for (int i = cnvImage.Children.Count - 1; i >= 0; i--)
            {
                if (cnvImage.Children[i] is Line || cnvImage.Children[i] is Ellipse || cnvImage.Children[i] is TextBlock || cnvImage.Children[i] is Polygon)
                {


                    cnvImage.Children.Remove(cnvImage.Children[i]);

                }

            }


            if (MouseMode == EnumMouseMode.FootWidth)
            {


                for (int i = 0; i < FootwidthPointList.Count; i++)
                {
                    DrawPoint(FootwidthPointList[i], ACTIVE_LINE_COLOR);

                    if (i < FootwidthPointList.Count - 1)
                        DrawLine(new Line(), FootwidthPointList[i], FootwidthPointList[i + 1], ACTIVE_LINE_COLOR);
                }

                if (FootwidthPointList.Count == 2)
                {


                    //txlTL.Text = GetDistance(FootwidthPointList[0], FootwidthPointList[1]).ToString("N2");

                }

            }


            if (MouseMode == EnumMouseMode.LAAb)
            {


                for (int i = 0; i < LAAbPointList.Count; i++)
                {
                    DrawPoint(LAAbPointList[i], ACTIVE_LINE_COLOR);

                    SolidColorBrush lineColor = ACTIVE_LINE_COLOR;
                    if (i == 0)
                        lineColor = RATIO_LINE_COLOR;

                    if (i == 1)
                        continue;

                    if (i < LAAbPointList.Count - 1)
                        DrawLine(new Line(), LAAbPointList[i], LAAbPointList[i + 1], lineColor);
                }

                if (LAAbPointList.Count == 5)
                {

                    double dAngle = Convert.ToDouble(GetAngle(LAAbPointList[2], LAAbPointList[4], LAAbPointList[3]).ToString("N2"));
                    txlLAA.Text = dAngle.ToString();

                    TextBlock txt = new TextBlock();
                    txt.Text = dAngle.ToString();
                    txt.Foreground = Brushes.Black;
                    txt.FontSize = 24;
                    txt.FontWeight = FontWeights.Bold;


                    Canvas.SetTop(txt, LAAbPointList[3].Y + 20);
                    Canvas.SetLeft(txt, LAAbPointList[3].X + 20);
                    cnvImage.Children.Add(txt);

                }



            }

            if (MouseMode == EnumMouseMode.LAAs)
            {


                for (int i = 0; i < LAAsPointList.Count; i++)
                {
                    DrawPoint(LAAsPointList[i], ACTIVE_LINE_COLOR);

                    if (i < LAAsPointList.Count - 1)
                        DrawLine(new Line(), LAAsPointList[i], LAAsPointList[i + 1], ACTIVE_LINE_COLOR);
                }

                if (LAAsPointList.Count == 3)
                {


                    double dAngle = Convert.ToDouble(GetAngle(LAAsPointList[0], LAAsPointList[2], LAAsPointList[1]).ToString("N2"));
                    txlLAA.Text = dAngle.ToString();

                    TextBlock txt = new TextBlock();
                    txt.Text = dAngle.ToString();
                    txt.Foreground = Brushes.Black;
                    txt.FontSize = 24;
                    txt.FontWeight = FontWeights.Bold;


                    Canvas.SetTop(txt, LAAsPointList[1].Y);
                    Canvas.SetLeft(txt, LAAsPointList[1].X);
                    cnvImage.Children.Add(txt);

                }



            }


            if (MouseMode == EnumMouseMode.Calcaneal)
            {
                for (int i = 0; i < CalcanealPointList.Count; i++)
                {
                    DrawPoint(CalcanealPointList[i], RATIO_LINE_COLOR);

                    if (i < CalcanealPointList.Count - 1)
                        if (i != 1)
                            DrawLine(new Line(), CalcanealPointList[i], CalcanealPointList[i + 1], RATIO_LINE_COLOR);
                }

                if (CalcanealPointList.Count == 4)
                {
                    Point kesisimNoktasi = FindIntersection(CalcanealPointList[0], CalcanealPointList[1], CalcanealPointList[2], CalcanealPointList[3]);

                    double dAngle = Convert.ToDouble(GetAngle(CalcanealPointList[1], CalcanealPointList[3], kesisimNoktasi).ToString("N2"));


                    txtKesisimAngle.Text = dAngle.ToString();
                    txbCalcanealValue.Text = dAngle.ToString();

                    Canvas.SetTop(txtKesisimAngle, kesisimNoktasi.Y + 3);
                    Canvas.SetLeft(txtKesisimAngle, kesisimNoktasi.X + 3);

                }
            }

            if (MouseMode == EnumMouseMode.QAngle)
            {
                for (int i = 0; i < QAnglelPointList.Count; i++)
                {
                    DrawPoint(QAnglelPointList[i], RATIO_LINE_COLOR);

                    if (i < QAnglelPointList.Count - 1)
                        if (i != 1)
                            DrawLine(new Line(), QAnglelPointList[i], QAnglelPointList[i + 1], RATIO_LINE_COLOR);
                }

                if (QAnglelPointList.Count == 4)
                {
                    Point kesisimNoktasi = FindIntersection(QAnglelPointList[0], QAnglelPointList[1], QAnglelPointList[2], QAnglelPointList[3]);

                    double dAngle = Convert.ToDouble(GetAngle(QAnglelPointList[1], QAnglelPointList[3], kesisimNoktasi).ToString("N2"));

                    txlQAngle.Text = dAngle.ToString();

                    txtKesisimAngle.Text = dAngle.ToString();


                    Canvas.SetTop(txtKesisimAngle, kesisimNoktasi.Y + 3);
                    Canvas.SetLeft(txtKesisimAngle, kesisimNoktasi.X + 3);

                }
            }

            if (MouseMode == EnumMouseMode.FaceRatio)
            {
                for (int i = 0; i < FaceRatioPointList.Count; i++)
                {
                    DrawPoint(FaceRatioPointList[i], RATIO_LINE_COLOR);

                    if (i < FaceRatioPointList.Count - 1)
                        if (i != 1)
                            DrawLine(new Line(), FaceRatioPointList[i], FaceRatioPointList[i + 1], RATIO_LINE_COLOR);
                }

                if (FaceRatioPointList.Count == 4)
                {
                    Point kesisimNoktasi = FindIntersection(FaceRatioPointList[0], FaceRatioPointList[1], FaceRatioPointList[2], FaceRatioPointList[3]);

                    double dAngle = Convert.ToDouble(GetAngle(FaceRatioPointList[1], FaceRatioPointList[3], kesisimNoktasi).ToString("N2"));

                    txlQAngle.Text = dAngle.ToString();

                    txtKesisimAngle.Text = dAngle.ToString() + "°";


                    double a = FaceRatioPointList[0].X - FaceRatioPointList[1].X;
                    double b = FaceRatioPointList[0].Y - FaceRatioPointList[1].Y;
                    double height = Math.Sqrt(a * a + b * b);

                    double c = FaceRatioPointList[2].X - FaceRatioPointList[3].X;
                    double d = FaceRatioPointList[2].Y - FaceRatioPointList[3].Y;
                    double width = Math.Sqrt(c * c + d * d);

                    dRatio =width  /height ;

                    txtFaceRatioVal.Text = dRatio.ToString("N2");


                    Canvas.SetTop(txtKesisimAngle, kesisimNoktasi.Y + 3);
                    Canvas.SetLeft(txtKesisimAngle, kesisimNoktasi.X + 3);

                }
            }

            foreach (var p in oCurrentRatio.Points)
            {
                DrawPoint(p, RATIO_LINE_COLOR);
            }

            if (oCurrentRatio.Points.Count == 2)
            {
                DrawLine(new Line(), oCurrentRatio.Points[0], oCurrentRatio.Points[1], RATIO_LINE_COLOR);

            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is Guid)
            {
                Guid id = new Guid(((Button)sender).Tag.ToString());

                using (var db = new AppDbContext())
                {
                    var studyData = db.StudyData.FirstOrDefault(x => x.Id == id);
                    if (studyData != null)
                    {
                        db.StudyData.Remove(studyData);
                        db.SaveChanges();
                    }
                }

                RefreshGrid();
            }
        }

        private void DeleteFaceRatio_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is Guid)
            {
                Guid id = new Guid(((Button)sender).Tag.ToString());

                using (var db = new AppDbContext())
                {
                    var faceData = db.FaceRatioData.FirstOrDefault(x => x.Id == id);
                    if (faceData != null)
                    {
                        db.FaceRatioData.Remove(faceData);
                        db.SaveChanges();
                    }
                }

                RefreshGrid();
            }
        }

        private double PolygonArea(Polygon p)
        {
            // Add the first point to the end.

            int num_points = p.Points.Count;
            Point[] pts = new Point[num_points + 1];
            p.Points.CopyTo(pts, 0);

            pts[num_points] = p.Points[0];

            // Get the areas.
            double area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area += ((pts[i + 1].X - pts[i].X) * dRatio) * ((pts[i + 1].Y + pts[i].Y) * dRatio) / 2;
            }

            // Return the result.

            area = Math.Abs(area);

            return area;
        }

        public double GetDistance(Point p, Point q)
        {
            double a = (p.X - q.X) * dRatio;
            double b = (p.Y - q.Y) * dRatio;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        private void DrawPoint(Point pntToBeDrawed, SolidColorBrush pForeColor)
        {


            Point canvasPosition = new Point(pntToBeDrawed.X - 5, pntToBeDrawed.Y - 5);
            SolidColorBrush fore = pForeColor;
            SolidColorBrush back = PASSIVE_POINT_STROKE_COLOR;

            //nokta ciziliyor
            Ellipse el = new Ellipse();
            Canvas.SetLeft(el, canvasPosition.X + 3);
            Canvas.SetTop(el, canvasPosition.Y + 3);
            el.Width = 3;
            el.Height = 3;
            el.Stroke = fore;
            el.Fill = back;
            el.StrokeThickness = 2;
            cnvImage.Children.Add(el);


        }

        private void DrawLine(Line canvasLine, Point pntFrom, Point pntTo, Brush pBrush)
        {

            #region cizgi ciziyor

            canvasLine.MouseLeftButtonDown += imgAltlik_MouseLeftButtonDown;
            canvasLine.X1 = pntFrom.X;
            canvasLine.Y1 = pntFrom.Y;
            canvasLine.X2 = pntTo.X;
            canvasLine.Y2 = pntTo.Y;

            canvasLine.Fill = pBrush;
            canvasLine.Stroke = pBrush;
            canvasLine.StrokeThickness = 1;



            cnvImage.Children.Add(canvasLine);


            #endregion

        }

        private void cokgen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            oCurrentPolygon = (Polygon)sender;
        }

        private double GetAngle(Point p0, Point p1, Point pCenter)
        {
            var p0c = Math.Sqrt(Math.Pow(pCenter.X - p0.X, 2) + Math.Pow(pCenter.Y - p0.Y, 2));

            var p1c = Math.Sqrt(Math.Pow(pCenter.X - p1.X, 2) + Math.Pow(pCenter.Y - p1.Y, 2));
            var p0p1 = Math.Sqrt(Math.Pow(p1.X - p0.X, 2) + Math.Pow(p1.Y - p0.Y, 2));
            //1 radian = 57.2957795
            return Math.Acos((p1c * p1c + p0c * p0c - p0p1 * p0p1) / (2 * p1c * p0c)) * 57.2957795;
        }

        public static Ellipse kesisimNoktasiEllipse = new Ellipse();
        public static TextBlock txtKesisimAngle = new TextBlock();

        private Point FindIntersection(Point s1, Point e1, Point s2, Point e2)
        {



            double a1 = e1.Y - s1.Y;
            double b1 = s1.X - e1.X;
            double c1 = a1 * s1.X + b1 * s1.Y;

            double a2 = e2.Y - s2.Y;
            double b2 = s2.X - e2.X;
            double c2 = a2 * s2.X + b2 * s2.Y;

            double delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be (NaN, NaN).

            Point kesisim = new Point();
            if (cnvImage.Children.Contains(kesisimNoktasiEllipse))
                cnvImage.Children.Remove(kesisimNoktasiEllipse);

            if (cnvImage.Children.Contains(txtKesisimAngle))
                cnvImage.Children.Remove(txtKesisimAngle);

            if (delta != 0)
                kesisim = new Point((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

            Point canvasPosition = new Point(kesisim.X - 5, kesisim.Y - 5);
            SolidColorBrush fore = ACTIVE_LINE_COLOR;
            SolidColorBrush back = PASSIVE_POINT_STROKE_COLOR;

            //nokta ciziliyor

            Canvas.SetLeft(kesisimNoktasiEllipse, canvasPosition.X + 3);
            Canvas.SetTop(kesisimNoktasiEllipse, canvasPosition.Y + 3);
            kesisimNoktasiEllipse.Width = 3;
            kesisimNoktasiEllipse.Height = 3;
            kesisimNoktasiEllipse.Stroke = fore;
            kesisimNoktasiEllipse.Fill = back;
            kesisimNoktasiEllipse.StrokeThickness = 2;
            cnvImage.Children.Add(kesisimNoktasiEllipse);

            double dAngle = Convert.ToDouble(GetAngle(e1, e2, kesisimNoktasi).ToString("N2"));


            txtKesisimAngle.Text = dAngle.ToString() + "°";
            txtKesisimAngle.Foreground = Brushes.Black;
            txtKesisimAngle.FontSize = 24;
            txtKesisimAngle.FontWeight = FontWeights.Bold;


            Canvas.SetTop(txtKesisimAngle, kesisimNoktasi.Y + 3);
            Canvas.SetLeft(txtKesisimAngle, kesisimNoktasi.X + 3);
            cnvImage.Children.Add(txtKesisimAngle);

            return kesisim;
        }

        private void list_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (list.SelectedValue is string)
                {
                    string dir = list.SelectedValue as string;
                    if (Directory.Exists(dir))
                    {
                        model.Path = dir;
                        e.Handled = true;
                    }
                }
            }
        }

        public void ClearCache()
        {
            FileToIconConverter fic = this.Resources["fic"] as FileToIconConverter;
            //Clear Thumbnail only, icon is not cleared.
            fic.ClearInstanceCache();
        }

        private void btnAddLeftHand_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddRightHand_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMode_Click(object sender, RoutedEventArgs e)
        {
            MouseMode = (EnumMouseMode)Convert.ToByte(((Button)sender).DataContext);

            if (MouseMode == EnumMouseMode.Idle)
            {
                oCurrentRatio = new Polyline();
                FootwidthPointList = new List<Point>();
                LAAbPointList = new List<Point>();
                LAAsPointList = new List<Point>();
                CalcanealPointList = new List<Point>();
                QAnglelPointList = new List<Point>();
                LabelList = new List<TextBlock>();

                oCurrentHandPolygons = new List<Polygon>() { new Polygon(), new Polygon(), new Polygon(), new Polygon(), new Polygon() };

                RefreshPolygons();
                RefreshPointLabels();
            }

            Button clickedButton = (Button)sender;

            var toolBar = (ToolBar)clickedButton.Parent;

            foreach (var item in toolBar.Items)
            {
                if (item is Button button)
                {
                    button.ClearValue(Button.BackgroundProperty);
                }
            }


            clickedButton.Background = Brushes.OrangeRed; // Highlight color

            //if (MouseMode == EnumMouseMode.Hand)
            //    this.Cursor = Cursors.Hand;
            //else if (MouseMode == EnumMouseMode.Ratio)
            //    this.Cursor = Cursors.SizeWE;
            //else
            //    this.Cursor = Cursors.Arrow;

            RefreshGrid();


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddHand_ClickOld(object sender, RoutedEventArgs e)
        {
            if (!System.IO.File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + settingsFile + ".txt"))
            {

                StreamWriter currentFileStream = new StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + settingsFile + ".txt", true);

                currentFileStream.WriteLine(model.Path);
                currentFileStream.Flush();
                currentFileStream.Close();
            }

            FootData hd = new FootData()
            {
                FileName = txlFileName.Text,
                Name = txtName.Text,
                Surname = txtSurname.Text,
                Gender = (byte)(rbMale.IsChecked.Value == true ? 1 : 0),
                SideOrBack = (byte)EnumSideOrBack.Width,
                RightOrLeft = (byte)(rbHandR.IsChecked.Value == true ? (byte)EnumRightOrLeft.Right : (byte)EnumRightOrLeft.Left),//R=1;L=0
                Optime = DateTime.Now,

            };

            if (MouseMode == EnumMouseMode.FootWidth)
            {

                if (FootwidthPointList.Count == 0)
                    return;



                hd.Width = Convert.ToDouble(GetDistance(FootwidthPointList[0], FootwidthPointList[1]).ToString("N2"));
                hd.Width_X1 = FootwidthPointList[0].X;
                hd.Width_Y1 = FootwidthPointList[0].Y;
                hd.Width_X2 = FootwidthPointList[1].X;
                hd.Width_Y2 = FootwidthPointList[1].Y;








            }
            else if (MouseMode == EnumMouseMode.LAAb)
            {
                if (LAAbPointList.Count == 0)
                    return;



                hd.LAA = Convert.ToDouble(GetAngle(LAAbPointList[2], LAAbPointList[4], LAAbPointList[3]).ToString("N2"));
                hd.LAA_X1 = LAAbPointList[2].X;
                hd.LAA_Y1 = LAAbPointList[2].Y;
                hd.LAA_X2 = LAAbPointList[3].X;
                hd.LAA_Y2 = LAAbPointList[3].Y;
                hd.LAA_X3 = LAAbPointList[4].X;
                hd.LAA_Y3 = LAAbPointList[4].Y;






            }
            else if (MouseMode == EnumMouseMode.LAAs)
            {
                if (LAAsPointList.Count == 0)
                    return;



                hd.LAA = Convert.ToDouble(GetAngle(LAAsPointList[0], LAAsPointList[2], LAAsPointList[1]).ToString("N2"));
                hd.LAA_X1 = LAAsPointList[0].X;
                hd.LAA_Y1 = LAAsPointList[0].Y;
                hd.LAA_X2 = LAAsPointList[1].X;
                hd.LAA_Y2 = LAAsPointList[1].Y;
                hd.LAA_X3 = LAAsPointList[2].X;
                hd.LAA_Y3 = LAAsPointList[2].Y;





            }
            else if (MouseMode == EnumMouseMode.Calcaneal)
            {
                if (CalcanealPointList.Count != 4)
                    return;

                Point kesisimNoktasi = FindIntersection(CalcanealPointList[0], CalcanealPointList[1], CalcanealPointList[2], CalcanealPointList[3]);

                double dAngle = Convert.ToDouble(GetAngle(CalcanealPointList[1], CalcanealPointList[3], kesisimNoktasi).ToString("N2"));


                hd.Calcaneal = dAngle;
                hd.Calcaneal_V_X1 = CalcanealPointList[0].X;
                hd.Calcaneal_V_Y1 = CalcanealPointList[0].Y;
                hd.Calcaneal_V_X2 = CalcanealPointList[1].X;
                hd.Calcaneal_V_Y2 = CalcanealPointList[1].Y;
                hd.Calcaneal_X1 = CalcanealPointList[2].X;
                hd.Calcaneal_Y1 = CalcanealPointList[2].Y;
                hd.Calcaneal_X2 = CalcanealPointList[3].X;
                hd.Calcaneal_Y2 = CalcanealPointList[3].Y;



            }
            else if (MouseMode == EnumMouseMode.QAngle)
            {
                if (QAnglelPointList.Count != 4)
                    return;

                Point kesisimNoktasi = FindIntersection(QAnglelPointList[0], QAnglelPointList[1], QAnglelPointList[2], QAnglelPointList[3]);

                double dAngle = Convert.ToDouble(GetAngle(QAnglelPointList[1], QAnglelPointList[3], kesisimNoktasi).ToString("N2"));

                if (hd.RightOrLeft == 1)
                {
                    hd.QAngle_Left = dAngle;
                    hd.QAngle_Left_V_X1 = QAnglelPointList[0].X;
                    hd.QAngle_Left_V_Y1 = QAnglelPointList[0].Y;
                    hd.QAngle_Left_V_X2 = QAnglelPointList[1].X;
                    hd.QAngle_Left_V_Y2 = QAnglelPointList[1].Y;
                    hd.QAngle_Left_X1 = QAnglelPointList[2].X;
                    hd.QAngle_Left_Y1 = QAnglelPointList[2].Y;
                    hd.QAngle_Left_X2 = QAnglelPointList[3].X;
                    hd.QAngle_Left_Y2 = QAnglelPointList[3].Y;
                }
                else
                {
                    hd.QAngle_Right = dAngle;
                    hd.QAngle_Right_V_X1 = QAnglelPointList[0].X;
                    hd.QAngle_Right_V_Y1 = QAnglelPointList[0].Y;
                    hd.QAngle_Right_V_X2 = QAnglelPointList[1].X;
                    hd.QAngle_Right_V_Y2 = QAnglelPointList[1].Y;
                    hd.QAngle_Right_X1 = QAnglelPointList[2].X;
                    hd.QAngle_Right_Y1 = QAnglelPointList[2].Y;
                    hd.QAngle_Right_X2 = QAnglelPointList[3].X;
                    hd.QAngle_Right_Y2 = QAnglelPointList[3].Y;

                }





            }

            FootData existstHD = _HandDataList.Where(x => x.FileName == hd.FileName).FirstOrDefault();
            if (existstHD != null)
                _HandDataList.Remove(existstHD);

            _HandDataList.Add(hd);

            DBHelper.WriteDataFile(hd);
            InitializeFootData();




        }

        private void btnAddHand_Click(object sender, RoutedEventArgs e)
        {
            Guid nwId = Guid.NewGuid();

            using (var context = new AppDbContext())
            {
                var side = rbHandR.IsChecked.Value == true ? "RIGHT" : "LEFT";
                StudyData hd = context.StudyData.Where(x => x.Title == txtTitle.Text && x.DataType == (int)MouseMode && x.Side == side).FirstOrDefault();

                if (hd is null)
                    hd = new StudyData()
                    {
                        FileName = txlFileName.Text,
                        Title = txtTitle.Text,
                        Gender = rbMale.IsChecked.Value == true ? "MALE" : "FEMALE",
                        Side = side,
                        Optime = DateTime.Now,
                        DataType = (int)MouseMode,
                        Id = nwId,


                    };



                if (MouseMode == EnumMouseMode.FootWidth)
                {

                    if (FootwidthPointList.Count == 0)
                        return;

                    hd.StudyDataValue = Convert.ToDouble(GetDistance(FootwidthPointList[0], FootwidthPointList[1]).ToString("N2"));
                    hd.StudyDataText = "Genişlik";
                    hd.StudyDataSource.Clear();
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = FootwidthPointList[0].X, Y = FootwidthPointList[0].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = FootwidthPointList[1].X, Y = FootwidthPointList[1].Y });

                }
                else if (MouseMode == EnumMouseMode.LAAb)
                {
                    if (LAAbPointList.Count == 0)
                        return;



                    hd.StudyDataValue = Convert.ToDouble(GetAngle(LAAbPointList[2], LAAbPointList[4], LAAbPointList[3]).ToString("N2"));
                    hd.StudyDataText = "LAAb";
                    hd.StudyDataSource.Clear();
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = LAAbPointList[2].X, Y = LAAbPointList[2].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = LAAbPointList[3].X, Y = LAAbPointList[3].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = LAAbPointList[4].X, Y = LAAbPointList[4].Y });

                }
                else if (MouseMode == EnumMouseMode.LAAs)
                {
                    if (LAAsPointList.Count == 0)
                        return;



                    hd.StudyDataValue = Convert.ToDouble(GetAngle(LAAsPointList[0], LAAsPointList[2], LAAsPointList[1]).ToString("N2"));
                    hd.StudyDataText = "LAAs";
                    hd.StudyDataSource.Clear();
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = LAAsPointList[0].X, Y = LAAsPointList[0].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = LAAsPointList[1].X, Y = LAAsPointList[1].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = LAAsPointList[2].X, Y = LAAsPointList[2].Y });





                }
                else if (MouseMode == EnumMouseMode.Calcaneal)
                {
                    if (CalcanealPointList.Count != 4)
                        return;

                    Point kesisimNoktasi = FindIntersection(CalcanealPointList[0], CalcanealPointList[1], CalcanealPointList[2], CalcanealPointList[3]);

                    double dAngle = Convert.ToDouble(GetAngle(CalcanealPointList[1], CalcanealPointList[3], kesisimNoktasi).ToString("N2"));


                    hd.StudyDataValue = dAngle;
                    hd.StudyDataText = "Calcaneal";
                    hd.StudyDataSource.Clear();
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = CalcanealPointList[0].X, Y = CalcanealPointList[0].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = CalcanealPointList[1].X, Y = CalcanealPointList[1].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = CalcanealPointList[2].X, Y = CalcanealPointList[2].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = CalcanealPointList[3].X, Y = CalcanealPointList[3].Y });



                }
                else if (MouseMode == EnumMouseMode.QAngle)
                {
                    if (QAnglelPointList.Count != 4)
                        return;

                    Point kesisimNoktasi = FindIntersection(QAnglelPointList[0], QAnglelPointList[1], QAnglelPointList[2], QAnglelPointList[3]);

                    double dAngle = Convert.ToDouble(GetAngle(QAnglelPointList[1], QAnglelPointList[3], kesisimNoktasi).ToString("N2"));


                    hd.StudyDataValue = dAngle;
                    hd.StudyDataText = "QAngle";
                    hd.StudyDataSource.Clear();
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = QAnglelPointList[0].X, Y = QAnglelPointList[0].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = QAnglelPointList[1].X, Y = QAnglelPointList[1].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = QAnglelPointList[2].X, Y = QAnglelPointList[2].Y });
                    hd.StudyDataSource.Add(new StudyDataPoint { StudyDataId = hd.Id, X = QAnglelPointList[3].X, Y = QAnglelPointList[3].Y });


                }


                //FootData existstHD = _HandDataList.Where(x => x.FileName == hd.FileName).FirstOrDefault();
                //if (existstHD != null)
                //    _HandDataList.Remove(existstHD);

                //_HandDataList.Add(hd);

                //DBHelper.WriteDataFile(hd);
                //InitializeHandData();


                // Yeni veri ekle
                if (hd.Id == nwId)
                    context.StudyData.Add(hd);

                context.SaveChanges();
                RefreshGrid();
                //// Verileri oku
                //var data = context.StudyData.ToList();
                //foreach (var item in data)
                //{
                //    Console.WriteLine($"Id: {item.Id}, Title: {item.Title}");
                //}
            }


        }

        private void btnAddFaceRatio_Click(object sender, RoutedEventArgs e)
        {
            Guid nwId = Guid.NewGuid();

            if (MouseMode != EnumMouseMode.FaceRatio)
                return;

            if (FaceRatioPointList.Count != 4)
                return;

            using (var context = new AppDbContext())
            {
                FaceRatioData fd = context.FaceRatioData.Where(x => x.Title == txtTitle.Text).FirstOrDefault();

                if (fd is null)
                    fd = new FaceRatioData() { Id = nwId , Title = txtTitle.Text };


                double a = FaceRatioPointList[0].X - FaceRatioPointList[1].X;
                double b = FaceRatioPointList[0].Y - FaceRatioPointList[1].Y;
                double height = Math.Sqrt(a * a + b * b);

                double c = FaceRatioPointList[2].X - FaceRatioPointList[3].X;
                double d = FaceRatioPointList[2].Y - FaceRatioPointList[3].Y;
                double width = Math.Sqrt(c * c + d * d);

                dRatio =  width / height;



                fd.FaceRatio = Convert.ToDecimal(dRatio);
                fd.Height = Convert.ToDecimal(height);
                fd.Optime = DateTime.Now;
                fd.Width = Convert.ToDecimal(width);

                if (fd.Id == nwId)
                    context.FaceRatioData.Add(fd);

                context.SaveChanges();
                RefreshGrid();

            }

        }
        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            List<StudyData> hd = new List<StudyData>();
            using (var context = new AppDbContext())
            {
                if (txtTitle.Text != "")
                    hd = context.StudyData.Where(x => x.Title == txtTitle.Text).ToList();
                else
                    hd = context.StudyData.ToList();

            }

            lvHandList.ItemsSource = hd.Select(x => new
            {
                x.Gender,
                x.Title,
                x.Optime,
                x.Id,
                x.FileName,
                x.StudyDataValue,
                x.StudyDataText,
                x.DataType,
                x.StudyDataSource,
                x.Side,
                TitleVal = x.Title.PadLeft(3, '0')
            }).OrderByDescending(x => x.TitleVal).ThenBy(x => x.StudyDataText);

            List<FaceRatioData> fd = new List<FaceRatioData>();
            using (var context = new AppDbContext())
            {

                fd = context.FaceRatioData.ToList();

            }

            lvFaceRatioList.ItemsSource = fd.Select(x => new
            {
                x.Title,
                x.Id,
                x.FaceRatio,
                x.Width,
                x.Height
            }).OrderByDescending(x => x.Title);

        }
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (MouseMode == EnumMouseMode.FootWidth)
            {
                if (FootwidthPointList.Count > 0)
                    FootwidthPointList.Remove(FootwidthPointList[FootwidthPointList.Count - 1]);
            }
            else if (MouseMode == EnumMouseMode.Ratio)
            {

                if (oCurrentRatio.Points.Count > 0)
                    oCurrentRatio.Points.Remove(oCurrentRatio.Points[oCurrentRatio.Points.Count - 1]);

            }
            else if (MouseMode == EnumMouseMode.LAAb)
            {

                if (LAAbPointList.Count > 0)
                    LAAbPointList.Remove(LAAbPointList[LAAbPointList.Count - 1]);

            }
            else if (MouseMode == EnumMouseMode.LAAs)
            {

                if (LAAsPointList.Count > 0)
                    LAAsPointList.Remove(LAAsPointList[LAAsPointList.Count - 1]);

            }
            else if (MouseMode == EnumMouseMode.Calcaneal)
            {

                if (CalcanealPointList.Count > 0)
                    CalcanealPointList.Remove(CalcanealPointList[CalcanealPointList.Count - 1]);

            }
            else if (MouseMode == EnumMouseMode.QAngle)
            {

                if (QAnglelPointList.Count > 0)
                    QAnglelPointList.Remove(QAnglelPointList[QAnglelPointList.Count - 1]);

            }
            else if (MouseMode == EnumMouseMode.FaceRatio)
            {

                if (FaceRatioPointList.Count > 0)
                    FaceRatioPointList.Remove(FaceRatioPointList[FaceRatioPointList.Count - 1]);

            }

            RefreshPolygons();
            RefreshPointLabels();
        }

        private void RatioChanged(object sender, TextChangedEventArgs e)
        {
            if (oCurrentRatio != null && oCurrentRatio.Points.Count == 2 && !string.IsNullOrEmpty(txtLength.Text))
            {
                double a = oCurrentRatio.Points[0].X - oCurrentRatio.Points[1].X;
                double b = oCurrentRatio.Points[0].Y - oCurrentRatio.Points[1].Y;
                double distance = Math.Sqrt(a * a + b * b);

                dRatio = double.Parse(txtLength.Text) / distance;
                txtRatio.Text = dRatio.ToString();

                btnFoot.IsEnabled = true;
            }
        }

        private void changeFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = model.Path;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                model.Path = dlg.SelectedPath;
                DBHelper.CreateDataFile(model.Path + "\\", projectName);
                InitializeFootData();
            }
        }


    }
}
