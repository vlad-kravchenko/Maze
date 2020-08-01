using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze
{
    public class MouseSolver
    {
        private List<Move> directions = new List<Move>
        {
             new Move(-1, 0, Direction.TOP),
             new Move(1, 0, Direction.BOTTOM),
             new Move(0, 1, Direction.RIGHT),
             new Move(0, -1, Direction.LEFT)
        };

        private CellType[,] map;
        private CellType[,] top;
        private int w, h;
        List<Direction> overalPath = new List<Direction>();

        public MouseSolver(CellType[,] map)
        {
            this.map = map;
            w = map.GetLength(0);
            h = map.GetLength(1);

            top = new CellType[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (map[x, y] == CellType.USER)
                        map[x, y] = CellType.GRASS;
                    top[x, y] = CellType.GRASS;
                }
            }
        }

        public List<Direction> SolveMaze(Cell start, Cell finish)
        {
            map[start.X, start.Y] = CellType.GRASS;
            top[start.X, start.Y] = CellType.USER;

            if (start.X == finish.X && start.Y == finish.Y) return new List<Direction>();

            Queue<Player> queue = new Queue<Player>();
            List<Cell> visited = new List<Cell>();

            Player player = new Player(start.X, start.Y, "");

            Cell place = new Cell(start.X, start.Y);

            queue.Enqueue(player);

            while (queue.Count > 0)
            {
                player = queue.Dequeue();
                foreach (var side in directions)
                {
                    place.X = player.X + side.Xdir;
                    place.Y = player.Y + side.Ydir;

                    if (!InRange(place)) continue;
                    if (visited.Contains(place)) continue;

                    visited.Add(place);

                    Player step = new Player(place.X, place.Y, player.Path + side.Direction + " ");

                    if (place.Equals(finish)) return GetPath(step.Path);
                    queue.Enqueue(step);
                }
            }

            return new List<Direction>();
        }

        private List<Direction> GetPath(string path)
        {
            List<string> strings = path.Trim(' ').Split(' ').ToList();
            strings.ForEach(s => overalPath.Add(ParseEnum<Direction>(s)));
            return overalPath;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private bool InRange(Cell place)
        {
            if (place.X < 0 || place.X > w - 1) return false;
            if (place.Y < 0 || place.Y > h - 1) return false;
            if (map[place.X, place.Y] == CellType.GRASS && top[place.X, place.Y] == CellType.GRASS) return true;
            if (map[place.X, place.Y] == CellType.EXIT && top[place.X, place.Y] == CellType.GRASS) return true;
            return false;
        }
    }
}