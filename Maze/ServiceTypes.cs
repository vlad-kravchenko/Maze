namespace Maze
{
    public enum Direction { LEFT, RIGHT, TOP, BOTTOM }
    public enum CellType { USER, WALL, GRASS, EXIT }
    public struct Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public struct Move
    {
        public int Xdir { get; set; }
        public int Ydir { get; set; }
        public Direction Direction { get; set; }

        public Move(int x, int y, Direction dir)
        {
            Xdir = x;
            Ydir = y;
            Direction = dir;
        }
    }
    public struct Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Path { get; set; }

        public Player(int x, int y, string path)
        {
            X = x;
            Y = y;
            Path = path;
        }
    }
}