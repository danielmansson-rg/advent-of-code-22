namespace Days {
    public class Day5 : DaySolverBase {
        public override string Example1 =>
            @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";

        class Input {
            public List<List<char>> stacks = new();
            public List<(int count, int from, int to)> moves = new();
        }

        Input Transform(string raw) {
            var split = raw
                .Split("\n\n");

            var input = new Input();

            var top = split[0]
                .Split("\n");

            int numStacks = top[top.Length - 1]
                .Replace("   ", " ")
                .Replace("  ", " ")
                .Trim()
                .Split(" ")
                .Select(int.Parse)
                .Max();
            for(int i = 0; i < numStacks; i++) {
                input.stacks.Add(new());
            }

            int w = numStacks;
            int h = top.Length - 1;

            for(int x = 0; x < w; x++) {
                for(int y = h - 1; y >= 0; y--) {
                    var c = top[y][x * 4 + 1];
                    if(c != ' ') {
                        input.stacks[x].Add(c);
                    }
                }
            }

            input.moves = split[1]
                .Replace("move ", "")
                .Replace(" from", "")
                .Replace(" to", "")
                .Split("\n")
                .Select(l => l
                    .Split(" ")
                    .Select(int.Parse)
                    .ToList()
                )
                .Select(i => (i[0], i[1], i[2]))
                .ToList();

            return input;
        }

        void MoveAll((int count, int from, int to) move, Input input) {
            for(int i = 0; i < move.count; i++) {
                Move(move.from, move.to, input);
            }
        }

        void Move(int from, int to, Input input) {
            var s1 = input.stacks[from - 1];
            if(s1.Count == 0) {
                return;
            }
            var s2 = input.stacks[to - 1];
            s2.Add(s1.Last());
            s1.RemoveAt(s1.Count - 1);
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            foreach(var move in input.moves) {
                MoveAll(move, input);
            }

            return new string(input.stacks
                .Select(s => s.Last())
                .ToArray());
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            foreach(var move in input.moves) {
                MoveAll2(move, input);
            }

            return new string(input.stacks
                .Select(s => s.Last())
                .ToArray());
        }

        void MoveAll2((int count, int from, int to) move, Input input) {
            var s1 = input.stacks[move.from - 1];
            var s2 = input.stacks[move.to - 1];

            move.count = Math.Min(move.count, s1.Count);
            var toMove = s1.TakeLast(move.count).ToList();
            s1.RemoveRange(s1.Count - toMove.Count, toMove.Count);

            s2.AddRange(toMove);
        }
    }
}
