namespace Days {
    public class Day16 : DaySolverBase {

        class Node {
            public string id;
            public int flow;
            public List<string> rawConnections = new();
            public List<Node> connections = new();
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
                .Select(l => {
                    var s = l.Split(" ");
                    return new Node() {
                        id = s[1],
                        flow = int.Parse(s[4]),
                        rawConnections = s.Skip(9).ToList()
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
            public HashSet<Node> open = new();
            public int maxFlow;
            public int currentFlow;
            public int best;
        }

        struct State2 {
            public Node current;
            public int currentFlow;
            public int depth;
            public int released;
            public HashSet<Node> open = new();
        }
        
        void Search( State2 state, PriorityQueue<State2, int> queue, int depthLimit, int maxNodes, int maxFlow) {
            var remaining = depthLimit - state.depth;

            // var potential = state.released + maxFlow * remaining;
            // if(state.best > potential) {
            //     return released;
            // }
            //
            // if(state.depth == depthLimit) {
            //     return state.released;
            // }
            //
            // if(state.open.Count == maxNodes) {
            //     return state.released + state.currentFlow * remaining;
            // }
            //
            // //Open valve
            // if(!state.open.Contains(state.current)) {
            //     state.open.Add(state.current);
            //     state.currentFlow += state.current.flow;
            //     var value = DFS(state.current, state, depth + 1, state.current + state.currentFlow, depthLimit, maxNodes);
            //
            //     state.currentFlow -= state.current.flow;
            //     state.open.Remove(state.current);
            // }
            //
            // //Move
            // foreach(Node next in current.connections) {
            //     var value = DFS(next, state, depth + 1, released + state.open.Sum(n => n.flow), depthLimit, maxNodes);
            //
            // }
            //
            // return state.best;
        }

        int DFS(Node current, State state, int depth, int released, int depthLimit, int maxNodes) {
            var remaining = depthLimit - depth;

            var potential = released + state.maxFlow * remaining;
            if(state.best > potential) {
                return released;
            }
            
            if(depth == depthLimit) {
                return released;
            }
            
            if(state.open.Count == maxNodes) {
                return released + state.currentFlow * remaining;
            }
            
            //Open valve
            if(!state.open.Contains(current)) {
                state.open.Add(current);
                state.currentFlow += current.flow;
                var value = DFS(current, state, depth + 1, released + state.currentFlow, depthLimit, maxNodes);
                if(value > state.best) {
                    Console.WriteLine("New best " + state.best);
                    state.best = value;
                }
                state.currentFlow -= current.flow;
                state.open.Remove(current);
            }

            //Move
            foreach(Node next in current.connections) {
                var value = DFS(next, state, depth + 1, released + state.open.Sum(n => n.flow), depthLimit, maxNodes);
                if(value > state.best) {
                    Console.WriteLine("New best m " + state.best);
                    state.best = value;
                }
            }

            return state.best;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            var start = input.First(n => n.id == "AA");

            int value = DFS(start, new State() {
                maxFlow = input.Sum(n=> n.flow)
            }, 0, 0, 29, input.Count);

            return value;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}