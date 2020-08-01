using System;
using System.Collections.Generic;

namespace Maze
{
    public class MazeFactory
    {
        private CellType[,] map;
        CellType[,] backupMap;
        private int[,] steps;
        private int width, height;
        private Random rand = new Random();
        private List<Move> directions = new List<Move>
        {
             new Move(-1, 0, Direction.LEFT),
             new Move(1, 0, Direction.RIGHT),
             new Move(0, 1, Direction.BOTTOM),
             new Move(0, -1, Direction.TOP)
        };
        public int Path { get; set; }

        public CellType[,] GenerateMap(int w, int h)
        {
            height = h;
            width = w;
            map = new CellType[width, height];
            steps = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = rand.Next(1, 3) == 1 ? CellType.WALL : CellType.GRASS;
                }
            }
            map[0, 0] = CellType.USER;
            map[width - 1, height - 1] = CellType.EXIT;

            backupMap = map.Clone() as CellType[,];

            if (Solvable())
            {
                map = null;
                return backupMap;
            }
            else
            {
                map = null;
                backupMap = null;
                return GenerateMap(width, height);
            }
        }

        private bool Solvable()
        {
            Queue<Cell> queue = new Queue<Cell>();
            map[0, 0] = CellType.WALL;
            queue.Enqueue(new Cell(0, 0));

            while (queue.Count > 0)
            {
                Cell cell = queue.Dequeue();

                if (FoundExit(cell)) return true;

                foreach (var dir in directions)
                {
                    if (CanMove(dir.Direction, cell.X, cell.Y, CellType.GRASS))
                    {
                        steps[cell.X + dir.Xdir, cell.Y + dir.Ydir] = steps[cell.X, cell.Y] + 1;
                        queue.Enqueue(new Cell(cell.X + dir.Xdir, cell.Y + dir.Ydir));
                        map[cell.X + dir.Xdir, cell.Y + dir.Ydir] = CellType.WALL;
                    }
                }
            }

            return false;
        }

        private bool FoundExit(Cell cell)
        {
            if (CanMove(Direction.LEFT, cell.X, cell.Y, CellType.EXIT) || 
                CanMove(Direction.RIGHT, cell.X, cell.Y, CellType.EXIT) || 
                CanMove(Direction.TOP, cell.X, cell.Y, CellType.EXIT) || 
                CanMove(Direction.BOTTOM, cell.X, cell.Y, CellType.EXIT))
            {
                Path = steps[cell.X, cell.Y] + 1;
                return true;
            }
            return false;
        }

        private bool CanMove(Direction direction, int x, int y, CellType cellType)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    if (x > 0)  return map[x - 1, y] == cellType;
                    break;
                case Direction.RIGHT:
                    if (x < width - 1) return map[x + 1, y] == cellType;
                    break;
                case Direction.TOP:
                    if (y > 0) return map[x, y - 1] == cellType;
                    break;
                case Direction.BOTTOM:
                    if (y < height - 1) return map[x, y + 1] == cellType;
                    break;
            }
            return false;
        }
    }
}