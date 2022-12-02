namespace Days {
    public class Day1 : DaySolverBase {
        public override string Example1 =>
            @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000";

        List<List<int>> Transform(string raw) {
            return raw
                .Split("\n\n")
                .Select(s => s
                    .Split("\n")
                    .Select(int.Parse)
                    .ToList())
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            return input
                .Select(i => i.Sum())
                .Max();
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return input
                .Select(i => i.Sum())
                .OrderByDescending(i => i)
                .Take(3)
                .Sum();;
        }
    }
}