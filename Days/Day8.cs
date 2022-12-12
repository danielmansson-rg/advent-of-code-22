using System.ComponentModel;
using System.Drawing;

namespace Days {
    public class Day8 : DaySolverBase {
        public override string Example1 =>
            @"30373
25512
65332
33549
35390";

        List<List<int>> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => l
                    .ToArray()
                    .Select(c => (int)(c - '0'))
                    .ToList())
                .ToList();
        }

        List<(int x, int y)> neighbors = new() {
                (-1, 0),
                (1, 0),
                (0, 1),
                (0, -1),
            };

        List<(int x, int y)> GetNeighbors((int x, int y) pos, int size) {
            return neighbors
                .Select(n => (pos.x + n.x, pos.y + n.y))
                .Where(n => InBounds(n, size))
                .ToList();
        }

        private bool InBounds((int x, int y) pos, int size) {
            return pos.x >= 0
                && pos.y >= 0
                && pos.x < size
                && pos.y < size;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            int size = input.Count;




            HashSet<(int x, int y)> visible = new();

            Check(input, visible, (1, 0), i => (0, i), size);
            Check(input, visible, (-1, 0), i => (size - 1, i), size);
            Check(input, visible, (0, 1), i => (i, 0), size);
            Check(input, visible, (0, -1), i => (i, size - 1), size);

            //Draw(input, visible, null);

            return visible.Count;
        }

        void Check(List<List<int>> input, HashSet<(int, int)> visible, (int x, int y) dir, Func<int, (int x, int y)> f, int size) {
            LinkedList<(int x, int y)> open = new();
            for(int i = 0; i < size; i++) {
                open.AddLast(f(i));
            }

            while(open.Count > 0) {
                var pos = open.First();
                open.RemoveFirst();

                int maxHeight = -1;
                for(int i = 0; i < size; i++) {
                    var height = input[pos.y][pos.x];
                    if(height > maxHeight) {
                        maxHeight = height;
                        visible.Add(pos);
                    }

                    pos = (pos.x + dir.x, pos.y + dir.y);
                }
            }
        }

        void Draw(List<List<int>> input, HashSet<(int x, int y)> visible, (int x, int y)? cur) {
            int size = input.Count;
            for(int y = 0; y < size; y++) {
                for(int x = 0; x < size; x++) {
                    if(cur == (x, y)) {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else if(visible.Contains((x, y))) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    Console.Write(input[y][x]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

        }

        public override object Solve2(string raw) {
            var input = Transform(raw);
            int size = input.Count;

            List<int> scores = new List<int>();
            for(int y = 0; y < size; y++) {
                for(int x = 0; x < size; x++) {
                    scores.Add(ScenicScore(input, (x, y), size));
                }
            }

            return scores.Max();
        }

        public int ScenicScore(List<List<int>> input, (int x, int y) p, int size) {
            int score = 1;

            score *= CheckFromPos(input, (0, -1), p, size);
            score *= CheckFromPos(input, (-1, 0), p, size);
            score *= CheckFromPos(input, (0, 1), p, size);
            score *= CheckFromPos(input, (1, 0), p, size);

            return score;
        }

        int CheckFromPos(List<List<int>> input, (int x, int y) dir, (int x, int y) start, int size) {
            int count = 0;

            (int x, int y) pos = start;
            int startHeight = input[pos.y][pos.x];
            pos = (pos.x + dir.x, pos.y + dir.y);
            if(!InBounds(pos, size))
                return count;

            for(int i = 0; i < size; i++) {
                var height = input[pos.y][pos.x];

                count++;

                if(height >= startHeight)
                    return count;

                pos = (pos.x + dir.x, pos.y + dir.y);
                if(!InBounds(pos, size))
                    return count;
            }

            return count;
        }
    }
}