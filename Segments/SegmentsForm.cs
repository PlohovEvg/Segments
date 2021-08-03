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
               IPoint.X = Math.Round((s2.b - s1.b) / (s1.k - s2.k), 2);
               IPoint.Y = Math.Round((s1.k * s2.b - s2.k * s1.b) / (s1.k - s2.k), 2);
            }
            else
            {
                //Если первая прямая вертикальная
                if(s1.IsVertical)
                {
                    IPoint.X = Math.Round(s1.SP.X, 2);
                    IPoint.Y = Math.Round(s2.k * IPoint.X + s2.b, 2);
                }
                //Иначе, вторая прямая вертикальная
                else
                {                                        
                    IPoint.X = s2.SP.X;
                    IPoint.Y = Math.Round(s1.k * IPoint.X + s1.b, 2);                    
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
                P1.X = Math.Round(r, 2);                
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P1.Y = Math.Round(r, 2);               
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P2.X = Math.Round(r, 2);               
                r = rand.NextDouble() * 2001.0 - 1000.0;
                P2.Y = Math.Round(r, 2);

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
            var ISegmentsList = new List<Segment>(); //Список отрезков, которые пересекаются с текущим
            var IndsList = new List<int>(); //Список индексов отрезков в SList, которые пересекаются с текущим
            var IPointsList = new ZedGraph.PointPairList(); //Список точек пересечения для текущего отрезка
            var PossibleIPoint = new ZedGraph.PointPair(); //Возможная точка пересечения
            Writer = new StreamWriter(IntersectionsFilePath, false, Encoding.Default);           

            //Проверка отрезка на возможность пересечения с остальными и нахождения координат точек пересечения
            for (int i = 0; i < SList.Count; i++)
            {
                //Если текущий отрезок был уже разделен, то пропускаем его               
                if (!SList[i].Divided)
                {
                    ISegmentsList.Clear();
                    IPointsList.Clear();
                    IndsList.Clear();
                    cur = SList[i];
                    //Проверяем все остальные неразделенные отрезки на возможность пересечения и, если пересечение возможно,
                    //вычисляем координаты точки пересечения
                    for (int j = 0; j < SList.Count; j++)
                    {
                        //Если отрезок не совпадает с текущим, не был уже разделен и пересечение возможно
                        if (cur != SList[j] && !SList[j].Divided && CheckForPossibleIntersection(cur, SList[j]) != -1)
                        {
                            //Отдельно рассмотрим случай совпадающих прямых
                            //Если отрезки имеют бесконечное число точек пересечения, то точкой пересечения
                            //будет крайняя точка текущего отрезка
                            if (CheckForPossibleIntersection(cur, SList[j]) == 1)
                            {
                                //Проверка на принадлежность хотя бы одной крайней точки первого отрезка второму
                                if (CheckForAffiliation(cur.SP, SList[j])) //Для начальной точки
                                {
                                    ISegmentsList.Add(SList[j]);
                                    IndsList.Add(j);
                                    IPointsList.Add(cur.SP);
                                }
                                else
                                {
                                    if (CheckForAffiliation(cur.FP, SList[j])) //Для конечной точки
                                    {
                                        ISegmentsList.Add(SList[j]);
                                        IndsList.Add(j);
                                        IPointsList.Add(cur.FP);
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
                                    ISegmentsList.Add(SList[j]); //Отрезок пересекается с текущим. Добавим его в список
                                    IndsList.Add(j); //Добавим его индекс в список
                                    IPointsList.Add(PossibleIPoint); //Добавим точку пересечения в список                                   
                                }
                            }
                        }
                    }
                    //Если список отрезков пересекающихся с текущим не пуст, то ищем отрезок с наибольшим наклоном по модулю
                    //и делим его на два отрезка с зазором в два единичных отрезка
                    if (ISegmentsList.Count != 0)
                    {
                        double maxAbsK = Math.Abs(ISegmentsList[0].k);//Максимальное по модулю значение наклона к горизонтальной оси
                        int Ind = 0; //Индекс отрезка в списке ISegmentsList, которому соответствует максимальный наклон
                        ISegmentsList.Add(cur);
                        IndsList.Add(i);
                        IPointsList.Add(IPointsList[0]);

                        //Поиск среди пересекающихся отрезков отрезка с максимальным наклоном
                        for(int ind = 1; i < ISegmentsList.Count; i++)
                        {
                            if(Math.Abs(ISegmentsList[ind].k) > maxAbsK)
                            {
                                maxAbsK = Math.Abs(ISegmentsList[ind].k);
                                Ind = ind;
                            }
                        }

                        //Пометим выбранный отрезок как разделенный в списке SList
                        //Для этого создадим его копию, изменим в ней флаг Divided удалим старый отрезок из списка и на его
                        //место добавим копию 
                        Segment Temp = new Segment(SList[IndsList[Ind]]); //Создаем копию
                        SList.RemoveAt(IndsList[Ind]); //Удаляем старый отрезок из SList
                        Temp.Divided = true; //Устанавливаем флаг Divided в значение true
                        SList.Insert(IndsList[Ind], Temp); //Вставляем исправленый отрезок на старое место

                        //Если точка пересечения не является крайней точкой отрезка
                        if (!IPointsList[Ind].Equals(ISegmentsList[Ind].SP) && !IPointsList[Ind].Equals(ISegmentsList[Ind].FP))
                        {                            
                            //Создаем два новых смещенных отрезка, записываем их в файл Intersections.txt и отображаем на плоскости
                            Segment S1 = new Segment(), S2 = new Segment();

                            //Инициализация параметров
                            S1.SP = ISegmentsList[Ind].SP.Clone();
                            S1.FP = IPointsList[Ind].Clone();
                            S1.k = ISegmentsList[Ind].k;
                            S1.b = ISegmentsList[Ind].b;
                            S1.IsVertical = ISegmentsList[Ind].IsVertical;
                            S1.Divided = true;
                            S2.SP = IPointsList[Ind].Clone();
                            S2.FP = ISegmentsList[Ind].FP.Clone();
                            S2.k = ISegmentsList[Ind].k;
                            S2.b = ISegmentsList[Ind].b;
                            S2.IsVertical = ISegmentsList[Ind].IsVertical;
                            S2.Divided = true;

                            //Если среди пересекающихся отрезков нет вертикальных, то отрезок S1 сдвигаем вниз на один единичный
                            //отрезок, а S2 вверх
                            if(!ISegmentsList.Exists(x => x.IsVertical == true))
                            {
                                S1.SP.Y -= 1.0;
                                S1.FP.Y -= 1.0;
                                S2.SP.Y += 1.0;
                                S2.FP.Y += 1.0;
                            }
                            //Если вертикальный отрезок есть, то отрезок S1 сдвигаем влево на один единичный
                            //отрезок, а S2 вправо
                            else
                            {
                                S1.SP.X -= 1.0;
                                S1.FP.X -= 1.0;
                                S2.SP.X += 1.0;
                                S2.FP.X += 1.0;
                            }

                            //Запись координат новых отрезков в файл Intersections.txt
                            StrToWrite = string.Format("Seg1: SP:({0}; {1})  FP:({2}; {3})\tSeg2: SP:({4}; {5})  FP:({6}; {7})",
                               S1.SP.X, S1.SP.Y, S1.FP.X, S1.FP.Y, S2.SP.X, S2.SP.Y, S2.FP.X, S2.FP.Y);
                            Writer.WriteLine(StrToWrite);

                            //Отображение отрезков на плоскости XY
                            ZedGraph.PointPairList L = new ZedGraph.PointPairList();
                            ZedGraph.PointPairList L2 = new ZedGraph.PointPairList();
                            L.Add(S1.SP);
                            L.Add(S1.FP);
                            L2.Add(S2.SP);
                            L2.Add(S2.FP);
                            var seg = zedGraphControl1.GraphPane.AddCurve("", L, Color.Green, 
                                ZedGraph.SymbolType.Circle);
                            var seg2 = zedGraphControl1.GraphPane.AddCurve("", L2, Color.Green,
                                ZedGraph.SymbolType.Circle);

                            seg.Symbol.Fill.Color = Color.Green;
                            seg.Symbol.Fill.Type = ZedGraph.FillType.Solid;
                            seg2.Symbol.Fill.Color = Color.Green;
                            seg2.Symbol.Fill.Type = ZedGraph.FillType.Solid;

                        }
                        //Если точка пересечения является крайней точкой отрезка, который нужно разделить, то сдвинем этот
                        //отрезок целиком на два единичных отрезка вверх или вправо 
                        else
                        {
                            if (!ISegmentsList.Exists(x => x.IsVertical == true))
                            {
                                ISegmentsList[Ind].SP.Y += 2.0;
                                ISegmentsList[Ind].FP.Y += 2.0;
                            }
                            else
                            {
                                ISegmentsList[Ind].SP.X += 2.0;
                                ISegmentsList[Ind].FP.X += 2.0;
                            }

                            //Запись координат отрезка в файл Intersections.txt
                            StrToWrite = string.Format("Seg: SP:({0}; {1})  FP:({2}; {3})", 
                                ISegmentsList[Ind].SP.X, ISegmentsList[Ind].SP.Y, ISegmentsList[Ind].FP.X, ISegmentsList[Ind].FP.Y);
                            Writer.WriteLine(StrToWrite);

                            //Отображение отрезка на плоскости XY
                            var L = new ZedGraph.PointPairList();                          
                            L.Add(ISegmentsList[Ind].SP);
                            L.Add(ISegmentsList[Ind].FP);                            
                            var seg = zedGraphControl1.GraphPane.AddCurve("", L, Color.Green,
                                ZedGraph.SymbolType.Circle);
                            seg.Symbol.Fill.Color = Color.Green;
                            seg.Symbol.Fill.Type = ZedGraph.FillType.Solid;                            
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