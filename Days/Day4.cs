namespace Days {
    public class Day4 : DaySolverBase {
        public override string Example1 =>
            @"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8";

        List<((int l, int h) a, (int l, int h) b)> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => l
                    .Split(",")
                    .Select(p => {
                        var e = p.Split("-");
                        return (int.Parse(e[0]), int.Parse(e[1])); })
                    .ToList())
                .Select(l => (l[0], l[1]))
                .ToList();
        }

        private bool AnyFullyContains((int l, int h) a, (int l, int h) b) {
            return FullyContains(a, b) || FullyContains(b, a);
        }

        private bool FullyContains((int l, int h) a, (int l, int h) b) {
            return a.l <= b.l && a.h >= b.h;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            return input
                .Count(i => AnyFullyContains(i.a, i.b));
        }

        private bool AnyOverlap((int l, int h) a, (int l, int h) b) {
            return !(a.l > b.h || a.h < b.l);
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return input
                .Count(i => AnyOverlap(i.a, i.b));
        }
    }
}