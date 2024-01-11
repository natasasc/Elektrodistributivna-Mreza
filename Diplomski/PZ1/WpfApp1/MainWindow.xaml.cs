#region Usings
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using WpfApp1.Model;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;
using Point = WpfApp1.Model.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Drawing.Size;
#endregion Usings

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        #region Variables
        public SolidColorBrush subColor = new SolidColorBrush(System.Windows.Media.Colors.SkyBlue);
        public SolidColorBrush nodeColor = new SolidColorBrush(System.Windows.Media.Colors.LightSalmon);
        public SolidColorBrush switchColor = new SolidColorBrush(System.Windows.Media.Colors.YellowGreen);
        public SolidColorBrush lineColor = new SolidColorBrush(System.Windows.Media.Colors.DarkSlateGray);

        int divCountX;
        int divCountY;
        
        Element[,] divs;

        public double noviX, noviY;

        List<Element> elements = new List<Element>();
        Dictionary<Ellipse, long> ellipses = new Dictionary<Ellipse, long>();
        Dictionary<Polyline, LineEntity> lines = new Dictionary<Polyline, LineEntity>();

        double minX = 0;
        double minY = 0;
        double maxX = 0;
        double maxY = 0;

        double canvasHeight;
        double canvasWidth;

        int bfsCalls = 0;


        List<SwitchEntity> switchList = new List<SwitchEntity>();
        List<NodeEntity> nodeobjList = new List<NodeEntity>();
        List<SubstationEntity> subList = new List<SubstationEntity>();
        List<LineEntity> lineList = new List<LineEntity>();


        bool loaded = false;

        Dictionary<Ellipse, long> InactiveElements = new Dictionary<Ellipse, long>();
        Dictionary<Polyline, long> InactiveLines = new Dictionary<Polyline, long>();
        bool showInactiveElements = true;


        Dictionary<Ellipse, SubstationEntity> subEllipses = new Dictionary<Ellipse, SubstationEntity>();
        Dictionary<Ellipse, NodeEntity> nodeEllipses = new Dictionary<Ellipse, NodeEntity>();
        Dictionary<Ellipse, SwitchEntity> switchEllipses = new Dictionary<Ellipse, SwitchEntity>();

        bool lineResistance = false;

        public ColorsClass colors = new ColorsClass();

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            divCountX = (int)Math.Floor(myCanvas.Width) + 1;
            divCountY = (int)Math.Floor(myCanvas.Height) + 1;

            divs = new Element[divCountX, divCountY];

            for (int i = 0; i <= myCanvas.Width; i++)
            {
                for (int j = 0; j <= myCanvas.Height; j++)
                {
                    Element el = new Element(i, j, 0);
                    divs[i, j] = el;
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
            if (loaded)
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;


            bool first = true;


            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {
                SubstationEntity sub = new SubstationEntity();
                sub.Id = long.Parse(node["Id"].InnerText);
                sub.Name = node["Name"].InnerText;
                sub.X = double.Parse(node["X"].InnerText, CultureInfo.InvariantCulture);
                sub.Y = double.Parse(node["Y"].InnerText, CultureInfo.InvariantCulture);

                ToLatLon(sub.X, sub.Y, 34, out noviY, out noviX);

                if (Double.IsNaN(noviX) || Double.IsNaN(noviY))
                    continue;

                sub.X = noviX;
                sub.Y = noviY;

                subList.Add(sub);

                if (first)
                {
                    minX = noviX;
                    minY = noviY;
                    maxX = noviX;
                    maxY = noviY;

                    first = false;
                }

                if (noviX > maxX)
                    maxX = noviX;
                if (noviX < minX)
                    minX = noviX;
                if (noviY > maxY)
                    maxY = noviY;
                if (noviY < minY)
                    minY = noviY;
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {
                NodeEntity nodeobj = new NodeEntity();
                nodeobj.Id = long.Parse(node["Id"].InnerText);
                nodeobj.Name = node["Name"].InnerText;
                nodeobj.X = double.Parse(node["X"].InnerText, CultureInfo.InvariantCulture);
                nodeobj.Y = double.Parse(node["Y"].InnerText, CultureInfo.InvariantCulture);

                ToLatLon(nodeobj.X, nodeobj.Y, 34, out noviY, out noviX);

                if (Double.IsNaN(noviX) || Double.IsNaN(noviY))
                    continue;

                nodeobj.X = noviX;
                nodeobj.Y = noviY;

                nodeobjList.Add(nodeobj);

                if (noviX > maxX)
                    maxX = noviX;
                if (noviX < minX)
                    minX = noviX;
                if (noviY > maxY)
                    maxY = noviY;
                if (noviY < minY)
                    minY = noviY;
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchobj = new SwitchEntity();
                switchobj.Id = long.Parse(node["Id"].InnerText);
                switchobj.Name = node["Name"].InnerText;
                switchobj.X = double.Parse(node["X"].InnerText, CultureInfo.InvariantCulture);
                switchobj.Y = double.Parse(node["Y"].InnerText, CultureInfo.InvariantCulture);
                switchobj.Status = node["Status"].InnerText;

                ToLatLon(switchobj.X, switchobj.Y, 34, out noviY, out noviX);

                if (Double.IsNaN(noviX) || Double.IsNaN(noviY))
                    continue;

                switchobj.X = noviX;
                switchobj.Y = noviY;

                switchList.Add(switchobj);

                if (noviX > maxX)
                    maxX = noviX;
                if (noviX < minX)
                    minX = noviX;
                if (noviY > maxY)
                    maxY = noviY;
                if (noviY < minY)
                    minY = noviY;
            }



            // Aproksimacija

            canvasHeight = maxY - minY;
            canvasWidth = maxX - minX;

            foreach (SubstationEntity se in subList)
            {
                Ellipse ellipse = new Ellipse
                {
                    Height = 1,
                    Width = 1,
                    Fill = subColor,
                    ToolTip = se.ToString()
                };

                int offsetX = (int)Math.Round((myCanvas.Width * (se.X - minX)) / canvasWidth);
                int offsetY = (int)Math.Round((myCanvas.Height * (se.Y - minY)) / canvasHeight);

                Element el;
                if (IsFree(offsetX, offsetY) == false)
                {
                    int offset = 1;
                    while (true)
                    {
                        el = CheckAround(offsetX, offsetY, offset);
                        if (el != null)
                        {
                            offsetX = el.X;
                            offsetY = el.Y;
                            break;
                        }
                        offset++;
                    }
                    el.Id = se.Id;
                    elements.Add(el);
                }
                else
                {
                    elements.Add(new Element(offsetX, offsetY, false, se.Id));
                }

                Canvas.SetLeft(ellipse, offsetX);
                Canvas.SetBottom(ellipse, offsetY);

                myCanvas.Children.Add(ellipse);

                ellipses.Add(ellipse, se.Id);
                subEllipses.Add(ellipse, se);
            }

            foreach (NodeEntity ne in nodeobjList)
            {
                Ellipse ellipse = new Ellipse
                {
                    Height = 1,
                    Width = 1,
                    Fill = nodeColor,
                    ToolTip = ne.ToString()
                };

                int offsetX = (int)Math.Round((myCanvas.Width * (ne.X - minX)) / canvasWidth);
                int offsetY = (int)Math.Round((myCanvas.Height * (ne.Y - minY)) / canvasHeight);

                Element el;
                if (IsFree(offsetX, offsetY) == false)
                {
                    // Logika za pomeranje
                    int offset = 1;
                    while (true)
                    {
                        el = CheckAround(offsetX, offsetY, offset);
                        if (el != null)
                        {
                            offsetX = el.X;
                            offsetY = el.Y;
                            break;
                        }
                        offset++;
                    }
                    el.Id = ne.Id;
                    elements.Add(el);
                }
                else
                {
                    elements.Add(new Element(offsetX, offsetY, ne.Id));
                }

                Canvas.SetLeft(ellipse, offsetX);
                Canvas.SetBottom(ellipse, offsetY);

                myCanvas.Children.Add(ellipse);

                ellipses.Add(ellipse, ne.Id);
                nodeEllipses.Add(ellipse, ne);
            }

            foreach (SwitchEntity se in switchList)
            {
                Ellipse ellipse = new Ellipse
                {
                    Height = 1,
                    Width = 1,
                    Fill = switchColor,
                    ToolTip = se.ToString()
                };
                
                int offsetX = (int)Math.Round((myCanvas.Width * (se.X - minX)) / canvasWidth);
                int offsetY = (int)Math.Round((myCanvas.Height * (se.Y - minY)) / canvasHeight);

                Element el;
                if (IsFree(offsetX, offsetY) == false)
                {
                    // Logika za pomeranje
                    int offset = 1;
                    while (true)
                    {
                        el = CheckAround(offsetX, offsetY, offset);
                        if (el != null)
                        {
                            offsetX = el.X;
                            offsetY = el.Y;
                            break;
                        }
                        offset++;
                    }
                    el.Id = se.Id;
                    elements.Add(el);
                }
                else
                {
                    elements.Add(new Element(offsetX, offsetY, se.Id));
                }

                Canvas.SetLeft(ellipse, offsetX);
                Canvas.SetBottom(ellipse, offsetY);

                myCanvas.Children.Add(ellipse);

                ellipses.Add(ellipse, se.Id);
                switchEllipses.Add(ellipse, se);
            }


            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                LineEntity l = new LineEntity();
                l.Id = long.Parse(node["Id"].InnerText);
                l.Name = node["Name"].InnerText;
                if (node["IsUnderground"].InnerText.Equals("true"))
                {
                    l.IsUnderground = true;
                }
                else
                {
                    l.IsUnderground = false;
                }
                l.R = float.Parse(node["R"].InnerText, CultureInfo.InvariantCulture);
                l.ConductorMaterial = node["ConductorMaterial"].InnerText;
                l.LineType = node["LineType"].InnerText;
                l.ThermalConstantHeat = long.Parse(node["ThermalConstantHeat"].InnerText);
                l.FirstEnd = long.Parse(node["FirstEnd"].InnerText);
                l.SecondEnd = long.Parse(node["SecondEnd"].InnerText);

                //foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes) // 9 posto je Vertices 9. node u jednom line objektu
                //{
                //    Point p = new Point();

                //    p.X = double.Parse(pointNode["X"].InnerText);
                //    p.Y = double.Parse(pointNode["Y"].InnerText);

                //    ToLatLon(p.X, p.Y, 34, out noviX, out noviY);

                //}

                lineList.Add(l);
            }

            List<LineEntity> pass = new List<LineEntity>();
            List<LineEntity> linesNoBfs = new List<LineEntity>();

            int bfsCount = 0;

            foreach (LineEntity le in lineList)
            {
                //Treba ignorisati ponovno iscrtavanje vodova izmedju dva ista entiteta
                bool duplicate = false;
                foreach (LineEntity old in pass)
                {
                    if (le.FirstEnd == old.FirstEnd && le.SecondEnd == old.SecondEnd)
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate)
                    continue;


                Element elFirst = new Element();
                Element elSecond = new Element();
                bool existsFirst = false;
                bool existsSecond = false;

                foreach (Element el in elements)
                {
                    if (el.Id == le.FirstEnd)
                    {
                        existsFirst = true;
                        elFirst = el;
                        if (existsSecond)
                            break;
                    }
                    if (el.Id == le.SecondEnd)
                    {
                        existsSecond = true;
                        elSecond = el;
                        if (existsFirst)
                            break;
                    }
                }

                // Iscrtavaju se samo one linije čiji Start i End Node postoje u kolekcijama entiteta
                if (existsFirst && existsSecond)
                {
                    foreach (SubstationEntity sub in subList)
                    {
                        if (sub.Id == le.FirstEnd || sub.Id == le.SecondEnd)
                            sub.ConNum++;
                    }

                    foreach (NodeEntity n in nodeobjList)
                    {
                        if (n.Id == le.FirstEnd || n.Id == le.SecondEnd)
                            n.ConNum++;
                    }

                    foreach (SwitchEntity sw in switchList)
                    {
                        if (sw.Id == le.FirstEnd || sw.Id == le.SecondEnd)
                            sw.ConNum++;
                    }

                    Queue<Element> queue = BFS(elFirst, elSecond);

                    if (queue == null)
                    {
                        linesNoBfs.Add(le);
                        continue;
                    }

                    bfsCount++;

                    Polyline polyLine = new Polyline()
                    {
                        Stroke = System.Windows.Media.Brushes.DarkGray,
                        StrokeThickness = 0.3,
                        ToolTip = le.ToString()
                    };

                    switch (le.ConductorMaterial)
                    {
                        case "Steel":
                            polyLine.Stroke = System.Windows.Media.Brushes.CornflowerBlue;
                            break;
                        case "Acsr":
                            polyLine.Stroke = System.Windows.Media.Brushes.LimeGreen;
                            break;
                        case "Copper":
                            polyLine.Stroke = System.Windows.Media.Brushes.DeepPink;
                            break;
                        case "Other":
                            polyLine.Stroke = System.Windows.Media.Brushes.DarkGray;
                            break;
                    }


                    Element last = queue.Dequeue();
                    polyLine.Points.Add(new System.Windows.Point(last.X + 0.5, myCanvas.Height - last.Y - 0.5));

                    while (queue.Any())
                    {
                        Element current = queue.Dequeue();
                        polyLine.Points.Add(new System.Windows.Point(current.X + 0.5, myCanvas.Height - current.Y - 0.5));

                        if (!queue.Any())
                            break;

                        if (last.X != current.X)
                            divs[current.X, current.Y].LineX = true;
                        if (last.Y != current.Y)
                            divs[current.X, current.Y].LineY = true;

                        last = current;
                    }

                    myCanvas.Children.Add(polyLine);

                    pass.Add(le);

                    polyLine.MouseRightButtonDown += Polyline_RightClick;

                    lines.Add(polyLine, le);

                }
            }


            int temp1 = bfsCount;
            int temp2 = bfsCalls;


            foreach (LineEntity le in linesNoBfs)
            {
                Polyline polyLine = new Polyline()
                {
                    Stroke = System.Windows.Media.Brushes.DarkGray,
                    StrokeThickness = 0.3,
                    ToolTip = le.ToString()
                };


                switch (le.ConductorMaterial)
                {
                    case "Steel":
                        polyLine.Stroke = System.Windows.Media.Brushes.CornflowerBlue;
                        break;
                    case "Acsr":
                        polyLine.Stroke = System.Windows.Media.Brushes.LimeGreen;
                        break;
                    case "Copper":
                        polyLine.Stroke = System.Windows.Media.Brushes.DeepPink;
                        break;
                    case "Other":
                        polyLine.Stroke = System.Windows.Media.Brushes.DarkGray;
                        break;
                }

                bool existsFirst = false;
                bool existsSecond = false;
                Element elFirst = new Element();
                Element elSecond = new Element();

                foreach (Element el in elements)
                {
                    if (el.Id == le.FirstEnd)
                    {
                        existsFirst = true;
                        elFirst = el;
                        if (existsSecond)
                            break;
                    }
                    if (el.Id == le.SecondEnd)
                    {
                        existsSecond = true;
                        elSecond = el;
                        if (existsFirst)
                            break;
                    }
                }

                // Linija mora da krece iz centra entiteta
                double startX = elFirst.X + 0.5;
                double startY = myCanvas.Height - elFirst.Y - 0.5;      // u odnosu na Canvas.Top
                double endX = elSecond.X + 0.5;
                double endY = myCanvas.Height - elSecond.Y - 0.5;

                polyLine.Points.Add(new System.Windows.Point(startX, startY));

                if (elFirst.X != elSecond.X && elFirst.Y != elSecond.Y)
                {
                    polyLine.Points.Add(new System.Windows.Point(startX, endY));

                    if (elFirst.X < elSecond.X && elFirst.Y > elSecond.Y)
                    {
                        //  |_

                        for (int y = elFirst.Y - 1; y > elSecond.Y; y--)
                        {
                            divs[elFirst.X, y].LineY = true;
                        }

                        for (int x = elFirst.X + 1; x < elSecond.X; x++)
                        {
                            divs[x, elSecond.Y].LineX = true;
                        }
                    }
                    else if (elFirst.X < elSecond.X && elFirst.Y < elSecond.Y)
                    {
                        //   _
                        //  |

                        for (int y = elFirst.Y + 1; y < elSecond.Y; y++)
                        {
                            divs[elFirst.X, y].LineY = true;
                        }

                        for (int x = elFirst.X + 1; x < elSecond.X; x++)
                        {
                            divs[x, elSecond.Y].LineX = true;
                        }
                    }
                    else if (elFirst.X > elSecond.X && elFirst.Y < elSecond.Y)
                    {
                        //  _
                        //   |

                        for (int y = elFirst.Y + 1; y < elSecond.Y; y++)
                        {
                            divs[elFirst.X, y].LineY = true;
                        }

                        for (int x = elFirst.X - 1; x > elSecond.X; x--)
                        {
                            divs[x, elSecond.Y].LineX = true;
                        }
                    }
                    else
                    {
                        //  _|

                        for (int y = elFirst.Y - 1; y > elSecond.Y; y--)
                        {
                            divs[elFirst.X, y].LineY = true;
                        }

                        for (int x = elFirst.X - 1; x > elSecond.X; x--)
                        {
                            divs[x, elSecond.Y].LineX = true;
                        }
                    }
                }
                else if (elFirst.X == elSecond.X)
                {
                    if (elFirst.Y < elSecond.Y)
                    {
                        for (int y = elFirst.Y + 1; y < elSecond.Y; y++)
                        {
                            divs[elFirst.X, y].LineY = true;
                        }
                    }
                    else
                    {
                        for (int y = elFirst.Y - 1; y > elSecond.Y; y--)
                        {
                            divs[elFirst.X, y].LineY = true;
                        }
                    }
                }
                else if (elFirst.Y == elSecond.Y)
                {
                    if (elFirst.X < elSecond.X)
                    {
                        for (int x = elFirst.X + 1; x < elSecond.X; x++)
                        {
                            divs[x, elFirst.Y].LineX = true;
                        }
                    }
                    else
                    {
                        for (int x = elFirst.X - 1; x > elSecond.X; x--)
                        {
                            divs[x, elFirst.Y].LineX = true;
                        }
                    }
                }

                polyLine.Points.Add(new System.Windows.Point(endX, endY));


                myCanvas.Children.Add(polyLine);

                pass.Add(le);

                polyLine.MouseRightButtonDown += Polyline_RightClick;

                lines.Add(polyLine, le);

            }

            foreach (SwitchEntity se in switchList)
            {
                if (se.Status == "Open")
                {
                    foreach (KeyValuePair<Polyline, LineEntity> pair in lines)
                    {
                        if (se.Id == pair.Value.FirstEnd)
                        {
                            InactiveLines.Add(pair.Key, pair.Value.Id);

                            if (!InactiveElements.ContainsKey(ellipses.FirstOrDefault(x => x.Value == pair.Value.SecondEnd).Key))
                                InactiveElements.Add(ellipses.FirstOrDefault(x => x.Value == pair.Value.SecondEnd).Key, pair.Value.SecondEnd);
                        }
                    }
                }
            }


            #region Entity Colors
            if (colors.SubImg != null || colors.SubColor != null)
            {
                foreach (KeyValuePair<Ellipse, SubstationEntity> pair in subEllipses)
                {
                    if (colors.SubImg != null)
                    {
                        ImageBrush ib = new ImageBrush();
                        ib.ImageSource = colors.SubImg;
                        pair.Key.Fill = ib;
                    }
                    else
                    {
                        pair.Key.Fill = colors.SubColor;
                    }
                }
            }
            else
            {
                InitialSubColor();
            }

            if (colors.NodeImg != null || colors.NodeColor != null)
            {
                foreach (KeyValuePair<Ellipse, NodeEntity> pair in nodeEllipses)
                {
                    if (colors.NodeImg != null)
                    {
                        ImageBrush ib = new ImageBrush();
                        ib.ImageSource = colors.NodeImg;
                        pair.Key.Fill = ib;
                    }
                    else
                    {
                        pair.Key.Fill = colors.NodeColor;
                    }
                }
            }
            else
            {
                InitialNodeColor();
            }

            if (colors.SwitchImg != null || colors.SwitchColor != null)
            {
                foreach (KeyValuePair<Ellipse, SwitchEntity> pair in switchEllipses)
                {
                    if (colors.SwitchImg != null)
                    {
                        ImageBrush ib = new ImageBrush();
                        ib.ImageSource = colors.SwitchImg;
                        pair.Key.Fill = ib;
                    }
                    else
                    {
                        pair.Key.Fill = colors.SwitchColor;
                    }
                }
            }
            else
            {
                InitialSwitchColor();
            }
            #endregion

            // Preseci
            foreach (Element el in divs)
            {
                if (el.LineX && el.LineY)
                {
                    // presek

                    double X = el.X + 0.5;
                    double Y = myCanvas.Height - el.Y - 0.5;      // u odnosu na Canvas.Top

                    Polygon p = new Polygon();
                    p.Stroke = System.Windows.Media.Brushes.PaleGoldenrod;
                    p.StrokeThickness = 0.1;

                    p.Points.Add(new System.Windows.Point(X, Y));   // tacka od X u sredini
                    p.Points.Add(new System.Windows.Point(X - 0.2, Y - 0.2));
                    p.Points.Add(new System.Windows.Point(X, Y));
                    p.Points.Add(new System.Windows.Point(X - 0.2, Y + 0.2));
                    p.Points.Add(new System.Windows.Point(X, Y));
                    p.Points.Add(new System.Windows.Point(X + 0.2, Y + 0.2));
                    p.Points.Add(new System.Windows.Point(X, Y));
                    p.Points.Add(new System.Windows.Point(X + 0.2, Y - 0.2));
                    p.Points.Add(new System.Windows.Point(X, Y));

                    myCanvas.Children.Add(p);
                }
            }

            loaded = true;

        }



        List<Tuple<System.Windows.Media.Brush, Ellipse>> coloredEllipses = new List<Tuple<System.Windows.Media.Brush, Ellipse>>();

        private void Polyline_RightClick(object sender, MouseButtonEventArgs e)
        {
            foreach (Tuple<System.Windows.Media.Brush, Ellipse> t in coloredEllipses)
            {
                t.Item2.Fill = t.Item1;
            }

            coloredEllipses.Clear();


            Polyline polyline = (Polyline)sender;

            System.Windows.Point start = new System.Windows.Point();
            start = polyline.Points[0];
            System.Windows.Point end = new System.Windows.Point();
            end = polyline.Points[polyline.Points.Count - 1];

            foreach (Ellipse el in ellipses.Keys)
            {
                double bottom = Canvas.GetBottom(el);
                double left = Canvas.GetLeft(el);

                if (left == (start.X - 0.5) && bottom == (myCanvas.Height - start.Y - 0.5))
                {
                    coloredEllipses.Add(new Tuple<System.Windows.Media.Brush, Ellipse>(el.Fill, el));
                    el.Fill = System.Windows.Media.Brushes.Black;
                }

                if (left == (end.X - 0.5) && bottom == (myCanvas.Height - end.Y - 0.5))
                {
                    coloredEllipses.Add(new Tuple<System.Windows.Media.Brush, Ellipse>(el.Fill, el));
                    el.Fill = System.Windows.Media.Brushes.Black;
                }
            }

        }

        public bool IsFree(int offsetX, int offsetY)
        {
            if (divs[offsetX, offsetY].Free)
            {
                divs[offsetX, offsetY].Free = false;
                return true;
            }

            return false;
        }

        public Element CheckAround(int x, int y, int offset)
        {
            for (int i = x - offset; i <= x + offset; i++)
            {
                if (i < 0)
                    continue;
                else if (i >= divCountX)
                    continue;

                for (int j = y - offset; j <= y + offset; j++)
                {
                    if (j < 0)
                        continue;
                    else if (j >= divCountY)
                        continue;

                    if (IsFree(i, j))
                        return divs[i,j];
                }
            }
            return null;
        }


        public Queue<Element> BFS(Element start, Element end)
        {
            bfsCalls++;

            Queue<Element> copy = new Queue<Element>();
            Queue<Element> queue = new Queue<Element>();

            divs[start.X, start.Y].Visited = true;
            queue.Enqueue(start);
            copy.Enqueue(start);

            bool found = false;

            Element current = new Element();

            while (queue.Any())
            {
                current = queue.Dequeue();

                if (current.X == end.X && current.Y == end.Y)
                {
                    found = true;
                    break;
                }

                //Element[] neighbors = new Element[4];
                if (current.X + 1 < divCountX)
                {
                    //neighbors[0] = divs[current.X + 1, current.Y];
                    if (!divs[current.X + 1, current.Y].Visited)
                    {
                        divs[current.X + 1, current.Y].Visited = true;
                        divs[current.X + 1, current.Y].ParentX = current.X;
                        divs[current.X + 1, current.Y].ParentY = current.Y;
                        queue.Enqueue(divs[current.X + 1, current.Y]);
                        copy.Enqueue(divs[current.X + 1, current.Y]);

                    }
                }
                if (current.X - 1 >= 0)
                {
                    //neighbors[1] = divs[current.X - 1, current.Y];
                    if (!divs[current.X - 1, current.Y].Visited)
                    {
                        divs[current.X - 1, current.Y].Visited = true;
                        divs[current.X - 1, current.Y].ParentX = current.X;
                        divs[current.X - 1, current.Y].ParentY = current.Y;
                        queue.Enqueue(divs[current.X - 1, current.Y]);
                        copy.Enqueue(divs[current.X - 1, current.Y]);

                    }
                }
                if (current.Y + 1 < divCountY)
                {
                    //neighbors[2] = divs[current.X, current.Y + 1];
                    if (!divs[current.X, current.Y + 1].Visited)
                    {
                        divs[current.X, current.Y + 1].Visited = true;
                        divs[current.X, current.Y + 1].ParentX = current.X;
                        divs[current.X, current.Y + 1].ParentY = current.Y;
                        queue.Enqueue(divs[current.X, current.Y + 1]);
                        copy.Enqueue(divs[current.X, current.Y + 1]);

                    }
                }
                if (current.Y - 1 >= 0)
                {
                    //neighbors[3] = divs[current.X, current.Y - 1];
                    if (!divs[current.X, current.Y - 1].Visited)
                    {
                        divs[current.X, current.Y - 1].Visited = true;
                        divs[current.X, current.Y - 1].ParentX = current.X;
                        divs[current.X, current.Y - 1].ParentY = current.Y;
                        queue.Enqueue(divs[current.X, current.Y - 1]);
                        copy.Enqueue(divs[current.X, current.Y - 1]);

                    }
                }
            }

            if (!found)
            {
                while (copy.Any())
                {
                    Element temp = copy.Dequeue();

                    divs[temp.X, temp.Y].Visited = false;
                    divs[temp.X, temp.Y].ParentX = -1;
                    divs[temp.X, temp.Y].ParentY = -1;
                }

                return null;
            }
            // obrcem red
            Stack<Element> stack = new Stack<Element>();
            while (copy.Count > 0)
            {
                stack.Push(copy.Dequeue());
            }
            while (stack.Count > 0)
            {
                copy.Enqueue(stack.Pop());
            }

            // trazim end element
            Element last = new Element();
            while (copy.Any())
            {
                Element temp = copy.Dequeue();
                if (temp.X == end.X && temp.Y == end.Y)
                {
                    last = temp;
                    break;
                }

                //temp.Visited = false;
                divs[temp.X, temp.Y].Visited = false;
                divs[temp.X, temp.Y].ParentX = -1;
                divs[temp.X, temp.Y].ParentY = -1;
            }

            Queue<Element> ret = new Queue<Element>();
            ret.Enqueue(last);

            while (copy.Any())
            {
                Element next = copy.Dequeue();
                if (last.ParentX == next.X && last.ParentY == next.Y)
                {
                    ret.Enqueue(next);
                    last = next;
                }
                else
                {
                    //next.Visited = false;
                    divs[next.X, next.Y].Visited = false;
                    divs[next.X, next.Y].ParentX = -1;
                    divs[next.X, next.Y].ParentY = -1;
                }
            }

            divs[start.X, start.Y].Visited = false;
            divs[start.X, start.Y].ParentX = -1;
            divs[start.X, start.Y].ParentY = -1;
            divs[end.X, end.Y].Visited = false;
            divs[end.X, end.Y].ParentX = -1;
            divs[end.X, end.Y].ParentX = -1;

            return ret;
        }


        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }



        public List<Canvas> canvasList = new List<Canvas>();
        Canvas undoSave;
        List<Canvas> clearSave;
        bool clear = false;

        #region Ellipse

        bool enableDrawEllipse = false;
        public Ellipse objEllipse;
        public TextBlock textEllipse;
        System.Windows.Point pointEllipse = new System.Windows.Point();
        public Ellipse editEllipse;

        private void DrawEllipse(object sender, RoutedEventArgs e)
        {
            enableDrawEllipse = true;
            enableDrawPolygon = false;
            enableAddText = false;
        }

        public void FinishedEllipse()
        {
            objEllipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

            Canvas subCanvas = new Canvas();
            subCanvas.Height = objEllipse.Height;
            subCanvas.Width = objEllipse.Width;

            subCanvas.Children.Add(objEllipse);
            Canvas.SetLeft(textEllipse, objEllipse.Width/4);
            Canvas.SetTop(textEllipse, objEllipse.Height/4);
            subCanvas.Children.Add(textEllipse);

            Canvas.SetLeft(subCanvas, pointEllipse.X);
            Canvas.SetTop(subCanvas, pointEllipse.Y);
            myCanvas.Children.Add(subCanvas);

            canvasList.Add(subCanvas);
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editEllipse = (Ellipse)sender;

            EditEllipseWindow eew = new EditEllipseWindow(this);
            eew.Show();
        }

        #endregion


        #region Polygon

        bool enableDrawPolygon = false;
        List<System.Windows.Point> polygonPoints = new List<System.Windows.Point>();
        public Polygon objPolygon;
        public TextBlock textPolygon;
        public Polygon editPolygon;

        private void DrawPolygon(object sender, RoutedEventArgs e)
        {
            enableDrawPolygon = true;
            enableDrawEllipse = false;
            enableAddText = false;
        }

        public void FinishedPolygon()
        {
            objPolygon.MouseLeftButtonDown += Polygon_MouseLeftButtonDown;

            Canvas subCanvas = new Canvas();

            bool first = true;
            double minX = 0;
            double maxX = 0;
            double minY = 0;
            double maxY = 0;
            foreach (System.Windows.Point p in objPolygon.Points)
            {
                if (first)
                {
                    minX = p.X;
                    maxX = p.X;
                    minY = p.Y;
                    maxY = p.Y;
                    first = false;
                }
                else
                {
                    if (p.X < minX)
                        minX = p.X;
                    if (p.X > maxX)
                        maxX = p.X;
                    if (p.Y < minY)
                        minY = p.Y;
                    if (p.Y > maxY)
                        maxY = p.Y;
                }
            }

            double height = maxY - minY;
            double width = maxX - minX;

            subCanvas.Height = height;
            subCanvas.Width = width;

            subCanvas.Children.Add(objPolygon);
            Canvas.SetLeft(textPolygon, minX + width / 4);
            Canvas.SetTop(textPolygon, minY + height / 4);
            subCanvas.Children.Add(textPolygon);

            // kod poligona nema pomeranja
            myCanvas.Children.Add(subCanvas);

            canvasList.Add(subCanvas);

            polygonPoints.RemoveRange(0, polygonPoints.Count);
        }

        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editPolygon = (Polygon)sender;

            EditPolygonWindow epw = new EditPolygonWindow(this);
            epw.Show();
        }

        #endregion


        #region Text

        bool enableAddText = false;
        public TextBlock objText;
        System.Windows.Point pointText = new System.Windows.Point();
        public TextBlock editText;

        private void AddText(object sender, RoutedEventArgs e)
        {
            enableAddText = true;
            enableDrawEllipse = false;
            enableDrawPolygon = false;
        }

        public void FinishedText()
        {
            objText.MouseLeftButtonDown += Text_MouseLeftButtonDown;

            Canvas subCanvas = new Canvas();
            subCanvas.Height = objText.Height;
            subCanvas.Width = objText.Width;

            subCanvas.Children.Add(objText);

            Canvas.SetLeft(subCanvas, pointText.X);
            Canvas.SetTop(subCanvas, pointText.Y);
            myCanvas.Children.Add(subCanvas);

            canvasList.Add(subCanvas);
        }

        private void Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editText = (TextBlock)sender;

            EditTextWindow etw = new EditTextWindow(this);
            etw.Show();
        }

        #endregion



        private void myCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Crtanje moze samo preko modela
            if (!loaded)
                return;

            if (enableDrawEllipse)
            {
                clear = false;
                if (clearSave != null)
                    clearSave.RemoveRange(0, clearSave.Count);

                pointEllipse = e.GetPosition(myCanvas);

                textEllipse = new TextBlock();
                objEllipse = new Ellipse();
                EllipseWindow ellipseWindow = new EllipseWindow(this);
                ellipseWindow.Show();
            }

            if (enableDrawPolygon)
            {
                clear = false;
                if (clearSave != null)
                    clearSave.RemoveRange(0, clearSave.Count);

                polygonPoints.Add(e.GetPosition(myCanvas));
            }
            else
            {
                polygonPoints.RemoveRange(0, polygonPoints.Count);
            }
            
            if (enableAddText)
            {
                clear = false;
                if (clearSave != null)
                    clearSave.RemoveRange(0, clearSave.Count);

                pointText = e.GetPosition(myCanvas);

                objText = new TextBlock();
                TextWindow textWindow = new TextWindow(this);
                textWindow.Show();
            }
        }

        private void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Nema iscrtavanja dok nije ucitan model
            if (!loaded)
                return;

            if (enableDrawPolygon && polygonPoints.Count > 0)
            {
                clear = false;
                if (clearSave != null)
                    clearSave.RemoveRange(0, clearSave.Count);

                textPolygon = new TextBlock();
                objPolygon = new Polygon();

                foreach (System.Windows.Point p in polygonPoints)
                {
                    objPolygon.Points.Add(p);
                }

                System.Windows.Point firstPoint = new System.Windows.Point();
                firstPoint.X = objPolygon.Points.First().X;
                firstPoint.Y = objPolygon.Points.First().Y;

                objPolygon.Points.Add(firstPoint);
                polygonPoints.Add(firstPoint);

                PolygonWindow polygonWindow = new PolygonWindow(this);
                polygonWindow.Show();
            }
        }

       

        #region Controls
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (clear)          // ne moze undo posle clear
                return;
            if (undoSave != null)       // vec smo uradili undo, a nismo redo
                return;
            if (canvasList.Count == 0)   // ostao je samo graf
                return;

            undoSave = (Canvas)myCanvas.Children[myCanvas.Children.Count - 1];      // cuvamo poslednje iscrtani element
            canvasList.Remove(undoSave);        // brisemo taj element iz svih trenutno iscrtanih elemenata
            myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);        // brisemo taj element sa canvasa
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (clear)      // posebna logika za redo posle clear
            {
                foreach (UIElement uie in clearSave)        // prolazimo kroz sve elemente koji su se sacuvali nakon Clear poziva
                {
                    canvasList.Add(uie as Canvas);      // svaki dodajemo u listu trenutnih elemenata
                    myCanvas.Children.Add(uie as Canvas);       // i dodajemo na canvas
                }

                clearSave.RemoveRange(0, clearSave.Count);      // brisemo sve elemente iz liste za cuvanje elemenata nakon Clear poziva
                clear = false;      // zavrsili smo clear + redo

                return;
            }

            if (undoSave == null)       // nemamo sta da vratimo
                return;
            
            canvasList.Add(undoSave);       // vracamo element u listu trenutnih elemenata
            myCanvas.Children.Add(undoSave);        // dodajemo ga na canvas da se prikaze
            undoSave = null;        // zavrsili smo undo + redo
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            radioDrawEllipse.IsChecked = false;
            enableDrawEllipse = false;
            radioDrawPolygon.IsChecked = false;
            enableDrawPolygon = false;
            radioAddText.IsChecked = false;
            enableAddText = false;

            if (canvasList.Count == 0)      // nemamo sta da obrisemo
                return;

            clear = true;

            clearSave = new List<Canvas>();
            foreach (UIElement uie in canvasList)   // cuvamo UIelemente za redo
            {
                clearSave.Add(uie as Canvas);
            }

            myCanvas.Children.RemoveRange(myCanvas.Children.Count - canvasList.Count, canvasList.Count);       
            // brisemo sve iznad grafa
            
            canvasList.RemoveRange(0, canvasList.Count);        // brisemo sve trenutne UIelemente
        }

        #endregion


        private void HideInactive_Click(object sender, RoutedEventArgs e)
        {
            if (showInactiveElements && loaded)
            {
                foreach (KeyValuePair<Ellipse, long> pair in InactiveElements)
                {
                    myCanvas.Children.Remove(pair.Key);
                }

                foreach (KeyValuePair<Polyline, long> pair in InactiveLines)
                {
                    myCanvas.Children.Remove(pair.Key);
                }

                showInactiveElements = false;
            }
        }

        private void ShowInactive_Click(object sender, RoutedEventArgs e)
        {
            if (!showInactiveElements && loaded)
            {
                foreach (KeyValuePair<Ellipse, long> pair in InactiveElements)
                {
                    myCanvas.Children.Add(pair.Key);
                }

                foreach (KeyValuePair<Polyline, long> pair in InactiveLines)
                {
                    myCanvas.Children.Add(pair.Key);
                }

                showInactiveElements = true;
            }
        }


        private void LineROY_Click(object sender, RoutedEventArgs e)
        {
            if (!lineResistance && loaded)
            {
                foreach (KeyValuePair<Polyline, LineEntity> pair in lines)
                {
                    if (pair.Value.R < 1)
                    {
                        pair.Key.Stroke = System.Windows.Media.Brushes.Red;
                    }
                    else if (pair.Value.R >= 1 && pair.Value.R <= 2)
                    {
                        pair.Key.Stroke = System.Windows.Media.Brushes.Orange;
                    }
                    else
                    {
                        pair.Key.Stroke = System.Windows.Media.Brushes.Yellow;
                    }

                }

                lineResistance = true;
            }
        }

        private void LineInitial_Click(object sender, RoutedEventArgs e)
        {
            if (lineResistance && loaded)
            {
                foreach (KeyValuePair<Polyline, LineEntity> pair in lines)
                {
                    switch (pair.Value.ConductorMaterial)
                    {
                        case "Steel":
                            pair.Key.Stroke = System.Windows.Media.Brushes.CornflowerBlue;
                            break;
                        case "Acsr":
                            pair.Key.Stroke = System.Windows.Media.Brushes.LimeGreen;
                            break;
                        case "Copper":
                            pair.Key.Stroke = System.Windows.Media.Brushes.DeepPink;
                            break;
                        case "Other":
                            pair.Key.Stroke = System.Windows.Media.Brushes.DarkGray;
                            break;
                    }
                }

                lineResistance = false;
            }
        }


        private void Colors_Click(object sender, RoutedEventArgs e)
        {
            EntityColors ec = new EntityColors(this);
            ec.Show();
        }

        #region Entity Colors

        public void ChangeColors()
        {
            if (loaded)
            {
                if (colors.SubImg != null || colors.SubColor != null)
                {
                    foreach (KeyValuePair<Ellipse, SubstationEntity> pair in subEllipses)
                    {
                        if (colors.SubImg != null)
                        {
                            ImageBrush ib = new ImageBrush();
                            ib.ImageSource = colors.SubImg;
                            pair.Key.Fill = ib;
                        }
                        else
                        {
                            pair.Key.Fill = colors.SubColor;
                        }
                    }
                }
                else
                {
                    InitialSubColor();
                }

                if (colors.NodeImg != null || colors.NodeColor != null)
                {
                    foreach (KeyValuePair<Ellipse, NodeEntity> pair in nodeEllipses)
                    {
                        if (colors.NodeImg != null)
                        {
                            ImageBrush ib = new ImageBrush();
                            ib.ImageSource = colors.NodeImg;
                            pair.Key.Fill = ib;
                        }
                        else
                        {
                            pair.Key.Fill = colors.NodeColor;
                        }
                    }
                }
                else
                {
                    InitialNodeColor();
                }

                if (colors.SwitchImg != null || colors.SwitchColor != null)
                {
                    foreach (KeyValuePair<Ellipse, SwitchEntity> pair in switchEllipses)
                    {
                        if (colors.SwitchImg != null)
                        {
                            ImageBrush ib = new ImageBrush();
                            ib.ImageSource = colors.SwitchImg;
                            pair.Key.Fill = ib;
                        }
                        else
                        {
                            pair.Key.Fill = colors.SwitchColor;
                        }
                    }
                }
                else
                {
                    InitialSwitchColor();
                }
            }
        }

        public void InitialEntityColors()
        {
            InitialSubColor();
            InitialNodeColor();
            InitialSwitchColor();
        }

        public void InitialSubColor()
        {
            foreach (KeyValuePair<Ellipse, SubstationEntity> pair in subEllipses)
            {
                if (pair.Value.ConNum < 3)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.Tomato;
                }
                else if (pair.Value.ConNum >= 3 && pair.Value.ConNum <= 5)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.Red;
                }
                else if (pair.Value.ConNum > 5)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.DarkRed;
                }
            }
        }

        public void InitialNodeColor()
        {
            foreach (KeyValuePair<Ellipse, NodeEntity> pair in nodeEllipses)
            {
                if (pair.Value.ConNum < 3)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.Tomato;
                }
                else if (pair.Value.ConNum >= 3 && pair.Value.ConNum <= 5)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.Red;
                }
                else if (pair.Value.ConNum > 5)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.DarkRed;
                }
            }
        }

        public void InitialSwitchColor()
        {
            foreach (KeyValuePair<Ellipse, SwitchEntity> pair in switchEllipses)
            {
                if (pair.Value.ConNum < 3)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.Tomato;
                }
                else if (pair.Value.ConNum >= 3 && pair.Value.ConNum <= 5)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.Red;
                }
                else if (pair.Value.ConNum > 5)
                {
                    pair.Key.Fill = System.Windows.Media.Brushes.DarkRed;
                }
            }
        }

        #endregion


        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)myCanvas.RenderSize.Width,
                (int)myCanvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Pbgra32);
            rtb.Render(myCanvas);

            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

            using (var stm = System.IO.File.Create(Directory.GetCurrentDirectory() + "..\\..\\..\\..\\" + DateTime.Now.ToString() + ".png"))
            {
                enc.Save(stm);
            }
        }


        Random rnd = new Random();

        List<long> SavingsIds = new List<long>();
        private void Savings_Checked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                int sirina = divCountX / 8;
                int visina = divCountY / 4;
                // Random deo grada
                int x = rnd.Next(8);
                int y = rnd.Next(4);


                for (int i = sirina * x; i < sirina * x + sirina; i++)
                {
                    for (int j = visina * y; j < visina * y + visina; j++)
                    {
                        foreach (Element el in elements)
                        {
                            if (el.X == i && el.Y == j)
                            {
                                SavingsIds.Add(el.Id);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Ellipse, long> pair in ellipses)
                {
                    if (SavingsIds.Contains(pair.Value))
                        pair.Key.Fill = Savings.Foreground;
                }

            }
        }

        private void Savings_Unchecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                ReturnColors(SavingsIds);

                SavingsIds.Clear();
            }
        }

        List<long> MaintenanceIds = new List<long>();
        private void Maintenance_Checked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                int sirina = divCountX / 8;
                int visina = divCountY / 4;
                // Random deo grada
                int x = rnd.Next(8);
                int y = rnd.Next(4);


                for (int i = sirina * x; i < sirina * x + sirina; i++)
                {
                    for (int j = visina * y; j < visina * y + visina; j++)
                    {
                        foreach (Element el in elements)
                        {
                            if (el.X == i && el.Y == j)
                            {
                                MaintenanceIds.Add(el.Id);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Ellipse, long> pair in ellipses)
                {
                    if (MaintenanceIds.Contains(pair.Value))
                        pair.Key.Fill = Maintenance.Foreground;
                }

            }
        }

        private void Maintenance_Unchecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                ReturnColors(MaintenanceIds);

                MaintenanceIds.Clear();
            }
        }

        List<long> StructureChangesIds = new List<long>();
        private void StructureChanges_Checked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                int sirina = divCountX / 8;
                int visina = divCountY / 4;
                // Random deo grada
                int x = rnd.Next(8);
                int y = rnd.Next(4);


                for (int i = sirina * x; i < sirina * x + sirina; i++)
                {
                    for (int j = visina * y; j < visina * y + visina; j++)
                    {
                        foreach (Element el in elements)
                        {
                            if (el.X == i && el.Y == j)
                            {
                                StructureChangesIds.Add(el.Id);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Ellipse, long> pair in ellipses)
                {
                    if (StructureChangesIds.Contains(pair.Value))
                        pair.Key.Fill = StructureChanges.Foreground;
                }

            }
        }

        private void StructureChanges_Unchecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                ReturnColors(StructureChangesIds);

                StructureChangesIds.Clear();
            }
        }

        List<long> NetworkOverloadIds = new List<long>();
        private void NetworkOverload_Checked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                int sirina = divCountX / 8;
                int visina = divCountY / 4;
                // Random deo grada
                int x = rnd.Next(8);
                int y = rnd.Next(4);


                for (int i = sirina * x; i < sirina * x + sirina; i++)
                {
                    for (int j = visina * y; j < visina * y + visina; j++)
                    {
                        foreach (Element el in elements)
                        {
                            if (el.X == i && el.Y == j)
                            {
                                NetworkOverloadIds.Add(el.Id);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Ellipse, long> pair in ellipses)
                {
                    if (NetworkOverloadIds.Contains(pair.Value))
                        pair.Key.Fill = NetworkOverload.Foreground;
                }

            }
        }

        private void NetworkOverload_Unchecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                ReturnColors(NetworkOverloadIds);

                NetworkOverloadIds.Clear();
            }
        }

        List<long> NaturalDisasterIds = new List<long>();
        private void NaturalDisaster_Checked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                int sirina = divCountX / 8;
                int visina = divCountY / 4;
                // Random deo grada
                int x = rnd.Next(8);
                int y = rnd.Next(4);


                for (int i = sirina * x; i < sirina * x + sirina; i++)
                {
                    for (int j = visina * y; j < visina * y + visina; j++)
                    {
                        foreach (Element el in elements)
                        {
                            if (el.X == i && el.Y == j)
                            {
                                NaturalDisasterIds.Add(el.Id);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<Ellipse, long> pair in ellipses)
                {
                    if (NaturalDisasterIds.Contains(pair.Value))
                        pair.Key.Fill = NaturalDisaster.Foreground;
                }


            }
        }

        private void NaturalDisaster_Unchecked(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                ReturnColors(NaturalDisasterIds);

                NaturalDisasterIds.Clear();
            }
        }


        public void ReturnColors(List<long> Ids)
        {
            foreach (KeyValuePair<Ellipse, SubstationEntity> pair in subEllipses)
            {
                if (Ids.Contains(pair.Value.Id))
                {
                    if (pair.Value.ConNum < 3)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.Tomato;
                    }
                    else if (pair.Value.ConNum >= 3 && pair.Value.ConNum <= 5)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.Red;
                    }
                    else if (pair.Value.ConNum > 5)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.DarkRed;
                    }
                }
            }

            foreach (KeyValuePair<Ellipse, NodeEntity> pair in nodeEllipses)
            {
                if (Ids.Contains(pair.Value.Id))
                {
                    if (pair.Value.ConNum < 3)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.Tomato;
                    }
                    else if (pair.Value.ConNum >= 3 && pair.Value.ConNum <= 5)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.Red;
                    }
                    else if (pair.Value.ConNum > 5)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.DarkRed;
                    }
                }
            }

            foreach (KeyValuePair<Ellipse, SwitchEntity> pair in switchEllipses)
            {
                if (Ids.Contains(pair.Value.Id))
                {
                    if (pair.Value.ConNum < 3)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.Tomato;
                    }
                    else if (pair.Value.ConNum >= 3 && pair.Value.ConNum <= 5)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.Red;
                    }
                    else if (pair.Value.ConNum > 5)
                    {
                        pair.Key.Fill = System.Windows.Media.Brushes.DarkRed;
                    }
                }
            }
        }

    }
}
