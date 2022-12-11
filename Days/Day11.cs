namespace Days {
    public class Day11 : DaySolverBase {
        public override string Example1 =>
            @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1";

        class Monkey {
            public List<Item> Items = new();
            public Operation Operation;
            public long TestDivisable;
            public int TargetIfTrue;
            public int TargetIfFalse;
            public long InspectionCount;
        }

        class Operation {
            private string op;
            private string rawValue;
            
            public Operation(string raw) {
                var s = raw.Split(" ");
                op = s.TakeLast(2).First();
                rawValue = s.TakeLast(1).First();
            }

            public long Perform(long oldValue) {
                long value;
                if(!long.TryParse(rawValue, out value)) {
                    value = oldValue;
                }

                switch(op) {
                    case "*":
                        return oldValue * value;
                    case "+":
                        return oldValue + value;
                }

                throw new Exception($"Unsupported op: {op}");
            }
        }
        
        class Item {
            public long Value;
        }
        
        List<Monkey> Transform(string raw) {
            return raw
                .Split("\n\n")
                .Select(s => {
                    var l = s.Split("\n");
                    return new Monkey() {
                        Items = l[1]
                            .Replace(",", "")
                            .Split(" ")
                            .Skip(4)
                            .Select(int.Parse)
                            .Select(i => new Item(){ Value = i})
                            .ToList(),
                        Operation = new Operation(l[2]),
                        TestDivisable = l[3]
                            .Split(" ")
                            .TakeLast(1)
                            .Select(int.Parse)
                            .First(),
                        TargetIfTrue = l[4].
                            Split(" ")
                            .TakeLast(1)
                            .Select(int.Parse)
                            .First(),
                        TargetIfFalse = l[5]
                            .Split(" ")
                            .TakeLast(1)
                            .Select(int.Parse)
                            .First(),
                    };
                })
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            for(int i = 0; i < 20; i++) {
                foreach(Monkey monkey in input) {
                    foreach(Item item in monkey.Items) {
                        monkey.InspectionCount++;
                        item.Value = monkey.Operation.Perform(item.Value);
                        item.Value /= 3;
                        if(item.Value % monkey.TestDivisable == 0) {
                            input[monkey.TargetIfTrue].Items.Add(item);
                        }
                        else {
                            input[monkey.TargetIfFalse].Items.Add(item);
                        }
                    }
                    monkey.Items.Clear();
                }
            }

            return input.Select(m => m.InspectionCount)
                .OrderByDescending(i => i)
                .Take(2)
                .Aggregate((a,b) => a * b);
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            var lcd = 1L;
            foreach(Monkey monkey in input) {
                lcd *= monkey.TestDivisable;
            }
            
            for(int i = 0; i < 10000; i++) {
                foreach(Monkey monkey in input) {
                    foreach(Item item in monkey.Items) {
                        monkey.InspectionCount++;
                        item.Value = monkey.Operation.Perform(item.Value);
                        item.Value %= lcd;
                        
                        if(item.Value % monkey.TestDivisable == 0) {
                            input[monkey.TargetIfTrue].Items.Add(item);
                        }
                        else {
                            input[monkey.TargetIfFalse].Items.Add(item);
                        }
                    }
                    monkey.Items.Clear();
                }

                // if(i == 0 || i == 19 || (i + 1) % 1000 == 0) {
                //     Console.WriteLine($"Round {i + 1}");
                //     for(int j = 0; j < input.Count; j++) {
                //         Console.WriteLine($"Monkey {j}: {input[j].InspectionCount}");
                //     }
                //     Console.WriteLine();
                // }
            }

            return input.Select(m => m.InspectionCount)
                .OrderByDescending(i => i)
                .Take(2)
                .Aggregate((a,b) => a * b);
        }
    }
}