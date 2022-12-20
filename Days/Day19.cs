namespace Days {
    public class Dasday19 : DaySolverBase {
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

        struct State {
            public (int ore, int clay, int obsidan, int geode) resources;
            public (int ore, int clay, int obsidan, int geode) robots;
            public int minute;
        }
        
        public override object Solve1(string raw) {
            var blueprints = Transform(raw);

            
            List<int> qualityLevels = new();
            
            foreach(Blueprint blueprint in blueprints) {
                var costs = new List<(int ore, int clay, int obsidan, int geode)>();
                foreach(var r in blueprint.costs) {
                    costs.Add((r[0],r[1],r[2],r[3]));
                }
                State s = new () {
                    robots = (1,0,0,0)
                };
                var queue = new LinkedList<State>();
                    queue.AddFirst(s);
                HashSet<State> visited = new();
                visited.Add(s);

                State bestState = new();

                int maxMinutes = 24;
                int it = 0;
                while(queue.Count > 0) {
                    var cur = queue.First();
                    queue.RemoveFirst();
                    it++;
                    if(it % 100000000 == 0) {
                        Console.WriteLine($"{queue.Count}");
                    }
                    if(cur.minute > maxMinutes) {
                        continue;
                    }

                    if(cur.resources.geode > bestState.resources.geode) {
                        Console.WriteLine("Best: " + bestState.resources.geode);
                        bestState = cur;
                    }
                    
                    var remaining = maxMinutes - cur.minute;
                    var potential = (cur.robots.geode + remaining) * remaining + cur.resources.geode;
                    if(potential <= bestState.resources.geode) {
                        continue;
                    }                    
                    (int ore, int clay, int obsidan, int geode) rem;
                    (int ore, int clay, int obsidan, int geode) cost;
                    
                    //Buy Ge
                    cost = costs[3];
                    rem = cur.resources;
                    rem = SubtractCost(rem, cost);
                    if(AllPositive(rem)) {
                        State n = cur;
                        //Buy
                        n.resources = rem;
                        //Collect
                        n = AddResourcesFromMining(n);
                        //Build
                        n.robots.geode++;
                        n.minute++;
                        queue.AddFirst(n);
                    }
                    //Buy Obs
                    cost = costs[2];
                    rem = cur.resources;
                    rem = SubtractCost(rem, cost);
                    if(AllPositive(rem)) {
                        State n = cur;
                        //Buy
                        n.resources = rem;
                        //Collect
                        n = AddResourcesFromMining(n);
                        //Build
                        n.robots.obsidan++;
                        n.minute++;
                        queue.AddFirst(n);
                    }
                    //Buy Clay
                    cost = costs[1];
                    rem = cur.resources;
                    rem = SubtractCost(rem, cost);
                    if(AllPositive(rem)) {
                        State n = cur;
                        //Buy
                        n.resources = rem;
                        //Collect
                        n = AddResourcesFromMining(n);
                        //Build
                        n.robots.clay++;
                        n.minute++;
                        queue.AddFirst(n);
                    }
                    //Buy Ore
                    cost = costs[0];
                    rem = cur.resources;
                    rem = SubtractCost(rem, cost);
                    if(AllPositive(rem)) {
                        State n = cur;
                        //Buy
                        n.resources = rem;
                        //Collect
                        n = AddResourcesFromMining(n);
                        //Build
                        n.robots.ore++;
                        n.minute++;
                        queue.AddFirst(n);
                    }
                    //Do nothing
                    {
                        State n = cur;
                        //Collect
                        n = AddResourcesFromMining(n);
                        n.minute++;
                        queue.AddFirst(n);
                    }
                }
            }

            return qualityLevels.Count;
        }
        private static bool AllPositive((int ore, int clay, int obsidan, int geode) rem) {
            return rem.ore >= 0
                && rem.clay >= 0
                && rem.obsidan >= 0
                && rem.geode >= 0;
        }
        private static (int ore, int clay, int obsidan, int geode) SubtractCost((int ore, int clay, int obsidan, int geode) rem,
            (int ore, int clay, int obsidan, int geode) cost) {
            rem.ore -= cost.ore;
            rem.clay -= cost.clay;
            rem.obsidan -= cost.obsidan;
            rem.geode -= cost.geode;
            return rem;
        }
        private static State AddResourcesFromMining(State n) {
            n.resources.ore += n.robots.ore;
            n.resources.clay += n.robots.clay;
            n.resources.obsidan += n.robots.obsidan;
            n.resources.geode += n.robots.geode;
            return n;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}