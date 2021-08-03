using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Segments
{

    public partial class SegmentsForm : Form
    {
        public SegmentsForm()
        {
            InitializeComponent();

            //Установка заголовков осей и графика
            zedGraphControl1.GraphPane.Title.Text = "Плоскость XY";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "X";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "Y";
            zedGraphControl1.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zedGraphControl1.GraphPane.YAxis.MajorGrid.IsVisible = true;

            zedGraphControl2.GraphPane.Title.Text = "Плоскость XY";
            zedGraphControl2.GraphPane.XAxis.Title.Text = "X";
            zedGraphControl2.GraphPane.YAxis.Title.Text = "Y";
            zedGraphControl2.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zedGraphControl2.GraphPane.YAxis.MajorGrid.IsVisible = true;
        }       

        //Функция проверки на возможность пересечения прямых, на которых лежат отрезки s1 и s2
        private int CheckForPossibleIntersection(Segment s1, Segment s2)
        {
            if(s1.k == s2.k)
            {
                if((s1.b == s2.b && !s1.IsVertical && !s2.IsVertical) || (s1.IsVertical && s2.IsVertical && s1.SP.X == s2.SP.X))
                {
                    return 1; //Прямые совпадают
                }
                else
                {
                    return -1; //Прямые параллельны
                }
            }
            else
            {
                return 0; //Прямые пересекаются
            }
        }

        //Функция нахождения координат точки пересечения двух прямых
        private ZedGraph.PointPair FindIntersectionPoint(Segment s1, Segment s2)
        {
            ZedGraph.PointPair IPoint = new ZedGraph.PointPair(); //Точка пересечения

            //Если обе прямые наклонные
            if (!s1.IsVertical && !s2.IsVertical)
            {
               IPoint.X = (s2.b - s1.b) / (s1.k - s2.k);
               IPoint.Y = (s1.k * s2.b - s2.k * s1.b) / (s1.k - s2.k);
            }
            else
            {
                //Если первая прямая вертикальная
                if(s1.IsVertical)
                {
                    IPoint.X = s1.SP.X;
                    IPoint.Y = s2.k * IPoint.X + s2.b;
                }
                //Иначе, вторая прямая вертикальная
                else
                {                                        
                    IPoint.X = s2.SP.X;
                    IPoint.Y = s1.k * IPoint.X + s1.b;                    
                }
            }

            return IPoint;
        }

        //Функция проверки принадлежности точки к отрезку
        private bool CheckForAffiliation(ZedGraph.PointPair p, Segment s)
        {
            return (p.X >= s.SP.X && p.Y >= Math.Min(s.SP.Y, s.FP.Y) && p.X <= s.FP.X &&
                p.Y <= Math.Max(s.SP.Y, s.FP.Y));
        }

        //Вызывается при нажатии на кнопку "СТАРТ"
        private void StartButton_Click(object sender, EventArgs e)
        {
            var panel = zedGraphControl1.GraphPane;
            var panel2 = zedGraphControl2.GraphPane;
            var NumSegments = Convert.ToInt32(NumOfSegmentsTB.Text); //Число отрезков           
            var SegmentsFilePath = Path.Combine(Environment.CurrentDirectory, "Segments.txt"); //Путь к файлу с отрезками 
            var IntersectionsFilePath = Path.Combine(Environment.CurrentDirectory, "Intersections.txt"); //Путь к файлу с пересечениями
            var rand = new Random(); //Генератор случайных чисел          
            var StrToWrite = string.Empty;
            var Writer = new StreamWriter(SegmentsFilePath, false, Encoding.Default);           
            var SList = new List<Segment>(NumSegments); //Список созданных отрезков
            ZedGraph.PointPair P1 = new ZedGraph.PointPair(), P2 = new ZedGraph.PointPair(); //Точки концов отрезка                                          
            double r; //Случайные числа            

            panel.CurveList.Clear();
            panel2.CurveList.Clear();

            for (int i = 0; i < NumSegments; i++)
            {
                //Генерация координат конечных точек отрезков в диапазоне [-1000; 1000]
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P1.X = r;                
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P1.Y = r;               
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P2.X = r;               
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P2.Y = r;

                var L = new ZedGraph.PointPairList();                

                Segment S = new Segment(P1, P2);
                //Добавление отрезка в список отрезков
                SList.Add(S);
                L.Add(S.SP);
                L.Add(S.FP);

                var seg = panel2.AddCurve("", L, Color.Red, ZedGraph.SymbolType.Circle);
                seg.Symbol.Fill.Color = Color.Red;
                seg.Symbol.Fill.Type = ZedGraph.FillType.Solid;

                //Запись координат в файл Segments.txt
                StrToWrite = string.Format("SP:({0}; {1})  FP:({2}; {3})", S.SP.X, S.SP.Y,
                   S.FP.X, S.FP.Y);
                Writer.WriteLine(StrToWrite);

            }
            Writer.Dispose();

            Segment cur = new Segment(); //Текущий рассматриваемый отрезок
            var ISegmentsList = new List<List<Segment>>(); //Список отрезков, которые пересекаются с текущим            
            var IPointsList = new ZedGraph.PointPairList(); //Список точек пересечения для текущего отрезка
            var PossibleIPoint = new ZedGraph.PointPair(); //Возможная точка пересечения
            Writer = new StreamWriter(IntersectionsFilePath, false, Encoding.Default);           

            //Проверка отрезка на возможность пересечения с остальными и нахождения координат точек пересечения
            for (int i = 0; i < SList.Count; i++)
            {
                //Если текущий отрезок был уже разделен, то пропускаем его               
                if (!SList[i].Divided)
                {
                    foreach (var item in ISegmentsList)
                    {
                        item.Clear();
                    }
                    ISegmentsList.Clear();
                    IPointsList.Clear();                   
                    cur = SList[i];
                    //Проверяем все остальные неразделенные отрезки на возможность пересечения и, если пересечение возможно,
                    //вычисляем координаты точки пересечения
                    for (int j = 0; j < SList.Count; j++)
                    {
                        //Если отрезок не совпадает с текущим, не был уже разделен и пересечение возможно
                        if (cur != SList[j] && CheckForPossibleIntersection(cur, SList[j]) != -1)
                        {
                            //Отдельно рассмотрим случай совпадающих прямых
                            //Если отрезки имеют бесконечное число точек пересечения, то точкой пересечения
                            //будет крайняя точка текущего отрезка
                            if (CheckForPossibleIntersection(cur, SList[j]) == 1)
                            {
                                //Проверка на принадлежность хотя бы одной крайней точки первого отрезка второму
                                if (CheckForAffiliation(cur.SP, SList[j])) //Для начальной точки
                                {                                   
                                    //Если точки нет в списке, то добавляем её и создаем новый подсписок пересекающихся отрезков
                                    //для этой точки
                                    if (!IPointsList.Contains(cur.SP))
                                    {
                                        IPointsList.Add(cur.SP);
                                        ISegmentsList.Add(new List<Segment>());
                                        ISegmentsList[ISegmentsList.Count - 1].Add(SList[j]);
                                    }
                                    //Если точка пересечения уже есть в списке, то добавляем только отрезок в список пересекающихся
                                    //отрезков для соответствующей точки пересечения
                                    else
                                    {
                                        ISegmentsList[IPointsList.IndexOf(cur.SP)].Add(SList[j]);
                                    }
                                }
                                else
                                {
                                    if (CheckForAffiliation(cur.FP, SList[j])) //Для конечной точки
                                    {
                                        //Если точки нет в списке, то добавляем её и создаем новый подсписок пересекающихся
                                        //отрезков для этой точки
                                        if (!IPointsList.Contains(cur.FP))
                                        {
                                            IPointsList.Add(cur.FP);
                                            ISegmentsList.Add(new List<Segment>());
                                            ISegmentsList[ISegmentsList.Count - 1].Add(SList[j]);
                                        }
                                        //Если точка пересечения уже есть в списке, то добавляем только отрезок в список
                                        //пересекающихся отрезков для соответствующей точки пересечения
                                        else
                                        {
                                            ISegmentsList[IPointsList.IndexOf(cur.FP)].Add(SList[j]);
                                        }
                                    }
                                }
                            }
                            //Или для пересекающихся прямых
                            else
                            {
                                //Вычисление координат точки пересечения двух прямых, на которых лежат отрезки
                                PossibleIPoint = FindIntersectionPoint(cur, SList[j]);
                                //Проверка принадлежности точки пересечения обоим отрезкам
                                if (CheckForAffiliation(PossibleIPoint, cur) && CheckForAffiliation(PossibleIPoint, SList[j]))
                                {
                                    //Если точки нет в списке, то добавляем её и создаем новый подсписок пересекающихся
                                    //отрезков для этой точки
                                    if (!IPointsList.Contains(PossibleIPoint))
                                    {
                                        IPointsList.Add(PossibleIPoint); //Добавим точку пересечения в список
                                        ISegmentsList.Add(new List<Segment>());
                                        ISegmentsList[ISegmentsList.Count - 1].Add(SList[j]);
                                    }
                                    // Если точка пересечения уже есть в списке, то добавляем только отрезок в список
                                    //пересекающихся отрезков для соответствующей точки пересечения
                                    else
                                    {
                                        ISegmentsList[IPointsList.IndexOf(PossibleIPoint)].Add(SList[j]);
                                    }                                                                      
                                }
                            }
                        }
                    }
                    //Если список точек пересечения с текущим не пуст
                    if (IPointsList.Count != 0)
                    {                                              
                        //Добавим текущий отрезок в каждый из списков отрезков для точек пересечения
                        foreach (var item in ISegmentsList)
                        {
                            item.Add(cur);
                        }

                        var curPoints = new ZedGraph.PointPairList(); //Список точек пересечения, для которых текущий отрезок 
                                                                      //обладает наибольшим наклоном

                        foreach (var item in ISegmentsList)
                        {
                            double maxAbsK = Math.Abs(item[0].k);//Максимальное значение наклона по модулю к горизонтальной оси
                            Segment maxKSeg = item[0]; //Отрезок с наибольшим наклоном

                            for(int INDEX = 1; INDEX < item.Count; INDEX++)
                            {
                                if (Math.Abs(item[INDEX].k) > maxAbsK)
                                {
                                    maxAbsK = Math.Abs(item[INDEX].k);
                                    maxKSeg = item[INDEX];
                                }
                            }

                            //Если для текущей точки пересечения отрезок cur обладает максимальным по модулю наклоном
                            if(maxKSeg == cur)
                            {
                                curPoints.Add(IPointsList[ISegmentsList.IndexOf(item)]);
                            }
                        }

                        //Если текущий отрезок обладает наибольшим наклоном хотя бы для одной точки пересечения
                        if (curPoints.Count != 0)
                        {
                            //Пометим текущий отрезок как разделенный
                            Segment Temp = new Segment(cur); //Создаем копию
                            SList.RemoveAt(i); //Удаляем старый отрезок из SList
                            Temp.Divided = true; //Устанавливаем флаг Divided в значение true
                            SList.Insert(i, Temp); //Вставляем исправленый отрезок на старое место                           

                            //Если среди точек пересечения нет крайних точек отрезка                          
                            if (curPoints.Contains(cur.SP) && curPoints.Contains(cur.FP))
                            {
                                curPoints.Remove(cur.SP);
                                curPoints.Remove(cur.FP);
                            }
                            else
                            {
                                if (curPoints.Contains(cur.SP))
                                {
                                    curPoints.Remove(cur.SP);
                                }
                                else
                                {
                                    if (curPoints.Contains(cur.FP))
                                    {
                                        curPoints.Remove(cur.FP);
                                    }
                                }
                            }

                            Segment[] Segs = new Segment[curPoints.Count + 1]; //Массив отрезков, на которые делится текущий

                            curPoints.Sort(); //Отсортируем список точек
                                              //Добавим начальную и конечную точки 
                            curPoints.Insert(0, cur.SP);
                            curPoints.Add(cur.FP);

                            for (int INDEX = 0; INDEX < Segs.Length; INDEX++)
                            {
                                Segs[INDEX] = new Segment();
                                //Инициализация параметров новых отрезков
                                Segs[INDEX].SP = curPoints[INDEX].Clone();
                                Segs[INDEX].FP = curPoints[INDEX + 1].Clone();
                                Segs[INDEX].b = cur.b;
                                Segs[INDEX].Divided = true;
                                Segs[INDEX].IsVertical = cur.IsVertical;
                                Segs[INDEX].k = cur.k;
                            }

                            for (int INDEX = 0; INDEX < Segs.Length; INDEX++)
                            {
                                //Каждый подотрезок смещаем вверх-вниз

                                if (Segs[INDEX].k > 0)
                                {
                                    //Каждый четный вниз на 1, а нечетный вверх на 1
                                    if (INDEX % 2 == 0)
                                    {
                                        Segs[INDEX].SP.Y -= 1.0;
                                        Segs[INDEX].FP.Y -= 1.0;
                                    }
                                    else
                                    {
                                        Segs[INDEX].SP.Y += 1.0;
                                        Segs[INDEX].FP.Y += 1.0;
                                    }
                                }
                                else
                                {
                                    //Каждый четный вверх на 1, а нечетный вниз на 1
                                    if (INDEX % 2 == 0)
                                    {
                                        Segs[INDEX].SP.Y += 1.0;
                                        Segs[INDEX].FP.Y += 1.0;
                                    }
                                    else
                                    {
                                        Segs[INDEX].SP.Y -= 1.0;
                                        Segs[INDEX].FP.Y -= 1.0;
                                    }
                                }

                                //Запись координат в файл 
                                StrToWrite += string.Format("Seg{0}: SP:({1}; {2})  FP:({3}; {4})\t", INDEX + 1,
                                   Math.Round(Segs[INDEX].SP.X, 2), Math.Round(Segs[INDEX].SP.Y, 2),
                                   Math.Round(Segs[INDEX].FP.X, 2), Math.Round(Segs[INDEX].FP.Y, 2));

                                ZedGraph.PointPairList L = new ZedGraph.PointPairList();
                                L.Add(Math.Round(Segs[INDEX].SP.X, 2), Math.Round(Segs[INDEX].SP.Y, 2));
                                L.Add(Math.Round(Segs[INDEX].FP.X, 2), Math.Round(Segs[INDEX].FP.Y, 2));

                                var seg = zedGraphControl1.GraphPane.AddCurve("", L, Color.Green,
                            ZedGraph.SymbolType.Circle);
                                seg.Symbol.Fill.Color = Color.Green;
                                seg.Symbol.Fill.Type = ZedGraph.FillType.Solid;
                            }
                            Writer.WriteLine(StrToWrite);
                        }                                                                                                                                                               
                    }                                                   
                }
            }

            //Отрисуем на плоскости XY все оставшиеся неразделенные отрезки
            for(int i = 0; i < SList.Count; i++)
            {
                if(!SList[i].Divided)
                {
                    ZedGraph.PointPairList L = new ZedGraph.PointPairList();
                    L.Add(SList[i].SP);
                    L.Add(SList[i].FP);
                    var seg = zedGraphControl1.GraphPane.AddCurve("", L, Color.Red, ZedGraph.SymbolType.Circle);
                    seg.Symbol.Fill.Color = Color.Red;
                    seg.Symbol.Fill.Type = ZedGraph.FillType.Solid;
                }
            }

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl2.AxisChange();
            zedGraphControl2.Invalidate();
            Writer.Dispose();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            zedGraphControl2.Visible = true;
            zedGraphControl1.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            zedGraphControl2.Visible = false;
            zedGraphControl1.Visible = true;
        }
    }

    //Класс Отрезок
    public class Segment
    {
        public ZedGraph.PointPair SP, FP; //Начальная и конечная точки
        public double k, b; //Коэффициент наклона и сдвиг по оси ординат
        public bool Divided; //Флаг, показывающий произошло ли разделение отрезка на 2 части
        public bool IsVertical; //Флаг, показывающий является ли отрезок перпендикулярным оси абсцисс              
        
        public Segment() { }

        //Конструктор копирования
        public Segment(Segment s)
        {
            SP = s.SP.Clone();
            FP = s.FP.Clone();
            k = s.k;
            b = s.b;
            Divided = s.Divided;
            IsVertical = s.IsVertical;
        }

        //Конструктор инициализации. Начальная точка SP всегда будет с меньшей координатой X
        public Segment(ZedGraph.PointPair _SP, ZedGraph.PointPair _FP)
        {
            Divided = false;
            if(_SP.X > _FP.X)
            {
                SP = _FP.Clone();
                FP = _SP.Clone();
                k = Math.Round((FP.Y - SP.Y) / (FP.X - SP.X), 2);
                b = Math.Round((SP.Y * FP.X - FP.Y * SP.X) / (FP.X - SP.X), 2);
                IsVertical = false;
            }
            else
            {
                if(_SP.X < _FP.X)
                {
                    SP = _SP.Clone();
                    FP = _FP.Clone();
                    k = Math.Round((FP.Y - SP.Y) / (FP.X - SP.X), 2);
                    b = Math.Round((SP.Y * FP.X - FP.Y * SP.X) / (FP.X - SP.X), 2);
                    IsVertical = false;
                }
                else
                {
                    SP = _SP.Clone();
                    FP = _FP.Clone();
                    k = double.PositiveInfinity;
                    b = double.NaN;
                    IsVertical = true;
                }
            }
        }
        
        public static bool operator !=(Segment left, Segment right)
        {
            return (!left.SP.Equals(right.SP) || !left.FP.Equals(right.FP));
        }
        public static bool operator ==(Segment left, Segment right)
        {
            return (left.SP.Equals(right.SP) && left.FP.Equals(right.FP));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !GetType().Equals(o.GetType()))
            {
                return false;
            }
            else
            {
                Segment s = (Segment)o;
                return (SP.Equals(s.SP) && FP.Equals(s.FP));
            }            
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }   
}