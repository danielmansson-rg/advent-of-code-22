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

        class State {
            public int maxFlow;
            public int currentFlow;
            public int best;
            public int openCount;
        }

        struct State2 {
            public Node current;
            public int currentFlow;
            public int depth;
            public int released;
            public ulong openBits;
            public HashSet<Node> open;
        }

        struct TState {
            public Node current;
            public int currentFlow;
            public int depth;
            public ulong openBits;
        }

        private Dictionary<TState, int> transpositionTable = new();

        int DFS(Node current, State state, int depth, int released, int depthLimit, int maxNodes, ulong openBits) {
            var remaining = depthLimit - depth;

            // var tstate = new TState() {
            //     current = current,
            //     currentFlow = state.currentFlow,
            //     depth = depth,
            //     openBits = openBits
            // };
            //
            // if(transpositionTable.TryGetValue(tstate, out var v)) {
            //     return v;
            // }

            var potential = released + state.maxFlow * remaining;
            if(state.best > potential) {
                // transpositionTable[tstate] = released;
                return released;
            }

            if(depth == depthLimit) {
                // transpositionTable[tstate] = released;
                return released;
            }

            if(state.openCount == maxNodes) {
                //  transpositionTable[tstate] = released + state.currentFlow * remaining;
                return released + state.currentFlow * remaining;
            }

            //Open valve
            if((openBits & (1UL << current.idx)) == 0) {
                //  if(!state.open.Contains(current)) {
                //state.open.Add(current);
                state.currentFlow += current.flow;
                state.openCount++;
                var value = DFS(current, state, depth + 1, released + state.currentFlow, depthLimit, maxNodes, openBits | (1UL << current.idx));
                if(value > state.best) {
                    state.best = value;
                }
                state.currentFlow -= current.flow;
                state.openCount--;
                //state.open.Remove(current);

            }

            //Move
            foreach(Node next in current.connections) {
                var value = DFS(next, state, depth + 1, released + state.currentFlow, depthLimit, maxNodes, openBits);
                if(value > state.best) {
                    state.best = value;
                }
            }

            //   transpositionTable[tstate] = state.best;
            return state.best;
        }

        struct BfsState {
            public Node p1;
            public int totalFlow;
            public int released;
            public int depth;
            public ulong open;
            public int parent;
            public char action;
            public ulong visited;
        }
        int BFS(Node start, int depthLimit, List<Node> input) {

            var queue = new Queue<BfsState>();

            queue.Enqueue(new BfsState() {
                p1 = start,
                parent = -1,
                action = 'S'
            });

            int best = 0;
            int it = 0;
            BfsState bestState = new();
            List<BfsState> history = new();

            while(queue.Count != 0) {
                var current = queue.Dequeue();
               // history.Add(current);
                it++;
                if(it % 1000000 == 0) {
                    Console.WriteLine($"d: {current.depth} q:{queue.Count} b:{best}");
                }

                //stand still
                int remaining = depthLimit - current.depth;
                var value = current.released + remaining * current.totalFlow;
                if(value > best) {
                    best = value;
                    bestState = current;
                    //Console.WriteLine("Best: " + best);
                }

                var sr = 0;
                for(int i = 0; i < input.Count; i++) {
                    sr += input[i].flow;
                }
                var potential = current.released + remaining * sr;
                if(potential < best) {
                      continue;
                }

                //Open valve
                if((current.open & (1UL << current.p1.idx)) == 0 && current.p1.flow > 0) {
                    var next = new BfsState() {
                        p1 = current.p1,
                        depth = current.depth + 1,
                        totalFlow = current.totalFlow + current.p1.flow,
                        open = current.open | (1UL << current.p1.idx),
                        parent = history.Count - 1,
                        action = 'o',
                        visited = 0UL,
                    };
                    next.released = current.released + current.totalFlow;

                    if(next.depth < depthLimit) {
                        queue.Enqueue(next);
                    }
                }

                //Move
                foreach(Node connection in current.p1.connections) {
                    ulong visitFlag = (1UL << connection.idx);

                    if((current.visited & visitFlag) != 0) {
                        continue;
                    }
                    
                    var next = new BfsState() {
                        p1 = connection,
                        depth = current.depth + 1,
                        totalFlow = current.totalFlow,
                        open = current.open,
                        parent = history.Count - 1,
                        action = 'm',
                        visited = current.visited | visitFlag
                    };
                    next.released = current.released + current.totalFlow;

                    // if(!p1Block.Contains(new Block() {
                    //         open = next.open,
                    //         p = next.p1.idx,
                    //      //   d = next.depth
                    //     })) 
                    {
                        if(next.depth < depthLimit) {
                            queue.Enqueue(next);
                        }
                    }
                }

            }
            //
            // var log = new List<BfsState>();
            // var h = bestState;
            // while(h.parent != -1) {
            //     log.Add(h);
            //     h = history[h.parent];
            // }
            // log.Reverse();
            //
            // foreach(BfsState s in log) {
            //     Console.Write($"== Minute {s.depth} == at {s.p1.id}");
            //     Console.Write($"  Open: {Convert.ToString((long)s.open, 2)}");
            //     Console.Write($"  F: {s.totalFlow}");
            //     Console.Write($"  R: {s.released}");
            //     Console.Write($"  A: {s.action}");
            //     Console.WriteLine($"");
            // }
            //
            return best;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            var start = input.First(n => n.id == "AA");

            int value = BFS(start, 30, input);

            return value;
        }
        public object Soslve1(string raw) {
            var input = Transform(raw);

            var start = input.First(n => n.id == "AA");

            PriorityQueue<State2, int> queue = new();

            queue.Enqueue(new State2() {
                current = start,
                currentFlow = 0,
                depth = 0,
                open = new(),
                released = 0
            }, 0);

            int best = 0;
            int depthLimit = 29;
            int maxFlow = input.Sum(n => n.flow);
            int it = 0;
            while(queue.Count > 0) {
                var state = queue.Dequeue();
                var current = state.current;
                var remaining = depthLimit - state.depth;

                if(it % 100000 == 0) {
                    Console.WriteLine($"It {it} q:{queue.Count} b:{best} d:{state.depth}");
                }
                it++;

                var potential = state.released + maxFlow * remaining;
                if(best > potential) {
                    continue;
                }

                if(state.depth == depthLimit) {
                    continue;
                }

                if(state.open.Count == input.Count) {
                    if(state.released + state.currentFlow * remaining > best) {
                        best = state.released + state.currentFlow * remaining;
                    }
                    continue;
                }

                if(state.released > best) {
                    best = state.released;
                }

                //Open valve
                if(!state.open.Contains(current)) {
                    var next = new State2() {
                        current = current,
                        depth = state.depth + 1,
                        currentFlow = state.currentFlow + current.flow,
                        open = new HashSet<Node>(state.open)
                    };

                    next.open.Add(current);
                    next.released = state.released + next.currentFlow;

                    var nextPot = next.released + maxFlow * (depthLimit - next.depth);
                    queue.Enqueue(next, -next.released);
                }

                //Move
                foreach(Node nextNode in current.connections) {
                    var next = new State2() {
                        current = nextNode,
                        depth = state.depth + 1,
                        currentFlow = state.currentFlow,
                        released = state.released + state.currentFlow,
                        open = new HashSet<Node>(state.open)
                    };

                    var nextPot = next.released + maxFlow * (depthLimit - next.depth);
                    queue.Enqueue(next, -next.released);
                }
            }
            Console.WriteLine($"Final it {it}");

            return best;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}