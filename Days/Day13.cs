using System.Text.RegularExpressions;

namespace Days {
    public class Day13 : DaySolverBase {
        public override string Example1 =>
            @"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]";

        class Node {
            public Node parent = null;
            public List<Node> list = new();
            public int? value = null;
        }
        
        class Pair {
            public string rawFirst;
            public string rawSecond;
            public Node first;
            public Node second;
            public void Build() {
                first = BuildNode(rawFirst);
                second = BuildNode(rawSecond);
            }

            Node BuildNode(string input) {
                var tokens = Regex.Split(input, @"([0-9]+|\]|\[|,)")
                    .Where(s => s != "," && s != "")
                    .ToList();

                Node current = null;
                Node root = null;
                
                foreach(string token in tokens) {
                    switch(token) {
                        case "[":
                            var node = new Node();
                            if(current == null) {
                                root = node;
                            }
                            else {
                                current.list.Add(node);
                                node.parent = current;
                            }
                            current = node;
                            break;
                        case "]":
                            current = current.parent;
                            break;
                        default:
                            current.list.Add(new Node() {
                                value = int.Parse(token),
                                parent = current
                            });
                            break;
                    }
                }

                return root;
            }
        }

        List<Pair> Transform(string raw) {
            var result = raw.Split("\n\n")
                .Select(i => {
                    var s = i.Split("\n");
                    return new Pair() {
                        rawFirst = s[0],
                        rawSecond = s[1]
                    };
                })
                .ToList();

            foreach (Pair pair in result) {
                pair.Build();
            }

            return result;
        }

        private bool IsInRightOrder(Node first, Node second) {
            throw new NotImplementedException();
        }
        
        public override object Solve1(string raw) {
            var input = Transform(raw);

            var result = new List<int>();
            for(int i = 0; i < input.Count; i++) {
                var p = input[i];
                if(IsInRightOrder(p.first, p.second)) {
                    result.Add(i + 1);
                }
            }

            return result.Sum();
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}