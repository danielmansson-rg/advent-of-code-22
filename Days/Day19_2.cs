namespace Days {
    public class Day19 : DaySolverBase {
        public override string Example1 =>
            @"Blueprint 1:  Each ore robot costs 4 ore.  Each clay robot costs 2 ore.  Each obsidian robot costs 3 ore and 14 clay.  Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2:  Each ore robot costs 2 ore.  Each clay robot costs 3 ore.  Each obsidian robot costs 3 ore and 8 clay.  Each geode robot costs 3 ore and 12 obsidian.";

        class Blueprint {
            public int id;
            public List<List<int>> costs;
        }

        List<Blueprint> Transform(string raw) {
            return raw
                .Replace(":", ".")
                .Replace("and ", "")
                .Split("\n")
                .Select(l => {
                    var parts = l.Split(".");

                    var allCosts = parts
                        .SkipLast(1)
                        .TakeLast(4)
                        .Select(p => {
                            var s = p.Split(" ")
                                .ToList();
                            
                            var start = s.IndexOf("costs") + 1;
                            var robotType = s[start - 3];
                            var costs = new Dictionary<string, int>();
                            
                            for(int i = start; i < s.Count; i += 2) {
                                var type = s[i + 1];
                                costs.Add(type, int.Parse(s[i]));
                            }

                            List<int> c = new List<string>() {
                                "ore",
                                "clay",
                                "obsidian",
                                "geode"
                            }.Select(t => {
                                costs.TryGetValue(t, out var val);
                                return val;
                            })
                                .ToList();
                            
                            return c;
                        });
                    
                    var bp = new Blueprint() {
                        id = int.Parse(parts[0].Split(" ")[1]),
                        costs = allCosts.ToList()
                    };

                    return bp;
                })
                .ToList();
        }

        struct HashState {
            public (int ore, int clay, int obsidan, int geode) resources;
            public (int ore, int clay, int obsidan, int geode) robots;
            public int minute;

            public State ToState() {
                return new State() {
                    resources = new() {
                        resources.ore,
                        resources.clay,
                        resources.obsidan,
                        resources.geode,
                    },
                    robots = new() {
                        robots.ore,
                        robots.clay,
                        robots.obsidan,
                        robots.geode,
                    },
                    minute = minute
                };
            }
        }

        class State {
            public List<int> resources = new();
            public List<int> robots = new();
            public int minute;

            public HashState ToHash() {
                return new HashState() {
                    resources = (resources[0],resources[1],resources[2],resources[3]),
                    robots = (robots[0],robots[1],robots[2],robots[3]),
                    minute = minute
                };
            }
        }
        
        public override object Solve1(string raw) {
            var blueprints = Transform(raw);

            
            List<int> qualityLevels = new();
            
            foreach(Blueprint blueprint in blueprints) {
                HashState s = new () {
                    robots = (1,0,0,0)
                };
                var queue = new LinkedList<HashState>();
                    queue.AddFirst(s);

                HashState bestState = new();

                int maxMinutes = 24;
                int it = 0;
                while(queue.Count > 0) {
                    var current = queue.First();
                    queue.RemoveFirst();
                    var state = current.ToState();

                    for(int buildTarget = 3; buildTarget >= 0; buildTarget++) {
                        int waitTurns = MinutesToAfford(state, blueprint.costs, buildTarget);
                        if(state.minute + waitTurns < maxMinutes) {
                            //Wait
                            //Buy
                            //Collect
                            //Create
                        }
                    }
                }
            }

            return qualityLevels.Count;
        }
        private int MinutesToAfford(State state, List<List<int>> blueprintCosts, int buildTarget) {
            int max = 0;
            var cost = blueprintCosts[buildTarget];

            for(int i = 0; i < 4; i++) {
                var needed = cost[i];
                var available = state.resources[i];
                var rate = state.robots[i];
                var left = needed - available;
                if(left > 0 && rate == 0) {
                    return int.MaxValue;
                }
                else {
                    var turns = left / rate + left % rate != 0 ? 1 : 0;
                    if(turns > max) {
                        max = turns;
                    }
                }
            }
            
            return max;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}