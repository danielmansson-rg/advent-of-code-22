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

        public override object Solve1(string raw) {
            var input = Transform(raw);

            int targetY = input.Count < 15 ? 10 : 2000000;

            var spans = input
                .Select(e => e.GetSpanX(targetY))
                .Where(s => s.min <= s.max)
                .ToList();

            for(int i = 0; i < spans.Count; i++) {
                for(int j = i + 1; j < spans.Count;) {
                    if(Overlap(spans[i], spans[j])) {
                        spans[i] = Merge(spans[i], spans[j]);
                        spans.RemoveAt(j);
                        i--;
                        break;
                    }
                    else {
                        j++;
                    }
                }    
            }
            
            var cover = spans
                .Select(s => s.max + 1 - s.min)
                .Sum();
            
            return cover;
        }

        private bool Overlap((int min, int max) a, (int min, int max) b) {
            var isOutside = b.min > a.max || b.max < a.min;
            return !isOutside;
        }
        
        private (int min, int max) Merge((int min, int max) a, (int min, int max) b) {
            return (Math.Min(a.min, b.min), Math.Max(a.max, b.max));
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            return -1;
        }
    }
}