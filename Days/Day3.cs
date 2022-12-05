namespace Days {
    public class Day3 : DaySolverBase {
        public override string Example1 =>
            @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw";

        List<(List<char> first, List<char> second)> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => (
                    l.Take(l.Length / 2).ToList(),
                    l.TakeLast(l.Length / 2).ToList()))
                .ToList();
        }

        int ToPriority(char c) {
            return c >= 'a' ? c - 'a' + 1 : c - 'A' + 27; 
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            return input
                .Select(i => i.first.Intersect(i.second)
                    .First())
                .Select(ToPriority)
                .Sum();
        }

        public override object Solve2(string raw) {
            var input = raw.Split("\n");

            return input
                .Select((value, idx) => (value, idx))
                .GroupBy(i => i.idx / 3)
                .Select(g => g.Select(x => x.value).ToList())
                .Select(i => i[0].Intersect(i[1]).Intersect(i[2]).First())
                .Select(ToPriority)
                .Sum();
        }
    }
}