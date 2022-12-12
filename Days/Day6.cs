namespace Days {
    public class Day6 : DaySolverBase {
        public override string Example1 =>
            @"nppdvjthqldpwncqszvftbrmjlhg";

        string Transform(string raw) {
            return raw;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            for(int i = 0; i < input.Length - 3; i++) {
                if(input.Substring(i, 4).Distinct().Count() == 4)
                    return i + 4;
            }

            return -1;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            for(int i = 0; i < input.Length - 13; i++) {
                if(input.Substring(i, 14).Distinct().Count() == 14)
                    return i + 14;
            }

            return -1;
        }
    }
}