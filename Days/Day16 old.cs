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
            return "skip";
            var input = Transform(raw);

            var start = input.First(n => n.id == "AA");

            int value = BFS(start, 30, input);

            return value;
        }

        struct BfsState2 {
            public Node p1;
            public Node p2;
            public int totalFlow;
            public int released;
            public int depth;
            public ulong open;
            public ulong visited1;
            public ulong visited2;
        }

        int BFS2(Node start, int depthLimit, List<Node> input) {

            var queue = new Queue<BfsState2>();

            queue.Enqueue(new BfsState2() {
                p1 = start,
            });

            int best = 0;
            int it = 0;
            BfsState2 bestState = new();
            List<BfsState2> history = new();

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

                if(current.depth + 1 >= depthLimit) {
                    continue;
                }
                
                GetActions(current, current.p1, 1, actionResult1);
                GetActions(current, current.p2, 2, actionResult2);

                foreach(var a1 in actionResult1) {
                    foreach(var a2 in actionResult2) {
                        var next = new BfsState2() {
                            depth = current.depth + 1,
                            visited1 = current.visited1,
                            visited2 = current.visited2,
                            open = current.open,
                            totalFlow = current.totalFlow,
                        };
                        
                        if(a1.t == 'o') {
                            next.p1 = a1.p;
                            next.totalFlow += a1.p.flow;
                            next.open |= (1UL << a1.p.idx);
                            next.visited1 = 0UL;
                        }
                        else {
                            ulong visitFlag = (1UL << a1.con.idx);
                            next.p1 = a1.p;
                            next.visited1 |= visitFlag;
                        }
                        
                        if(a2.t == 'o') {
                            next.p2 = a2.p;
                            next.totalFlow += a2.p.flow;
                            next.open |= (1UL << a2.p.idx);
                            next.visited2 = 0UL;
                        }
                        else {
                            ulong visitFlag = (1UL << a2.con.idx);
                            next.p2 = a2.p;
                            next.visited2 |= visitFlag;
                        }
                        
                        next.released = current.released + current.totalFlow;
                        queue.Enqueue(next);
                    }
                }
                

                foreach(var action in actionResult1) {
                    if(action.t == 'o') {
                        var next = new BfsState2() {
                            p1 = action.p,
                            depth = current.depth + 1,
                            totalFlow = current.totalFlow + action.p.flow,
                            open = current.open | (1UL << action.p.idx),
                            visited1 = action.id == 1 ? 0UL : current.visited1,
                            visited2 = action.id == 2 ? 0UL : current.visited2,
                        };
                        next.released = current.released + current.totalFlow;

                        queue.Enqueue(next);
                    }
                    else {
                        ulong visitFlag = (1UL << action.con.idx);

                        var next = new BfsState2() {
                            p1 = action.con,
                            depth = current.depth + 1,
                            totalFlow = current.totalFlow,
                            open = current.open,
                            visited1 = current.visited1 | visitFlag
                        };
                        next.released = current.released + current.totalFlow;

                        queue.Enqueue(next);
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
        
        List<(char t, Node p, Node con, int id)> actionResult1 = new List<(char, Node, Node, int)>();
        List<(char t, Node p, Node con, int id)> actionResult2 = new List<(char, Node, Node, int)>();

        private void GetActions(in BfsState2 current, Node p, int id,List<(char, Node, Node, int)> actionResult) {
            actionResult.Clear();
            //Open valve
            if((current.open & (1UL << current.p1.idx)) == 0 && p.flow > 0) {
                actionResult.Add(('o', p, null, id));
            }

            //Move
            foreach(Node connection in p.connections) {
                ulong visitFlag = (1UL << connection.idx);

                if((current.visited1 & visitFlag) != 0) {
                    continue;
                }

                actionResult.Add(('m', p, connection, id));
            }
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            var start = input.First(n => n.id == "AA");

            int value = BFS2(start, 30, input);

            return value;
        }
    }
}