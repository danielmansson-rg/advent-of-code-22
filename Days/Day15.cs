namespace Days {
    public class Day15 : DaySolverBase {
        public override string Example1 =>
            @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3";

        class Entry {
            public Entry(string raw) {
                var s = raw
                    .Replace(",", "")
                    .Replace("x=", "")
                    .Replace("y=", "")
                    .Replace(":", "")
                    .Split(" ");

                sensor = (int.Parse(s[2]), int.Parse(s[3]));
                beacon = (int.Parse(s[8]), int.Parse(s[9]));
                radius = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
                rangeX = (sensor.x - radius, sensor.x + radius);
                rangeY = (sensor.y - radius, sensor.y + radius);
            }

            public (int x, int y) sensor;
            public (int x, int y) beacon;
            public int radius;
            public (int min, int max) rangeY;
            public (int min, int max) rangeX;

            public (int min, int max) GetSpanX(int y) {
                var d = Math.Abs(sensor.y - y);
                return (rangeX.min + d, rangeX.max - d);
            }
        }

        List<Entry> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => new Entry(l))
                .ToList();
        }

        private List<(int min, int max)> GenerateSpans(List<Entry> input, int targetY) {
            var spans = input
                .Select(e => e.GetSpanX(targetY))
                .Where(s => s.min <= s.max)
                .ToList();

            for(int i = 0; i < spans.Count; i++) {
                for(int j = i + 1; j < spans.Count; ++j) {
                    if(Overlap(spans[i], spans[j])) {
                        spans[i] = Merge(spans[i], spans[j]);
                        spans.RemoveAt(j);
                        i--;
                        break;
                    }
                }
            }

            return spans;
        }

        private bool Overlap((int min, int max) a, (int min, int max) b) {
            var isOutside = b.min > a.max || b.max < a.min;
            return !isOutside;
        }

        private (int min, int max) Merge((int min, int max) a, (int min, int max) b) {
            return (Math.Min(a.min, b.min), Math.Max(a.max, b.max));
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);
            int targetY = input.Count < 15 ? 10 : 2000000;
            List<(int min, int max)> spans = GenerateSpans(input, targetY);

            var cover = spans
                .Select(s => s.max + 1 - s.min)
                .Sum();

            var beacons = input
                .Select(e => e.beacon)
                .Where(b => b.y == targetY)
                .Distinct()
                .Count(b => spans.Any(s => Overlap(s, (b.x, b.x))));

            return cover - beacons;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);
            int max = input.Count < 15 ? 20 : 4000000;

            List<(int min, int max)> spans = new();
            for(int i = 0; i <= max; i++) {
                //List<(int min, int max)> spans = GenerateSpans(input, i);

                //Quick test optimization to just skip heavy allocs in GenerateSpans. 2x speedup
                spans.Clear();
                foreach(var e in input) {
                    var s = e.GetSpanX(i);
                    if(s.min <= s.max) {
                        spans.Add(s);
                    }
                }

                int x = 0;
                while(x < max) {
                    (int min, int max) current = spans.FirstOrDefault(s => Overlap(s, (x, x)));
                    if(current == (0, 0)) {
                        return $"Found at {x}, {i}: {x * 4000000L + i}";
                    }

                    x = current.max + 1;
                }
            }

            return -1;
        }
    }
}