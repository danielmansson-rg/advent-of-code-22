namespace Days {
    public class Day9 : DaySolverBase {
        public override string Example1 =>
            @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2";

        List<(string, int)> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => {
                    var s = l.Split(" ");
                    return (s[0], int.Parse(s[1]));
                })
                .ToList();
        }

        Dictionary<string, (int x, int y)> dirLookup = new() {
            { "D", (0, 1) },
            { "U", (0, -1) },
            { "L", (-1, 0) },
            { "R", (1, 0) }
        };

        int Dist((int x, int y) a, (int x, int y) b) {
            var dx = Math.Abs(a.x - b.x);
            var dy = Math.Abs(a.y - b.y);
            return Math.Max(dx, dy);
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            (int x, int y) headPos = (0, 0);
            (int x, int y) tailPos = (0, 0);
            HashSet<(int, int)> visited = new();
            visited.Add(tailPos);

            foreach(var cmd in input) {
                var dir = dirLookup[cmd.Item1];
                for(int i = 0; i < cmd.Item2; i++) {
                    var prev = headPos;
                    headPos.x += dir.x;
                    headPos.y += dir.y;

                    if(Dist(headPos, tailPos) > 1) {
                        tailPos = prev;
                        visited.Add(tailPos);
                    }
                }
            }

            return visited.Count;
        }

        public override string Example2 => @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20";

        public override object Solve2(string raw) {
            var input = Transform(raw);

            List<(int x, int y)> segments = new();
            for(int i = 0; i < 10; i++) {
                segments.Add((0,0));
            }
            HashSet<(int, int)> visited = new();
            visited.Add(segments.Last());

            foreach(var cmd in input) {
                var dir = dirLookup[cmd.Item1];
                for(int i = 0; i < cmd.Item2; i++) {
                    var p = segments[0];
                    segments[0] = (p.x + dir.x, p.y + dir.y);

                    for(int s = 1; s < segments.Count; s++) {
                        var a = segments[s - 1];
                        var b = segments[s];

                        if(Dist(a, b) > 1) {
                            (int x, int y) d = (a.x - b.x, a.y - b.y);
                            d.x = Math.Sign(d.x);
                            d.y = Math.Sign(d.y);

                            segments[s] = (b.x + d.x, b.y + d.y);
                        }
                    }

                    visited.Add(segments.Last());
                }
            }

            //Draw(segments, visited);

            return visited.Count;
        }

        void Draw(List<(int x, int y)> segments, HashSet<(int, int)> visited) {
            var size = 20;
            var offset = -10;

            for(int i = 0; i < size; i++) {
                for(int j = 0; j < size; j++) {
                    var p = (j + offset, i + offset);

                    Console.BackgroundColor = visited.Contains(p) ? ConsoleColor.DarkCyan : ConsoleColor.Black;
                    int idx = segments.IndexOf(p);
                    if(idx == -1) {
                        Console.Write(p == (0, 0) ? "s" : ".");
                    }
                    else {
                        Console.Write(idx);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}