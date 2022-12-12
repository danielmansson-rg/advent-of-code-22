namespace Days {
    public class Day12 : DaySolverBase {
        public override string Example1 =>
            @"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi";

        (List<List<int>> grid, (int x, int y) start, (int x, int y) end) Transform(string raw) {
            List<List<int>>  grid = raw
                .Replace("S", "a")
                .Replace("E", "z")
                .Split("\n")
                .Select(l => l.Select(c => c - 'a').ToList())
                .ToList();

            var start = (-1, -1);
            var end = (-1, -1);
            var split = raw.Split("\n");
            for(int i = 0; i < split.Length; i++) {
                for(int j = 0; j < split[i].Length; j++) {
                    char c = split[i][j];
                    if(c == 'S') {
                        start = (j, i);
                    }
                    else if(c == 'E') {
                        end = (j, i);
                    }
                }
            }

            return (grid, start, end);
        }

        private List<(int x, int y)> neighborLookup = new() {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1),
        };

        bool InBounds((int x, int y) pos, int w, int h) {
            return pos.x >= 0
                && pos.x < w
                && pos.y >= 0
                && pos.y < h;
        }
        
        List<(int x, int y)> GetNeighbors((int x, int y) p, int w, int h) {
            return neighborLookup
                .Select(n => (p.x + n.x, p.y + n.y))
                .Where(n => InBounds(n, w, h))
                .ToList();
        }
        
        public override object Solve1(string raw) {
            var input = Transform(raw);

            var visited = new Dictionary<(int x, int y), (int x, int y)>();
            var queue = new LinkedList<(int x, int y)>();

            queue.AddFirst(input.start);
            visited.Add(input.start, input.start);

            while(queue.Count != 0) {
                var current = queue.First();
                queue.RemoveFirst();

            }

            return -1;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}