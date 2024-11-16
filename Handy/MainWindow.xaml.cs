using Handy.Models;
using Handy.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Handy
{
    public enum EnumMouseMode
    {
        Idle,
        Zoom,
        Ratio,
        Hand,
        IRFinger
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static List<HandData> _HandDataList;


        public static List<HandData> HandDataList
        {
            get { return MainWindow._HandDataList; }
            set { MainWindow._HandDataList = value; }
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

        public static Polyline oCurrentRatio;
        public static List<Point> HandPointList;
        public static List<Point> IRPointList;
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
        string path = "E:\\perim\\hands";
        string projectName = "Proje1";

        public static List<string> oHandPointsLabels;
        public static List<string> oIRPointsLabels;
        public static List<TextBlock> LabelList;

        public MainWindow()
        {
            InitializeComponent();

            model = new Model(this);
            model.Path = path;
            DataContext = model;

            FileToIconConverter fic = this.Resources["fic"] as FileToIconConverter;
            fic.DefaultSize = 100;
            list.AddHandler(ListViewItem.PreviewMouseDownEvent, new MouseButtonEventHandler(list_MouseDown));


            //oCurrentRatio.MouseLeftButtonDown += new MouseButtonEventHandler(imgAltlik_MouseLeftButtonDown);



            oHandPointsLabels = new List<string>();
            oHandPointsLabels.Add("T");
            oHandPointsLabels.Add("I");
            oHandPointsLabels.Add("M");
            oHandPointsLabels.Add("R");
            oHandPointsLabels.Add("L");
            oHandPointsLabels.Add("W");

            oIRPointsLabels = new List<string>();
            oIRPointsLabels.Add("I");
            oIRPointsLabels.Add("I_");
            oIRPointsLabels.Add("R");
            oIRPointsLabels.Add("R_");

            Renkler[0] = Brushes.Blue;
            Renkler[1] = Brushes.Red;
            Renkler[2] = Brushes.Yellow;
            Renkler[3] = Brushes.Green;
            Renkler[4] = Brushes.Olive;



            _HandDataList = new List<HandData>();
            if (!string.IsNullOrEmpty(model.Path))
                DBHelper.CreateDataFile(model.Path + "\\", projectName);

            InitializeHandData();


        }


        private void InitializeHandData()
        {
            dRatio = 1;
            txtRatio.Text = "";
            txtLength.Text = "";
            btnHand.IsEnabled = false;
            txbSurname.Text = "";
            txbName.Text = "";

            oCurrentRatio = new Polyline();
            HandPointList = new List<Point>();
            IRPointList = new List<Point>();
            LabelList = new List<TextBlock>();

            oCurrentHandPolygons = new List<Polygon>() { new Polygon(), new Polygon(), new Polygon(), new Polygon(), new Polygon() };
            if (!string.IsNullOrEmpty(model.Path))
                HandDataList = DBHelper.ReadDataFile(projectName);
            lvHandList.ItemsSource = HandDataList.OrderByDescending(x => x.Optime);

            txlP1.Text = "";
            txlFS1.Text = "";
            txlTL.Text = "";

            txlP2.Text = "";
            txlFS2.Text = "";
            txlIFL.Text = "";

            txlP3.Text = "";
            txlFS3.Text = "";
            txlMFL.Text = "";

            txlP4.Text = "";
            txlFS4.Text = "";
            txlRFL.Text = "";

            txlP5.Text = "";
            txlLFL.Text = "";

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

            InitializeHandData();


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

            if (MouseMode == EnumMouseMode.Hand)
            {

                if (HandPointList.Count != 6)
                {
                    HandPointList.Add(p);
                    DrawPoint(p, ACTIVE_LINE_COLOR);
                    RefreshPolygons();
                    RefreshPointLabels();
                }

            }

            if (MouseMode == EnumMouseMode.IRFinger)
            {

                if (IRPointList.Count != 4)
                {
                    IRPointList.Add(p);
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

            if (MouseMode == EnumMouseMode.Hand && HandPointList.Count > 0)
            {
                for (int i = 0; i <= HandPointList.Count - 1; i++)
                {

                    TextBlock txt = new TextBlock() { Text = oHandPointsLabels[i], Foreground = Brushes.Black, FontSize = 16 };

                    Canvas.SetTop(txt, HandPointList[i].Y);
                    Canvas.SetLeft(txt, HandPointList[i].X);
                    cnvImage.Children.Add(txt);
                    LabelList.Add(txt);
                }



            }

            if (MouseMode == EnumMouseMode.IRFinger && IRPointList.Count > 0)
            {
                for (int i = 0; i <= IRPointList.Count - 1; i++)
                {



                    TextBlock txt = new TextBlock() { Text = oIRPointsLabels[i], Foreground = Brushes.Black, FontSize = 16 };

                    Canvas.SetTop(txt, IRPointList[i].Y);
                    Canvas.SetLeft(txt, IRPointList[i].X);
                    cnvImage.Children.Add(txt);
                    LabelList.Add(txt);
                }



            }



        }

        private void imgAltlik_MouseMove(object sender, MouseEventArgs e)
        {

            Point p = e.GetPosition(cnvImage);
            txbCursor.Text = string.Format("{0};{1}", (int)p.X, (int)p.Y);

            if (MouseMode == EnumMouseMode.Hand)
            {

                if (HandPointList.Count > 0 && HandPointList.Count < 6)
                {
                    if (oTempHandLine != null)
                        cnvImage.Children.Remove(oTempHandLine);

                    Point lastPoint = HandPointList[HandPointList.Count - 1];
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
            else if (MouseMode == EnumMouseMode.IRFinger)
            {
                if (IRPointList.Count > 0 && IRPointList.Count < 4)
                {
                    if (oTempIRLine != null)
                        cnvImage.Children.Remove(oTempIRLine);

                    Point lastPoint = IRPointList[IRPointList.Count - 1];
                    oTempIRLine = new Line();

                    if (IRPointList.Count() % 2 != 0)
                        DrawLine(oTempIRLine, lastPoint, p, ACTIVE_LINE_COLOR);
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

            #region Hand
            if (MouseMode == EnumMouseMode.Hand)
            {


                for (int i = 0; i < HandPointList.Count; i++)
                {
                    DrawPoint(HandPointList[i], ACTIVE_LINE_COLOR);

                    if (i < HandPointList.Count - 1)
                        DrawLine(new Line(), HandPointList[i], HandPointList[i + 1], ACTIVE_LINE_COLOR);
                }

                if (HandPointList.Count == 6)
                {
                    DrawLine(new Line(), HandPointList[0], HandPointList[5], ACTIVE_LINE_COLOR);
                    oCurrentHandPolygons[0].Points.Clear();
                    oCurrentHandPolygons[1].Points.Clear();
                    oCurrentHandPolygons[2].Points.Clear();
                    oCurrentHandPolygons[3].Points.Clear();
                    oCurrentHandPolygons[4].Points.Clear();
                    //P1 = WTIW => P[5],P[0],P[1],P[5]
                    oCurrentHandPolygons[0].Points.Add(HandPointList[5]);
                    oCurrentHandPolygons[0].Points.Add(HandPointList[0]);
                    oCurrentHandPolygons[0].Points.Add(HandPointList[1]);
                    txlP1.Text = PolygonArea(oCurrentHandPolygons[0]).ToString("N2");
                    txlFS1.Text = GetDistance(HandPointList[0], HandPointList[1]).ToString("N2");
                    txlTL.Text = GetDistance(HandPointList[5], HandPointList[0]).ToString("N2");
                    //P2 = WTMW =>  P[5],P[0],P[2],P[5]
                    oCurrentHandPolygons[1].Points.Add(HandPointList[5]);
                    oCurrentHandPolygons[1].Points.Add(HandPointList[0]);
                    oCurrentHandPolygons[1].Points.Add(HandPointList[2]);
                    txlP2.Text = PolygonArea(oCurrentHandPolygons[1]).ToString("N2");
                    txlFS2.Text = GetDistance(HandPointList[0], HandPointList[2]).ToString("N2");
                    txlIFL.Text = GetDistance(HandPointList[5], HandPointList[1]).ToString("N2");
                    //P3 = WIMW =>  P[5],P[1],P[2],P[5]
                    oCurrentHandPolygons[2].Points.Add(HandPointList[5]);
                    oCurrentHandPolygons[2].Points.Add(HandPointList[1]);
                    oCurrentHandPolygons[2].Points.Add(HandPointList[2]);
                    txlP3.Text = PolygonArea(oCurrentHandPolygons[2]).ToString("N2");
                    txlFS3.Text = GetDistance(HandPointList[0], HandPointList[3]).ToString("N2");
                    txlMFL.Text = GetDistance(HandPointList[5], HandPointList[2]).ToString("N2");
                    //P4 = WMRLW =>  P[5],P[2],P[3],P[4],P[5]
                    oCurrentHandPolygons[3].Points.Add(HandPointList[5]);
                    oCurrentHandPolygons[3].Points.Add(HandPointList[2]);
                    oCurrentHandPolygons[3].Points.Add(HandPointList[3]);
                    oCurrentHandPolygons[3].Points.Add(HandPointList[4]);
                    txlP4.Text = PolygonArea(oCurrentHandPolygons[3]).ToString("N2");
                    txlFS4.Text = GetDistance(HandPointList[0], HandPointList[4]).ToString("N2");
                    txlRFL.Text = GetDistance(HandPointList[5], HandPointList[3]).ToString("N2");
                    //P5 = WTIMRLW =>  P[5],P[0],P[1],P[2],P[3],P[4],P[5]
                    oCurrentHandPolygons[4].Points.Add(HandPointList[5]);
                    oCurrentHandPolygons[4].Points.Add(HandPointList[0]);
                    oCurrentHandPolygons[4].Points.Add(HandPointList[1]);
                    oCurrentHandPolygons[4].Points.Add(HandPointList[2]);
                    oCurrentHandPolygons[4].Points.Add(HandPointList[3]);
                    oCurrentHandPolygons[4].Points.Add(HandPointList[4]);
                    txlP5.Text = PolygonArea(oCurrentHandPolygons[4]).ToString("N2");
                    txlLFL.Text = GetDistance(HandPointList[5], HandPointList[4]).ToString("N2");



                    for (int i = 0; i < oCurrentHandPolygons.Count - 1; i++)
                    {

                        if (cnvImage.Children.Contains(oCurrentHandPolygons[i]))
                            cnvImage.Children.Remove(oCurrentHandPolygons[i]);

                        oCurrentHandPolygons[i].Fill = Renkler[i];
                        oCurrentHandPolygons[i].Stroke = Brushes.Black;
                        oCurrentHandPolygons[i].Opacity = 0.3;
                        oCurrentHandPolygons[i].StrokeThickness = 0.8;
                        cnvImage.Children.Add(oCurrentHandPolygons[i]);


                    }

                }


            }

            #endregion


            if (MouseMode == EnumMouseMode.IRFinger)
            {


                for (int i = 0; i < IRPointList.Count; i++)
                {
                    DrawPoint(IRPointList[i], ACTIVE_LINE_COLOR);

                    if (i < IRPointList.Count - 1 && (i % 2 == 0))
                        DrawLine(new Line(), IRPointList[i], IRPointList[i + 1], ACTIVE_LINE_COLOR);
                }

                if (IRPointList.Count == 2)
                {


                    txlP1.Text = GetDistance(IRPointList[0], IRPointList[1]).ToString("N2");

                }
                if (IRPointList.Count == 4)
                {

                    txlP2.Text = GetDistance(IRPointList[2], IRPointList[3]).ToString("N2");

                }




                //for (int i = 0; i < cnvImage.Children.Count; i++)
                //{
                //    if (cnvImage.Children[i] is Line && cnvImage.Children[i] != oRatioLine)
                //    {
                //        cnvImage.Children.Remove(cnvImage.Children[i]);
                //    }

                //}



                //imgCurrent.LayoutTransform = new RotateTransform(_rotationAngle);
                //HandPointList[0].MouseLeftButtonDown += new MouseButtonEventHandler(cokgen_MouseLeftButtonDown);


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

            //if (MouseMode == EnumMouseMode.Hand)
            //    this.Cursor = Cursors.Hand;
            //else if (MouseMode == EnumMouseMode.Ratio)
            //    this.Cursor = Cursors.SizeWE;
            //else
            //    this.Cursor = Cursors.Arrow;


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddHand_Click(object sender, RoutedEventArgs e)
        {

            if (MouseMode == EnumMouseMode.Hand)
            {

                if (HandPointList.Count == 0)
                    return;

                HandData hd = new HandData()
                {
                    FileName = txlFileName.Text,
                    Name = txtName.Text,
                    Surname = txtSurname.Text,
                    Gender = (byte)(rbMale.IsChecked.Value == true ? 1 : 0),
                    Dominant = (byte)(rbDominant.IsChecked.Value == true ? 1 : 0),
                    RightOrLeft = (byte)(rbHandR.IsChecked.Value == true ? 1 : 0),//R=1;L=0
                    Optime = DateTime.Now,
                    //.ToString("N2")
                    P1 = Convert.ToDouble(PolygonArea(oCurrentHandPolygons[0]).ToString("N2")),
                    P2 = Convert.ToDouble(PolygonArea(oCurrentHandPolygons[1]).ToString("N2")),
                    P3 = Convert.ToDouble(PolygonArea(oCurrentHandPolygons[2]).ToString("N2")),
                    P4 = Convert.ToDouble(PolygonArea(oCurrentHandPolygons[3]).ToString("N2")),
                    P5 = Convert.ToDouble(PolygonArea(oCurrentHandPolygons[4]).ToString("N2")),

                    TL = Convert.ToDouble(GetDistance(HandPointList[5], HandPointList[0]).ToString("N2")),
                    IFL = Convert.ToDouble(GetDistance(HandPointList[5], HandPointList[1]).ToString("N2")),
                    MFL = Convert.ToDouble(GetDistance(HandPointList[5], HandPointList[2]).ToString("N2")),
                    RFL = Convert.ToDouble(GetDistance(HandPointList[5], HandPointList[3]).ToString("N2")),
                    LFL = Convert.ToDouble(GetDistance(HandPointList[5], HandPointList[4]).ToString("N2")),

                    FS1 = Convert.ToDouble(GetDistance(HandPointList[0], HandPointList[1]).ToString("N2")),
                    FS2 = Convert.ToDouble(GetDistance(HandPointList[0], HandPointList[2]).ToString("N2")),
                    FS3 = Convert.ToDouble(GetDistance(HandPointList[0], HandPointList[3]).ToString("N2")),
                    FS4 = Convert.ToDouble(GetDistance(HandPointList[0], HandPointList[4]).ToString("N2")),

                    Thumb_X = HandPointList[0].X,
                    Thumb_Y = HandPointList[0].Y,
                    Index_X = HandPointList[1].X,
                    Index_Y = HandPointList[1].Y,
                    Middle_X = HandPointList[2].X,
                    Middle_Y = HandPointList[2].Y,
                    Ring_X = HandPointList[3].X,
                    Ring_Y = HandPointList[3].Y,
                    Little_X = HandPointList[4].X,
                    Little_Y = HandPointList[4].Y,

                };



                HandData existstHD = _HandDataList.Where(x => x.FileName == hd.FileName).FirstOrDefault();
                if (existstHD != null)
                    _HandDataList.Remove(existstHD);

                _HandDataList.Add(hd);

                DBHelper.WriteDataFile(hd);
                InitializeHandData();

            }
            else if (MouseMode == EnumMouseMode.IRFinger)
            {
                if (IRPointList.Count == 0)
                    return;

                HandData hd = new HandData()
                {
                    FileName = txlFileName.Text,
                    Name = txtName.Text,
                    Surname = txtSurname.Text,
                    Gender = (byte)(rbMale.IsChecked.Value == true ? 1 : 0),
                    Dominant = (byte)(rbDominant.IsChecked.Value == true ? 1 : 0),
                    RightOrLeft = (byte)(rbHandR.IsChecked.Value == true ? 1 : 0),//R=1;L=0
                    Optime = DateTime.Now,


                    P1 = Convert.ToDouble(GetDistance(IRPointList[0], IRPointList[1]).ToString("N2")),
                    P2 = Convert.ToDouble(GetDistance(IRPointList[2], IRPointList[3]).ToString("N2")),
                    P3 = 0,
                    P4 = 0,
                    P5 = 0,

                    TL = 0,
                    IFL = 0,
                    MFL = 0,
                    RFL = 0,
                    LFL = 0,

                    FS1 = 0,
                    FS2 = 0,
                    FS3 = 0,
                    FS4 = 0,


                    Thumb_X = IRPointList[0].X,
                    Thumb_Y = IRPointList[0].Y,
                    Index_X = IRPointList[1].X,
                    Index_Y = IRPointList[1].Y,
                    Middle_X = IRPointList[2].X,
                    Middle_Y = IRPointList[2].Y,
                    Ring_X = IRPointList[3].X,
                    Ring_Y = IRPointList[3].Y,
                    Little_X = 0,
                    Little_Y = 0,

                };



                HandData existstHD = _HandDataList.Where(x => x.FileName == hd.FileName).FirstOrDefault();
                if (existstHD != null)
                    _HandDataList.Remove(existstHD);

                _HandDataList.Add(hd);

                DBHelper.WriteDataFile(hd);
                InitializeHandData();
            }



        }



        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (MouseMode == EnumMouseMode.Hand)
            {
                if (HandPointList.Count > 0)
                    HandPointList.Remove(HandPointList[HandPointList.Count - 1]);
            }
            else if (MouseMode == EnumMouseMode.Ratio)
            {

                if (oCurrentRatio.Points.Count > 0)
                    oCurrentRatio.Points.Remove(oCurrentRatio.Points[oCurrentRatio.Points.Count - 1]);

            }
            else if (MouseMode == EnumMouseMode.IRFinger)
            {

                if (IRPointList.Count > 0)
                    IRPointList.Remove(IRPointList[IRPointList.Count - 1]);

            }

            RefreshPolygons();
            RefreshPointLabels();
        }

        private void RatioChanged(object sender, TextChangedEventArgs e)
        {
            if (oCurrentRatio!=null && oCurrentRatio.Points.Count == 2 && !string.IsNullOrEmpty(txtLength.Text))
            {
                double a = oCurrentRatio.Points[0].X - oCurrentRatio.Points[1].X;
                double b = oCurrentRatio.Points[0].Y - oCurrentRatio.Points[1].Y;
                double distance = Math.Sqrt(a * a + b * b);

                dRatio = double.Parse(txtLength.Text) / distance;
                txtRatio.Text = dRatio.ToString();

                btnHand.IsEnabled = true;
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
                InitializeHandData();
            }
        }


    }
}
