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
        
        string ToString(Node node) {
            if(node.value.HasValue) {
                return node.value.ToString();
            }

            return $"[{string.Join(",", node.list.Select(n => ToString(n)))}]";
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

        enum Outcome {
            InProgress = 0,
            Correct = -1,
            Wrong = 1
        }
        
        private Outcome IsInRightOrder(Node first, Node second, int depth) {
            if(first.value.HasValue && second.value.HasValue) {
                //Console.WriteLine($"{new string(' ', depth)}Compare values {first.value} {second.value}");
                if(first.value < second.value) {
                    //Console.WriteLine($"{new string(' ', depth)}LS smaller Correct! {first.value} {second.value}");

                    return Outcome.Correct;
                } 
                else if(first.value > second.value) {
                    //Console.WriteLine($"{new string(' ', depth)}RS smaller Wrong! {first.value} {second.value}");
                    return Outcome.Wrong;
                }
                else {
                    return Outcome.InProgress;
                }
            }

            if(first.value.HasValue) {
                //Console.WriteLine($"{new string(' ', depth)}Convert first to list: {first.value}");

                first = new Node() {
                    list = new List<Node>() {
                        new Node() {
                            value = first.value
                        }
                    }
                };
            }

            if(second.value.HasValue) {
                //Console.WriteLine($"{new string(' ', depth)}Convert first to list: {second.value}");
     
                second = new Node() {
                    list = new List<Node>() {
                        new Node() {
                            value = second.value
                        }
                    }
                };
            }

            //Console.WriteLine($"{new string(' ', depth)}cmp lists");
            for(int i = 0; i < first.list.Count; i++) {
                if(i >= second.list.Count) {
                    //Console.WriteLine($"{new string(' ', depth)}RS out of items");
                    return Outcome.Wrong;
                }

                var a = first.list[i];
                var b = second.list[i];

                var outcome = IsInRightOrder(a, b, depth + 1);
                if(outcome != Outcome.InProgress)
                    return outcome;
            }

            if(first.list.Count < second.list.Count) {
                //Console.WriteLine($"{new string(' ', depth)}LS out of items");
                return Outcome.Correct;
            }

            return Outcome.InProgress;
        }

        
        public override object Solve1(string raw) {
            var input = Transform(raw);

            var result = new List<int>();
            for(int i = 0; i < input.Count; i++) {
                var p = input[i];
                //Console.WriteLine($"Pair {i + 1}\n");
                var outcome = IsInRightOrder(p.first, p.second, 0);
                if(outcome == Outcome.Correct) {
                    result.Add(i + 1);
                }
                //Console.WriteLine($"Outcome: {outcome}\n");

            }

            return result.Sum();
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            List<Node> all = input.SelectMany(i => new[] { i.first, i.second }).ToList();
            var first = new Node() {
                list = new() {
                    new Node() {
                        list = new() {
                            new Node() {
                                value = 6
                            }
                        }
                    }
                }
            };
            var second = new Node() {
                list = new() {
                    new Node() {
                        list = new() {
                            new Node() {
                                value = 2
                            }
                        }
                    }
                }
            };
            all.Add(first);            
            all.Add(second);   

            all.Sort((a, b) => (int)IsInRightOrder(a, b, 0));

            // foreach(Node node in all) {
            //     Console.WriteLine(ToString(node));
            // }
            return (all.IndexOf(first) + 1) * (all.IndexOf(second) + 1);
        }
    }
}