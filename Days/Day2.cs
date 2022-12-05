namespace Days {
    public class Day2 : DaySolverBase {
        public override string Example1 =>
            @"A Y
B X
C Z";

        List<List<char>> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => l
                    .Split(" ")
                    .Select(s => s[0])
                    .ToList())
                .ToList();
        }

        char ToChoice(char c) {
            return c switch {
                'X' => 'A',
                'Y' => 'B',
                'Z' => 'C',
                _ => c
            };
        }

        int ScoreForChoice(char c) {
            return c switch {
                'A' => 1,
                'B' => 2,
                'C' => 3,
                _ => throw new Exception()
            };
        }
        
        int ScoreForOutcome(char a, char b) {
            return 3 * EvaluateRockPaperScissor(a, b);
        }

        int EvaluateRockPaperScissor(char a, char b) {
            return (4 + a - b) % 3;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw)
                .Select(i => {
                    i[1] = ToChoice(i[1]);
                    return i;
                });

            return input
                .Select(i => ScoreForChoice(i[1]) + ScoreForOutcome(i[1], i[0]))
                .Sum();
        }

        char ToChoice2(char target, char opponent) {
            int v = opponent - 'A';
            return target switch {
                'X' => (char)((v + 2) % 3 + 'A'),
                'Y' => opponent,
                'Z' => (char)((v + 1) % 3 + 'A'),
                _ => throw new Exception()
            };
        }
        public override object Solve2(string raw) {
            var input = Transform(raw)
                .Select(i => {
                    i[1] = ToChoice2(i[1], i[0]);
                    return i;
                });

            return input
                .Select(i => ScoreForChoice(i[1]) + ScoreForOutcome(i[1], i[0]))
                .Sum();
        }
    }
}