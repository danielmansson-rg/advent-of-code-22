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
                            return (robotType, costs);
                        });
                    
                    var bp = new Blueprint() {
                        id = int.Parse(parts[0].Split(" ")[1]),
                        costs = allCosts.ToDictionary(v => v.robotType, v => v.costs)
                    };

                    return bp;
                })
                .ToList();
        }

        public override object Solve1(string raw) {
            var blueprints = Transform(raw);

            List<int> qualityLevels = new();
            
            foreach(Blueprint blueprint in blueprints) {
                Dictionary<string, int> res = new() {
                    {"ore", 0},
                    {"clay", 0},
                    {"obsidian", 0},
                    {"geode", 0},
                };
                Dictionary<string, int> robots = new() {
                    {"ore", 1},
                    {"clay", 0},
                    {"obsidian", 0},
                    {"geode", 0},
                };
                
                for(int i = 0; i < 24; i++) {
                    //start build
                    var robotChoice = blueprint.costs
                        .FirstOrDefault(c => c.Value
                            .All(v => res[v.Key] >= v.Value));
                    if(robotChoice.Key != null) {
                        foreach(KeyValuePair<string,int> cost in robotChoice.Value) {
                            res[cost.Key] -= cost.Value;
                        }
                    }

                    //get resources
                    foreach(KeyValuePair<string,int> robot in robots) {
                        res[robot.Key] += robot.Value;
                    }

                    //build done
                    if(robotChoice.Key != null) {
                        robots[robotChoice.Key] += 1;
                    }
                }

                int obsidian = res["obsidian"];
                int qualityLevel = obsidian * blueprint.id;

                qualityLevels.Add(qualityLevel);
            }

            return qualityLevels.Count;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}