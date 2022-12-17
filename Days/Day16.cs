namespace Days {
    public class Day16 : DaySolverBase {

        class Node {
            public string id;
            public int flow;
            public List<string> rawConnections = new();
            public List<Node> connections = new();
            public int idx;
        }

        public override string Example1 =>
            @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II";

        List<Node> Transform(string raw) {
            var result = raw
                .Replace(";", "")
                .Replace("rate=", "")
                .Replace(",", "")
                .Split("\n")
                .Select((l, idx) => {
                    var s = l.Split(" ");
                    return new Node() {
                        id = s[1],
                        flow = int.Parse(s[4]),
                        rawConnections = s.Skip(9).ToList(),
                        idx = idx
                    };
                })
                .ToList();

            foreach(var node in result) {
                node.connections = node.rawConnections
                    .Select(c => result.First(n => n.id == c))
                    .ToList();
            }

            return result;
        }

        void FillCost(Node source, Dictionary<(int, int), int> cost) {
            HashSet<Node> visited = new();
            LinkedList<(Node n, int d)> open = new();
            open.AddLast((source, 0));
            visited.Add(source);

            while(open.Count > 0) {
                var current = open.First();
                open.RemoveFirst();
                foreach(Node node in current.n.connections) {
                    if(!visited.Contains(node)) {
                        cost.Add((source.idx, node.idx), current.d + 1);
                        open.AddLast((node, current.d + 1));
                        visited.Add(node);
                    }
                }
            }
        }

        struct State {
            public ulong open;
            public int depth;
            public int current;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            Dictionary<(int, int), int> cost = new();
            foreach(Node node in input) {
                FillCost(node, cost);
            }

            var start = input.First(n => n.id == "AA");

            int maxDepth = 30;
            Dictionary<State, int> lookup = new();
            Queue<(State s, Node n, int v)> queue = new();

            queue.Enqueue((new State(), start, 0));

            var best = 0;
            var maxFlow = input.Sum(n => n.flow);

            while(queue.Count > 0) {
                var current = queue.Dequeue();

                foreach(Node node in input) {
                    if((current.s.open & (1UL << node.idx)) == 0 && node.flow > 0) {
                        if(!cost.TryGetValue((current.n.idx, node.idx), out var moveCost)) {
                            continue;
                        }

                        var nextState = new State() {
                            current = node.idx,
                            depth = current.s.depth + moveCost + 1,
                            open = current.s.open | (1UL << node.idx)
                        };
                        var remaining = maxDepth - nextState.depth;

                        if(remaining <= 0)
                            continue;

                        var nextValue = current.v + node.flow * remaining;

                        if(best < nextValue) {
                            best = nextValue;
                        }

                        int potential = Potential(nextState.open, cost, node, remaining, input, nextValue);
                        if(potential < best) {
                            continue;
                        }

                        if(lookup.TryGetValue(nextState, out var prevValue)) {
                            if(prevValue >= nextValue) {
                                continue;
                            }
                        }

                        queue.Enqueue((nextState, node, nextValue));
                        lookup[nextState] = nextValue;
                    }
                }
            }

            return lookup.Max(l => l.Value);
        }
        private static int Potential(ulong open, Dictionary<(int, int), int> cost, Node node, int remaining, List<Node> input, int nextValue) {
            var sr = 0;
            for(int i = 0; i < 64; i++) {
                if((open & (1UL << i)) == 0) {
                    if(cost.TryGetValue((node.idx, i), out var mv)) {
                        var r2 = remaining - mv - 1;
                        if(r2 > 0) {
                            sr += input[i].flow * r2;
                        }
                    }
                }
            }

            var potential = nextValue + sr;
            return potential;
        }

        struct State2 {
            public ulong open;
            public Node p1;
            public Node p2;
            public int depth1;
            public int depth2;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            Dictionary<(int, int), int> cost = new();
            foreach(Node node in input) {
                FillCost(node, cost);
            }

            var start = input.First(n => n.id == "AA");

            int maxDepth = 26;
            Dictionary<State2, (int value, State2 prev)> lookup = new();
            Queue<(State2 s, int v)> queue = new();

            queue.Enqueue((new State2() {
                p1 = start,
                p2 = start
            }, 0));

            var best = 0;
            var maxFlow = input.Sum(n => n.flow);

            while(queue.Count > 0) {
                var current = queue.Dequeue();

                Node cn;
                int cd;
                int id;
                if(current.s.depth1 <= current.s.depth2) {
                    cn = current.s.p1;
                    cd = current.s.depth1;
                    id = 1;
                }
                else {
                    cn = current.s.p2;
                    cd = current.s.depth2;
                    id = 2;
                }

                foreach(Node node in input) {
                    if((current.s.open & (1UL << node.idx)) == 0 && node.flow > 0) {
                        if(!cost.TryGetValue((cn.idx, node.idx), out var moveCost)) {
                            continue;
                        }

                        var nextState = new State2() {
                            //depth = cd + moveCost + 1,
                            open = current.s.open | (1UL << node.idx)
                        };

                        var nd = cd + moveCost + 1;

                        if(id == 1) {
                            nextState.p1 = node;
                            nextState.depth1 = nd;
                            nextState.p2 = current.s.p2;
                            nextState.depth2 = current.s.depth2;
                        }
                        else {
                            nextState.p2 = node;
                            nextState.depth2 = nd;
                            nextState.p1 = current.s.p1;
                            nextState.depth1 = current.s.depth1;
                        }
                        
                        var remaining = maxDepth - nd;
                        if(remaining <= 0)
                            continue;

                        var nextValue = current.v + node.flow * remaining;
                        if(best < nextValue) {
                            best = nextValue;
                        }

                        int potential = Potential(nextState.open, cost, node, remaining, input, nextValue);
                        if(potential < best) {
                            continue;
                        }

                        if(lookup.TryGetValue(nextState, out var prevValue)) {
                            if(prevValue.value >= nextValue) {
                                continue;
                            }
                        }

                        queue.Enqueue((nextState, nextValue));
                        lookup[nextState] = (nextValue, current.s);
                    }
                }
            }

            var ans = lookup.Max(l => l.Value.value);

         //   if(ans == 1707) 
            {
                var c = lookup.First(l => l.Value.value == ans);
                List<(State2 s, int v)> log = new();
                log.Add((c.Key, c.Value.value));
                while(lookup.TryGetValue(c.Value.prev, out var p)) {
                    c = new(c.Value.prev, p);
                    log.Add((c.Key, c.Value.value));
                }

                log.Reverse();
                foreach((State2 s, int v) l in log) {
                    Console.WriteLine($"p1:{l.s.depth1} p2:{l.s.depth2} {l.v} {Convert.ToString((long)l.s.open, 2)}");
                }
            }
            
            if(ans <= 2180 && ans != 1707) {
                return "Too low: " + ans;
            }
            else {
                return ans;
            }
        }
    }
}