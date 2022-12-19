namespace Days {
    public class Day19 : DaySolverBase {
        public override string Example1 =>
            @"Blueprint 1:  Each ore robot costs 4 ore.  Each clay robot costs 2 ore.  Each obsidian robot costs 3 ore and 14 clay.  Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2:  Each ore robot costs 2 ore.  Each clay robot costs 3 ore.  Each obsidian robot costs 3 ore and 8 clay.  Each geode robot costs 3 ore and 12 obsidian.";

        class Blueprint {
            public int id;
            public Dictionary<string, Dictionary<string, int>> costs;
        }
        
        List<Blueprint> Transform(string raw) {
            return raw
                .Replace(":", ".")
                .Replace("and ", "")
                .Split("\n")
                .Select(l => {
                    var parts = l.Split(".");
                    
                    var part = parts
                        .TakeLast(4)
                        .Select(p => {
                        var s = part.Split(" ")
                            .ToList();
                        
                        var start = s.IndexOf("costs") + 1;
                        var robotType = s[start - 2];
                    
                        for(int i = start; i < s.Count;) {
                            var type = s[i + 1];
                            switch(type) {
                                case "ore":
                            }
                        }
                    });
                    
                    var bp = new Blueprint() {
                        id = int.Parse(parts[1]),
                    };

                    return new Blueprint();
                })
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            return -1;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}