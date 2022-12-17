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

            int maxDepth = 30;
            Dictionary<State2, int> lookup = new();
            Queue<(State2 s, Node n1, Node n2, int v)> queue = new();

            queue.Enqueue((new State2(), start, start, 0));

            var best = 0;
            var maxFlow = input.Sum(n => n.flow);

            while(queue.Count > 0) {
                var current = queue.Dequeue();

                //alternate instead
                foreach(Node node in input) {
                    foreach(Node node2 in input) {
                        if((current.s.open & (1UL << node.idx)) != 0 || node.flow == 0)
                            continue;
                        if((current.s.open & (1UL << node2.idx)) != 0 || node2.flow == 0)
                            continue;
                        
                        if(!cost.TryGetValue((current.n1.idx, node.idx), out var moveCost)) {
                            continue;
                        }
                        if(!cost.TryGetValue((current.n2.idx, node2.idx), out var moveCost2)) {
                            continue;
                        }

                        var nextState = new State2() {
                            depth1 = current.s.depth1 + moveCost + 1,
                            depth2 = current.s.depth2 + moveCost2 + 1,
                            open = current.s.open //| (1UL << node.idx) | (1UL << node2.idx)
                        };
                        
                        var remaining1 = maxDepth - nextState.depth1;
                        var remaining2 = maxDepth - nextState.depth2;

                        var nextValue = 0;
                        if(remaining1 > 0 && (nextState.open & (1UL << node.idx)) == 0) {
                            nextValue += current.v + node.flow * remaining1;
                            nextState.open |= 1UL << node.idx;
                        }
                        if(remaining2 > 0 && (nextState.open & (1UL << node2.idx)) == 0) {
                            nextValue += current.v + node2.flow * remaining2;
                            nextState.open |= 1UL << node2.idx;
                        }

                        if(best < nextValue) {
                            best = nextValue;
                        }

                        int potential = Potential(nextState.open, cost, node, Math.Max(remaining1, remaining2), input, nextValue);
                        if(potential < best) {
                            continue;
                        }

                        if(lookup.TryGetValue(nextState, out var prevValue)) {
                            if(prevValue >= nextValue) {
                                continue;
                            }
                        }

                        queue.Enqueue((nextState, node, node2, nextValue));
                        lookup[nextState] = nextValue;
                    }
                }
            }

            return lookup.Max(l => l.Value);
        }
    }
}