using LiveBoard.Class;
using LiveBoard.ProtocolUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LiveBoard
{
    public sealed partial class BoardPage : Page
    {
        public BoardPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0.3;
            animation.To = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)").Path);
            story.Children.Add(animation);
            story.Begin();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            #region 房间信息初始化
            RoomIDTextBlock.Text = App.currentRoom.RoomID.ToString();
            RoomTiTleTextBlock.Text = App.currentRoom.RoomTitle;
            PresentNumberTextBlock.Text = App.currentRoom.PresentNumber.ToString();
            MaximumNumberTextBlock.Text = App.currentRoom.MaximumNumber.ToString();
            #endregion

            #region Shape ComboBox初始化
            List<dynamic> shapeList = new List<dynamic>();
            shapeList.Add(new { Icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Point.png")), ShapeName = "Point" });
            shapeList.Add(new { Icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Line.png")), ShapeName = "Line" });
            shapeList.Add(new { Icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Rectangle.png")), ShapeName = "Rectangle" });
            shapeList.Add(new { Icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Eraser.png")), ShapeName = "Eraser" });
            shapeList.Add(new { Icon = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/TextBox.png")), ShapeName = "TextBox" });
            ShapeComboBox.ItemsSource = shapeList;
            ShapeComboBox.SelectedIndex = 0;
            #endregion

            #region Color ComboBox初始化
            List<dynamic> colorList = new List<dynamic>();
            colorList.Add(new { Brush = new SolidColorBrush(Colors.Red), ColorName = "Red" });
            colorList.Add(new { Brush = new SolidColorBrush(Colors.Yellow), ColorName = "Yellow" });
            colorList.Add(new { Brush = new SolidColorBrush(Colors.Green), ColorName = "Green" });
            colorList.Add(new { Brush = new SolidColorBrush(Colors.Blue), ColorName = "Blue" });
            colorList.Add(new { Brush = new SolidColorBrush(Colors.Black), ColorName = "Black" });
            ColorComboBox.ItemsSource = colorList;
            ColorComboBox.SelectedIndex = 0;
            #endregion

            #region DrawAction初始化
            App.currentAction.Shape = "Point";
            App.currentAction.Color = "Red";
            App.currentAction.PointSize = 10;
            App.currentAction.Thickness = 5;
            #endregion

            #region 读取转发数据
            while (isReadData)
            {
                try
                {
                    engine.ReceiveData(SchemaDef.ACTION);
                    if (App.responseResult.command == "draw" && App.receivedAction.Coordinates.Count != 0)
                    {
                        AddToCanvas(App.receivedAction, 0);
                        App.receivedAction.Coordinates.Clear();
                    }
                    else
                        await Task.Delay(1);
                }
                catch
                {

                }
            }
            #endregion
        }

        private async void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isReadData = false;
                CProtocolEngine engine = new CProtocolEngine();
                engine.Request(SchemaDef.ROOMEXIT, App.currentUser, App.currentRoom);
                engine.ReceiveData(SchemaDef.ROOMEXIT);
                bool isExitSucceed = false;
                while(!isExitSucceed)
                {
                    await Task.Delay(100);
                    if (App.responseResult.command == "roomExit" && App.responseResult.code == 1)
                    {
                        isExitSucceed = true;
                        engine.Request(SchemaDef.ROOMLIST, App.currentUser, null);
                        engine.ReceiveData(SchemaDef.ROOMLIST);
                        await Task.Delay(300);
                        Frame.GoBack();
                    }
                    if (App.responseResult.command == "roomExit" && App.responseResult.code == 0)
                    {
                        isExitSucceed = true;
                        App.ShowMessage(App.responseResult.tips, "Error");
                    }
                }
            }
            catch (Exception exception)
            {
                App.ShowMessage(exception.Message, "Error");
            }
        }

        #region Draw with Undo & Redo
        //private int actionCount = -1;
        //private int clickCount = -1;
        //private List<int> actionStartPoint = new List<int>();
        //private List<int> actionEndPoint = new List<int>();
        //private List<dynamic> actionList = new List<dynamic>();

        //private void myCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    drawStarted = true;
        //    var point = e.GetCurrentPoint(this.myCanvas);
        //    tempX = point.Position.X;
        //    tempY = point.Position.Y;
        //    actionCount++;
        //    clickCount++;
        //    if (actionCount < actionList.Count)
        //    {
        //        for (int i = 0; i < actionEndPoint[actionEndPoint.Count - 1] - actionStartPoint[clickCount] + 1; i++)
        //        {
        //            actionList.RemoveAt(actionStartPoint[clickCount]);
        //        }
        //        int j = actionStartPoint.Count - clickCount;
        //        for (int i = 0; i < j; i++)
        //        {
        //            actionStartPoint.RemoveAt(clickCount);
        //            actionEndPoint.RemoveAt(clickCount);
        //        }
        //    }
        //    actionStartPoint.Add(actionCount);
        //    if (currentShape == "Point")
        //    {
        //        Draw_Point(point.Position.X, point.Position.Y, currentBrush, pointsize);
        //        actionList.Add(tempElp);
        //    }
        //    else if (currentShape=="Eraser")
        //    {
        //        Brush tempBrush = currentBrush;
        //        SetColor(new SolidColorBrush(Colors.White));
        //        Draw_Point(point.Position.X, point.Position.Y, currentBrush, pointsize);
        //        actionList.Add(tempElp);
        //        currentBrush = tempBrush;
        //    }
        //}

        //private void myCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    if (drawStarted == true)
        //    {
        //        var point = e.GetCurrentPoint(this.myCanvas);
        //        if (currentShape == "Point")
        //        {
        //            double tempSize = thickness;
        //            SetLineThickness(pointsize);
        //            Draw_Line(tempElp.Margin.Left + pointsize / 2.0, point.Position.X, tempElp.Margin.Top + pointsize / 2.0, point.Position.Y, currentBrush, thickness);
        //            Draw_Point(point.Position.X, point.Position.Y, currentBrush, pointsize);
        //            actionList.Add(tempElp);
        //            actionList.Add(tempLine);
        //            actionCount += 2;
        //            thickness = tempSize;
        //        }
        //        else if (currentShape == "Eraser")
        //        {
        //            double tempSize = thickness;
        //            Brush tempBrush = currentBrush;
        //            SetLineThickness(pointsize);
        //            SetColor(new SolidColorBrush(Colors.White));
        //            Draw_Line(tempElp.Margin.Left + pointsize / 2.0, point.Position.X, tempElp.Margin.Top + pointsize / 2.0, point.Position.Y, currentBrush, thickness);
        //            Draw_Point(point.Position.X, point.Position.Y, currentBrush, pointsize);
        //            actionList.Add(tempElp);
        //            actionList.Add(tempLine);
        //            actionCount += 2;
        //            thickness = tempSize;
        //            currentBrush = tempBrush;
        //        }
        //        else if (currentShape == "Line")
        //        {
        //            if (isLastOne == false)
        //            {
        //                this.myCanvas.Children.Remove(tempLine);
        //                actionList.Remove(tempLine);
        //            }
        //            else
        //                isLastOne = false;
        //            Draw_Line(tempElp.Margin.Left + pointsize / 2.0, point.Position.X, tempElp.Margin.Top + pointsize / 2.0, point.Position.Y, currentBrush, thickness);
        //            actionList.Add(tempLine);
        //        }
        //        else if (currentShape == "Rectangle")
        //        {
        //            if (isLastOne == false)
        //            {
        //                this.myCanvas.Children.Remove(tempRec);
        //                actionList.Remove(tempRec);
        //            }
        //            else
        //                isLastOne = false;
        //            Draw_Rectangle(tempX, point.Position.X, tempY, point.Position.Y, currentBrush, thickness);
        //            actionList.Add(tempRec);
        //        }
        //        else
        //        {
        //            if (isLastOne == false)
        //            {
        //                this.myCanvas.Children.Remove(tempTB);
        //                actionList.Remove(tempTB);
        //            }
        //            else
        //                isLastOne = false;
        //            Draw_TextBox(tempX, point.Position.X, tempY, point.Position.Y, currentBrush, null);
        //            actionList.Add(tempTB);
        //        }
        //    }
        //}

        //private void myCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        //{
        //    drawStarted = false;
        //    isLastOne = true;
        //    actionEndPoint.Add(actionCount);
        //    if (currentShape == "TextBox")
        //    {
        //        tempTB.Height = Double.NaN;
        //        tempTB.BorderBrush = Color_Transparent;
        //        tempTB.Focus(FocusState.Programmatic); 
        //    }
        //}
        #endregion

        #region Draw
        CProtocolEngine engine = new CProtocolEngine();
        private bool isReadData = true;
        private bool isDrawStarted = false;
        private bool isLastOne = true;
        private bool isTextBoxNeedToSent = true;
        private bool isMouseMoved = false;
        private string tempContent;
        private string tempColor;
        private Ellipse tempElp = new Ellipse();
        private Line tempLine = new Line();
        private Rectangle tempRec = new Rectangle();
        private TextBox tempTB = new TextBox();
        private Brush Color_Transparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        private void myCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isDrawStarted = true;
            if (tempTB.Text == "")
            {
                this.myCanvas.Children.Remove(tempTB);
                isTextBoxNeedToSent = false;
            }
            if (isTextBoxNeedToSent)
            {
                App.currentAction.Text = tempTB.Text;
                engine.Request(SchemaDef.ACTION, App.currentUser, App.currentAction);
                isTextBoxNeedToSent = false;
            }

            App.currentAction.Coordinates.Clear();
            Coordinate tempCoordinate = new Coordinate();
            var point = e.GetCurrentPoint(this.myCanvas);
            tempCoordinate.X = GetShort(point.Position.X);
            tempCoordinate.Y = GetShort(point.Position.Y);
            App.currentAction.Coordinates.Add(tempCoordinate);

            switch (App.currentAction.Shape)
            {
                case "Point":
                    {
                        AddToCanvas(App.currentAction, 1);
                        break;
                    }
                case "Eraser":
                    {
                        tempColor = App.currentAction.Color;
                        SetColor("White");
                        AddToCanvas(App.currentAction, 1);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void myCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDrawStarted == true)
            {
                Coordinate tempCoordinate = new Coordinate();
                var point = e.GetCurrentPoint(this.myCanvas);
                tempCoordinate.X = GetShort(point.Position.X);
                tempCoordinate.Y = GetShort(point.Position.Y);

                if (tempCoordinate.X != App.currentAction.Coordinates[App.currentAction.Coordinates.Count - 1].X || tempCoordinate.Y != App.currentAction.Coordinates[App.currentAction.Coordinates.Count - 1].Y)
                {
                    isMouseMoved = true;
                    if (App.currentAction.Coordinates.Count == 100)
                    {
                        engine.Request(SchemaDef.ACTION, App.currentUser, App.currentAction);
                        App.currentAction.Coordinates[0] = App.currentAction.Coordinates[99];
                        App.currentAction.Coordinates.RemoveRange(1, 99);
                    }
                    App.currentAction.Coordinates.Add(tempCoordinate);
                }
                else
                    isMouseMoved = false;

                switch (App.currentAction.Shape)
                {
                    case "Point":
                        {
                            if (isMouseMoved)
                            {
                                double tempSize = App.currentAction.Thickness;
                                SetLineThickness(App.currentAction.PointSize);
                                AddToCanvas(App.currentAction, 1);
                                SetLineThickness(tempSize);
                            }
                            break;
                        }
                    case "Eraser":
                        {
                            if (isMouseMoved)
                            {
                                double tempSize = App.currentAction.Thickness;
                                SetLineThickness(App.currentAction.PointSize);
                                AddToCanvas(App.currentAction, 1);
                                SetLineThickness(tempSize);
                            }
                            break;
                        }
                    case "Line":
                        {
                            if (!isLastOne)
                            {
                                this.myCanvas.Children.Remove(tempLine);
                                App.currentAction.Coordinates.RemoveAt(1);
                            }
                            else
                                isLastOne = false;
                            AddToCanvas(App.currentAction, 1);
                            break;
                        }
                    case "Rectangle":
                        {
                            if (!isLastOne)
                            {
                                this.myCanvas.Children.Remove(tempRec);
                                App.currentAction.Coordinates.RemoveAt(1);
                            }
                            else
                                isLastOne = false;
                            AddToCanvas(App.currentAction, 1);
                            break;
                        }
                    case "TextBox":
                        {
                            if (!isLastOne)
                            {
                                this.myCanvas.Children.Remove(tempTB);
                                App.currentAction.Coordinates.RemoveAt(1);
                            }
                            else
                                isLastOne = false;
                            App.currentAction.Text = "";
                            AddToCanvas(App.currentAction, 1);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void myCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDrawStarted = false;
            isLastOne = true;
            if (App.currentAction.Shape == "TextBox")
            {
                isTextBoxNeedToSent = true;
                tempTB.Height = Double.NaN;
                tempTB.BorderBrush = Color_Transparent;
                tempTB.Focus(FocusState.Programmatic);
            }
            else
            {
                engine.Request(SchemaDef.ACTION, App.currentUser, App.currentAction);
                if(App.currentAction.Shape=="Eraser")
                    SetColor(tempColor);
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
            {
                return;
            }
            dynamic item = (sender as ComboBox).SelectedItem as dynamic;
            if (item != null)
            {
                SetColor(item.ColorName);
            }
        }

        private void ShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
            {
                return;
            }
            dynamic item = (sender as ComboBox).SelectedItem as dynamic;
            if (item != null)
            {
                SetShape(item.ShapeName);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.myCanvas.Children.Clear();
        }

        private void Draw_Point(double x, double y, Brush brush, double pointsize)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = brush;
            ellipse.Fill = brush;
            ellipse.Height = pointsize;
            ellipse.Width = pointsize;
            ellipse.Margin = new Thickness(x - pointsize / 2.0, y - pointsize / 2.0, 0, 0);
            tempElp = ellipse;
            this.myCanvas.Children.Add(tempElp);
        }

        private void Draw_Line(double x1, double x2, double y1, double y2, Brush brush, double thickness)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.StrokeThickness = thickness;
            line.Stroke = brush;
            tempLine = line;
            this.myCanvas.Children.Add(tempLine);
        }

        private void Draw_Rectangle(double x1, double x2, double y1, double y2, Brush brush, double thickness)
        {
            Rectangle rec = new Rectangle();
            rec.StrokeThickness = thickness;
            rec.Stroke = brush;
            rec.Height = Math.Abs(y2 - y1);
            rec.Width = Math.Abs(x2 - x1);
            rec.Margin = new Thickness(Math.Min(x1, x2), Math.Min(y1, y2), 0, 0);
            rec.Fill = Color_Transparent;
            tempRec = rec;
            this.myCanvas.Children.Add(tempRec);
        }

        private void Draw_TextBox(double x1, double x2, double y1, double y2, Brush brush, String text)
        {
            TextBox tb = new TextBox();
            tb.BorderBrush = brush;
            tb.Height = Math.Abs(y2 - y1);
            tb.Width = Math.Abs(x2 - x1);
            tb.Margin = new Thickness(Math.Min(x1, x2), Math.Min(y1, y2), 0, 0);
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Background = Color_Transparent;
            tb.Foreground = brush;
            if (text != "")
            {
                tb.Height = double.NaN;
                tb.Text = text;
                tb.BorderBrush = Color_Transparent;
            }
            else
            {
                tb.GotFocus += new RoutedEventHandler(TextBox_GotFocus);
            }
            tempTB = tb;
            this.myCanvas.Children.Add(tempTB);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            isDrawStarted = false;
            (sender as TextBox).Focus(FocusState.Programmatic);
        }

        private void SetColor(String color)
        {
            App.currentAction.Color = color;
        }

        private void SetShape(String shape)
        {
            App.currentAction.Shape = shape;
        }

        private void SetLineThickness(double x)
        {
            App.currentAction.Thickness = x;
        }

        private void SetPointSize(double x)
        {
            App.currentAction.PointSize = x;
        }

        private void PointSizePlusButton_Click(object sender, RoutedEventArgs e)
        {
            tempContent = PointSizeTextBox.Text;
            App.currentAction.PointSize = double.Parse(PointSizeTextBox.Text);
            if (App.currentAction.PointSize < 20)
            {
                App.currentAction.PointSize += 0.1;
                PointSizeTextBox.Text = App.currentAction.PointSize.ToString();
            }
        }

        private void PointSizeMinusButton_Click(object sender, RoutedEventArgs e)
        {
            tempContent = PointSizeTextBox.Text;
            App.currentAction.PointSize = double.Parse(PointSizeTextBox.Text);
            if (App.currentAction.PointSize > 1)
            {
                App.currentAction.PointSize -= 0.1;
                PointSizeTextBox.Text = App.currentAction.PointSize.ToString();
            }
        }

        private void PointSizeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tempContent = PointSizeTextBox.Text;
        }

        private void PointSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double tempResult;
            if (double.TryParse(PointSizeTextBox.Text, out tempResult) & tempResult > 0 & tempResult <= 20)
            {
                App.currentAction.PointSize = tempResult;
                tempContent = App.currentAction.PointSize.ToString();
            }
            else
            {
                App.ShowMessage("Invalid value. Please try again.", "Error");
                PointSizeTextBox.Text = tempContent;
            }
        }

        private void ThicknessPlusButton_Click(object sender, RoutedEventArgs e)
        {
            tempContent = ThicknessTextBox.Text;
            App.currentAction.Thickness = double.Parse(ThicknessTextBox.Text);
            if (App.currentAction.Thickness < 20)
            {
                App.currentAction.Thickness += 0.1;
                ThicknessTextBox.Text = App.currentAction.Thickness.ToString();
            }
        }

        private void ThicknessMinusButton_Click(object sender, RoutedEventArgs e)
        {
            tempContent = ThicknessTextBox.Text;
            App.currentAction.Thickness = double.Parse(ThicknessTextBox.Text);
            if (App.currentAction.Thickness > 1)
            {
                App.currentAction.Thickness -= 0.1;
                ThicknessTextBox.Text = App.currentAction.Thickness.ToString();
            }
        }

        private void ThicknessTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tempContent = ThicknessTextBox.Text;
        }

        private void ThicknessTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double tempResult;
            if (double.TryParse(ThicknessTextBox.Text, out tempResult) & tempResult > 0 & tempResult <= 20)
            {
                App.currentAction.Thickness = tempResult;
                tempContent = App.currentAction.Thickness.ToString();
            }
            else
            {
                App.ShowMessage("Invalid value. Please try again.", "Error");
                ThicknessTextBox.Text = tempContent;
            }
        }

        public double GetShort(double a)
        {
            int temp = Convert.ToInt32(a);
            a = Convert.ToDouble(temp);
            return a;
        }

        private void AddToCanvas(DrawAction action, int type)
        {
            //type=1为本地绘图，type=0为转发数据

            #region Get Color
            Brush tempBrush = new SolidColorBrush(Colors.White);
            switch (action.Color)
            {
                case "Red":
                    {
                        tempBrush = new SolidColorBrush(Colors.Red);
                        break;
                    }
                case "Yellow":
                    {
                        tempBrush = new SolidColorBrush(Colors.Yellow);
                        break;
                    }
                case "Green":
                    {
                        tempBrush = new SolidColorBrush(Colors.Green);
                        break;
                    }
                case "Blue":
                    {
                        tempBrush = new SolidColorBrush(Colors.Blue);
                        break;
                    }
                case "Black":
                    {
                        tempBrush = new SolidColorBrush(Colors.Black);
                        break;
                    }
                case "White":
                    {
                        tempBrush = new SolidColorBrush(Colors.White);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            #endregion

            switch (action.Shape)
            {
                case "Point":
                    {
                        int temp = action.Coordinates.Count;
                        if (type == 1)
                        {
                            if (temp >= 2)
                            {
                                Draw_Line(action.Coordinates[temp - 2].X, action.Coordinates[temp - 1].X, action.Coordinates[temp - 2].Y, action.Coordinates[temp - 1].Y, tempBrush, action.PointSize);
                            }
                            Draw_Point(action.Coordinates[temp - 1].X, action.Coordinates[temp - 1].Y, tempBrush, action.PointSize);
                        }
                        else
                        {
                            for (int i = 0; i < temp - 1; i++)
                            {
                                Draw_Point(action.Coordinates[i].X, action.Coordinates[i].Y, tempBrush, action.PointSize);
                                Draw_Line(action.Coordinates[i].X, action.Coordinates[i + 1].X, action.Coordinates[i].Y, action.Coordinates[i + 1].Y, tempBrush, action.PointSize);
                            }
                            Draw_Point(action.Coordinates[temp - 1].X, action.Coordinates[temp - 1].Y, tempBrush, action.PointSize);
                        }
                        break;
                    }
                case "Eraser":
                    {
                        int temp = action.Coordinates.Count;
                        if (type == 1)
                        {
                            if (temp > 2)
                            {
                                Draw_Line(action.Coordinates[temp - 2].X, action.Coordinates[temp - 1].X, action.Coordinates[temp - 2].Y, action.Coordinates[temp - 1].Y, tempBrush, action.PointSize);
                            }
                            Draw_Point(action.Coordinates[temp - 1].X, action.Coordinates[temp - 1].Y, tempBrush, action.PointSize);
                        }
                        else
                        {
                            for (int i = 0; i < temp - 1; i++)
                            {
                                Draw_Point(action.Coordinates[i].X, action.Coordinates[i].Y, tempBrush, action.PointSize);
                                Draw_Line(action.Coordinates[i].X, action.Coordinates[i + 1].X, action.Coordinates[i].Y, action.Coordinates[i + 1].Y, tempBrush, action.PointSize);
                            }
                            Draw_Point(action.Coordinates[temp - 1].X, action.Coordinates[temp - 1].Y, tempBrush, action.PointSize);
                        }
                        break;
                    }
                case "Line":
                    {
                        Draw_Line(action.Coordinates[0].X, action.Coordinates[1].X, action.Coordinates[0].Y, action.Coordinates[1].Y, tempBrush, action.Thickness);
                        break;
                    }
                case "Rectangle":
                    {
                        Draw_Rectangle(action.Coordinates[0].X, action.Coordinates[1].X, action.Coordinates[0].Y, action.Coordinates[1].Y, tempBrush, action.Thickness);
                        break;
                    }
                case "TextBox":
                    {
                        Draw_TextBox(action.Coordinates[0].X, action.Coordinates[1].X, action.Coordinates[0].Y, action.Coordinates[1].Y, tempBrush, action.Text);
                        break;
                    }
                default:
                    {
                        break;
                    }
            } 
        }
        #endregion
    }
}
