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

        public List<(int x, int y)> BFS((List<List<int>> grid, (int x, int y) start, (int x, int y) end) input, List<(int x, int y)> startPositions) {
            var queue = new LinkedList<(int x, int y)>();
            var visited = new Dictionary<(int x, int y), (int x, int y)>();
            int h = input.grid.Count;
            int w = input.grid[0].Count;

            foreach(var start in startPositions) {
                queue.AddLast(start);
                visited.Add(start, start);
            }

            while(queue.Count != 0) {
                var p = queue.First();
                queue.RemoveFirst();

                foreach(var n in GetNeighbors(p, w, h)) {
                    if(!visited.ContainsKey(n) && input.grid[n.y][n.x] <= input.grid[p.y][p.x] + 1) {
                        queue.AddLast(n);
                        visited.Add(n, p);

                        if(n == input.end) {
                            queue.Clear();
                            break;
                        }
                    }
                }
            }

            var result = new List<(int x, int y)>();
            var current = input.end;
            result.Add(current);
            while(current != visited[current]) {
                current = visited[current];
                result.Add(current);
            }
            result.Reverse();
            
            return result;
        }
        
        public override object Solve1(string raw) {
            var input = Transform(raw);

            var result = BFS(input, new() {
                input.start
            });

            return result.Count - 1;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);
            var start = new List<(int x, int y)>();
            for(int i = 0; i < input.grid.Count; i++) {
                for(int j = 0; j < input.grid[i].Count; j++) {
                    if(input.grid[i][j] == 0) {
                        start.Add((j, i));
                    }
                }
            }

            var result = BFS(input, start);
            
            return result.Count -1;
        }
    }
}