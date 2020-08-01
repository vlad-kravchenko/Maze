using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Maze
{
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        Image UserImage;
        Grid map;
        int cols = 15, rows = 15;
        CellType[,] abstractMap;
        string reportString;
        int step = 0;
        List<Direction> solution;
        Queue<Key> stepByStep;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            CreateGrid();
            FillGrid();
        }

        private void CreateGrid()
        {
            map = new Grid();
            MainGrid.Children.Add(map);
            Grid.SetRow(map, 2);
            for(int i = 0; i < cols; i++)
            {
                map.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for(int i = 0; i < rows; i++)
            {
                map.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void FillGrid()
        {
            step = 0;
            MazeFactory factory = new MazeFactory();
            abstractMap = factory.GenerateMap(cols, rows);
            reportString = $"Maze {cols}*{rows}. Can be solved in {factory.Path} steps";
            Report.Text = reportString;
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var image = new Image
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Stretch = System.Windows.Media.Stretch.Fill
                    };
                    switch (abstractMap[x, y])
                    {
                        case CellType.USER:
                            Panel.SetZIndex(image, 10);
                            image.Source = Resources["User"] as BitmapImage;
                            UserImage = image;
                            break;
                        case CellType.EXIT:
                            Panel.SetZIndex(image, 10);
                            image.Source = Resources["Exit"] as BitmapImage;
                            break;
                        case CellType.WALL:
                            image.Source = Resources["Wall"] as BitmapImage;
                            break;
                        default:
                            image.Source = Resources["Grass"] as BitmapImage;
                            break;
                    }
                    map.Children.Add(image);
                    Grid.SetRow(image, x);
                    Grid.SetColumn(image, y);
                }
            }
            var firstGrass = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Stretch = System.Windows.Media.Stretch.Fill,
                Source = Resources["Grass"] as BitmapImage
            };
            map.Children.Add(firstGrass);
            Grid.SetRow(firstGrass, 0);
            Grid.SetColumn(firstGrass, 0);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            MoveUser(e.Key);
        }

        private void MoveUser(Key key)
        {
            int row = Grid.GetRow(UserImage);
            int col = Grid.GetColumn(UserImage);

            var grid = map;

            switch (key)
            {
                case Key.Up:
                    if (row > 0 && CanMove(row - 1, col))
                    {
                        Grid.SetRow(UserImage, row - 1);
                        IncStep();
                    }
                    break;
                case Key.Down:
                    if (row < map.RowDefinitions.Count - 1 && CanMove(row + 1, col))
                    {
                        Grid.SetRow(UserImage, row + 1);
                        IncStep();
                    }
                    break;
                case Key.Left:
                    if (col > 0 && CanMove(row, col - 1))
                    {
                        Grid.SetColumn(UserImage, col - 1);
                        IncStep();
                    }
                    break;
                case Key.Right:
                    if (col < map.ColumnDefinitions.Count - 1 && CanMove(row, col + 1))
                    {
                        Grid.SetColumn(UserImage, col + 1);
                        IncStep();
                    }
                    break;
            }
            if (Finished())
                Restart_Click(null, null);
        }

        private void IncStep()
        {
            step++;
            Report.Text = reportString + ". Current step: " + step;
        }

        private bool Finished()
        {
            var children = map.Children
                          .Cast<UIElement>()
                          .Where(i => Grid.GetRow(i) == Grid.GetRow(UserImage) && Grid.GetColumn(i) == Grid.GetColumn(UserImage));
            return children.Any(s => (s as Image).Source == Resources["Exit"]);
        }

        private bool CanMove(int row, int col)
        {
            var children = map.Children
                          .Cast<UIElement>()
                          .Where(i => Grid.GetRow(i) == row && Grid.GetColumn(i) == col);
            return !children.Any(s => (s as Image).Source.ToString().Contains("wall"));
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (stepByStep.Count > 1)
                MoveUser(stepByStep.Dequeue());
            else
            {
                MoveUser(stepByStep.Dequeue());
                timer.Stop();
                Restart.IsEnabled = true;
                Help.IsEnabled = true;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            solution = new MouseSolver(abstractMap.Clone() as CellType[,])
                .SolveMaze(new Cell(Grid.GetRow(UserImage), Grid.GetColumn(UserImage)), new Cell(cols - 1, rows - 1));
            stepByStep = new Queue<Key>();
            foreach (var step in solution)
            {
                switch (step)
                {
                    case Direction.LEFT:
                        stepByStep.Enqueue(Key.Left);
                        break;
                    case Direction.RIGHT:
                        stepByStep.Enqueue(Key.Right);
                        break;
                    case Direction.TOP:
                        stepByStep.Enqueue(Key.Up);
                        break;
                    default:
                        stepByStep.Enqueue(Key.Down);
                        break;
                }
            }
            stepByStep.Reverse();

            timer.Start();
            Restart.IsEnabled = false;
            Help.IsEnabled = false;
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (sender !=null || e != null)
            {
                cols--;
                rows--;
            }
            map.Children.Clear();
            Grid.SetColumn(UserImage, 0);
            Grid.SetRow(UserImage, 0);
            cols++;
            rows++;
            CreateGrid();
            FillGrid();
            timer.Stop();
        }
    }
}