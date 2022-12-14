namespace Days {
    public class Day14 : DaySolverBase {
        public override string Example1 =>
            @"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9";

        List<List<(int x, int y)>> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => l
                    .Split(" -> ")
                    .Select(i => {
                        var s = i.Split(",");
                        return (int.Parse(s[0]), int.Parse(s[1]));
                    })
                    .ToList())
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);
            Dictionary<(int x, int y), char> map = CreateMap(input);
            (int x, int y) s = (500, 0);
            int limitY = map.Max(i => i.Key.y);
            int count = 0;

            while(true) {
                var p = s;
                while(p.y < limitY) {
                    if(!map.ContainsKey((p.x, p.y + 1))) {
                        p = (p.x, p.y + 1);
                    }
                    else if(!map.ContainsKey((p.x - 1, p.y + 1))) {
                        p = (p.x - 1, p.y + 1);
                    }
                    else if(!map.ContainsKey((p.x + 1, p.y + 1))) {
                        p = (p.x + 1, p.y + 1);
                    }
                    else {
                        break;
                    }
                }

                if(p.y >= limitY) {
                    break;
                }

                map[p] = 'o';
                count++;
            }
            
          //  Print(map);
            
            return count;
        }

        void Print(Dictionary<(int x, int y), char> map) {
            (int x, int y) min = (map.Keys.Min(k => k.x), map.Keys.Min(k => k.y));
            (int x, int y) max = (map.Keys.Max(k => k.x), map.Keys.Max(k => k.y));

            var fg = Console.ForegroundColor;
            
            for(int i = min.y; i <= max.y; i++) {
                for(int j = min.x; j <= max.x; j++) {
                    if(!map.TryGetValue((j, i), out char c)) {
                        c = '.';
                    }

                    switch(c) {
                        case '.':
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;
                        case 'o':
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        default:
                            Console.ForegroundColor = fg;
                            break;
                    }
                    
                    Console.Write(c);
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = fg;
        }
        
        private static Dictionary<(int x, int y), char> CreateMap(List<List<(int x, int y)>> input) {
            Dictionary<(int x, int y), char> map = new();

            foreach(List<(int x, int y)> lineSequence in input) {
                for(int i = 0; i < lineSequence.Count - 1; i++) {
                    var from = lineSequence[i];
                    var to = lineSequence[i + 1];

                    (int x, int y) d = (Math.Sign(to.x - from.x), Math.Sign(to.y - from.y));
                    do {
                        map[from] = '#';
                        from = (from.x + d.x, from.y + d.y);
                    } while(from != to);
                    map[from] = '#';

                }
            }

            return map;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);
            Dictionary<(int x, int y), char> map = CreateMap(input);
            (int x, int y) s = (500, 0);
            int limitY = map.Max(i => i.Key.y) + 1;
            int count = 0;

            while(true) {
                var p = s;
                while(p.y < limitY) {
                    if(!map.ContainsKey((p.x, p.y + 1))) {
                        p = (p.x, p.y + 1);
                    }
                    else if(!map.ContainsKey((p.x - 1, p.y + 1))) {
                        p = (p.x - 1, p.y + 1);
                    }
                    else if(!map.ContainsKey((p.x + 1, p.y + 1))) {
                        p = (p.x + 1, p.y + 1);
                    }
                    else {
                        break;
                    }
                }

                map[p] = 'o';
                count++;
                
                if(p == (500, 0)) {
                    break;
                }
            }
            
         //   Print(map);
            
            return count;
        }
    }
}