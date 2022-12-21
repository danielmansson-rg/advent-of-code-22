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
                    resources = (resources[0], resources[1], resources[2], resources[3]),
                    robots = (robots[0], robots[1], robots[2], robots[3]),
                    minute = minute
                };
            }
        }

        public override object Solve1(string raw) {
            var blueprints = Transform(raw);


            List<int> qualityLevels = new();

            foreach(Blueprint blueprint in blueprints) {
                HashState s = new() {
                    robots = (1, 0, 0, 0)
                };
                var queue = new LinkedList<HashState>();
                queue.AddFirst(s);

                HashState bestState = new();

                int maxMinutes = 24;
                int it = 0;
                while(queue.Count > 0) {
                    var current = queue.First();
                    queue.RemoveFirst();

                    if(current.minute > maxMinutes) {
                        continue;
                    }
                    
                    {
                        var remaining = maxMinutes - current.minute;
                        var potential = (current.robots.geode + remaining) * remaining + current.resources.geode;
                        if(potential <= bestState.resources.geode) {
                            continue;
                        }
                    }

                    if(current.resources.geode > bestState.resources.geode) {
                        bestState = current;
                    }

                    var state = current.ToState();

                    {
                        var waitTurns = maxMinutes - current.minute;
                        var next = state.ToHash().ToState();
                        //Wait
                        next.minute += waitTurns;
                        for(int j = 0; j < 4; j++) {
                            next.resources[j] += next.robots[j] * waitTurns;
                        }

                        var remaining = maxMinutes - next.minute;
                        var potential = (next.robots[3] + remaining) * remaining + next.resources[3];
                        if(potential > bestState.resources.geode) {
                            queue.AddFirst(next.ToHash());
                        }
                    }
                    for(int buildTarget = 0; buildTarget < 4; buildTarget++) {
                        int waitTurns = MinutesToAfford(state, blueprint.costs, buildTarget);
                        if(state.minute + waitTurns < maxMinutes) {
                            var next = state.ToHash().ToState();
                            //Wait
                            next.minute += waitTurns;
                            for(int j = 0; j < 4; j++) {
                                next.resources[j] += next.robots[j] * waitTurns;
                            }

                            next.minute += 1;
                            //Buy
                            for(int j = 0; j < 4; j++) {
                                next.resources[j] -= blueprint.costs[buildTarget][j];
                            }
                            //Collect
                            for(int j = 0; j < 4; j++) {
                                next.resources[j] += next.robots[j];
                            }
                            //Create
                            next.robots[buildTarget]++;
                            
                            var remaining = maxMinutes - next.minute;
                            var potential = (next.robots[3] + remaining) * remaining + next.resources[3];
                            if(potential > bestState.resources.geode) {
                                queue.AddFirst(next.ToHash());
                            }
                        }
                    }

                }
                
                qualityLevels.Add(bestState.resources.geode * blueprint.id);
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
                    return 10000000;
                }
                else if(left != 0) {
                    var turns = left / rate + (left % rate != 0 ? 1 : 0);
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