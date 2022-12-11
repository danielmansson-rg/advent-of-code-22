namespace Days {
    public class Day10 : DaySolverBase {
        public override string Example1 =>
            @"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop";

        List<int> Transform(string raw) {
            return raw
                .Replace("addx", "noop\naddx")
                .Replace("noop", "addx 0")
                .Split("\n")
                .Select(l => l.Split(" ")[1])
                .Select(int.Parse)
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            int x = 1;
            int sum = 0;
            for(int i = 0; i < input.Count; i++) {
                if((i + 21) % 40 == 0) {
                    // Console.WriteLine($"Cycle {i + 1} * {x} = {(i + 1) * x}");
                    sum += (i + 1) * x;
                }
                var add = input[i];
                x += add;
            }

            return sum;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            int sprite = 1;
            int x = 1;
            for(int i = 0; i < input.Count; i++) {
                if(i % 40 == 0) {
                    Console.WriteLine();
                }
                bool visible = IsVisible(sprite, i);
                Console.ForegroundColor = visible ? ConsoleColor.Yellow : ConsoleColor.Black;
                Console.BackgroundColor = visible ? ConsoleColor.Red : ConsoleColor.DarkGreen;
                Console.Write(visible ? '#' : '@');

                var add = input[i];
                x += add;
                if(add != 0) {
                    sprite = x;
                }
            }

            Console.ResetColor();
            Console.WriteLine();

            return -1;
        }

        private static bool IsVisible(int spritePos, int instrIdx) {
            var pixelPos = instrIdx % 40;
            return pixelPos >= spritePos - 1 && pixelPos <= spritePos + 1;
        }
    }
}