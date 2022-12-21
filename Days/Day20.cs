namespace Days {
    public class Day20 : DaySolverBase {
        public override string Example1 =>
            @"1
2
-3
3
-2
0
4";

        List<int> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(int.Parse)
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);
            var min = input.Min();
            var mod = (-min / input.Count + 1) * input.Count;
            var idxMap = Enumerable.Range(0, input.Count).ToList();
            var valueToMap = new Dictionary<int, int>();
            for(int i = 0; i < input.Count; i++) {
                valueToMap[input[i]] = i;
            }

            for(int i = 0; i < input.Count; i++) {
              
            }

            return -1;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}