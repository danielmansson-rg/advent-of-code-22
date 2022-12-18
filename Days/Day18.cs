using System.Numerics;

namespace Days {
    public class Day18 : DaySolverBase {
        public override string Example1 =>
            @"2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5";

        List<Vector3> Transform(string raw) {
            return raw
                .Split("\n")
                .Select(l => {
                    var s = l.Split(",");
                    return new Vector3(
                        int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
                })
                .ToList();
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            Dictionary<Vector3, int> area = new();
            List<Vector3> neighbors = new() {
                new(1,0,0),
                new(-1,0,0),
                new(0,1,0),
                new(0,-1,0),
                new(0,0,1),
                new(0,0,-1),
            };

            foreach(Vector3 p in input) {
                int numN = 0;
                foreach(var offset in neighbors) {
                    var n = p + offset;
                    if(area.TryGetValue(n, out var nv)) {
                        numN++;
                        area[n] = nv - 1;
                    }
                }
                
                area.Add(p, 6 - numN);
            }

            return area.Values.Sum();
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            Dictionary<Vector3, int> hasArea = new();
            List<Vector3> neighbors = new() {
                new(1,0,0),
                new(-1,0,0),
                new(0,1,0),
                new(0,-1,0),
                new(0,0,1),
                new(0,0,-1),
            };

            foreach(Vector3 p in input) {
                int numN = 0;
                foreach(var offset in neighbors) {
                    var n = p + offset;
                    if(hasArea.TryGetValue(n, out var nv)) {
                        numN++;
                        hasArea[n] = nv - 1;
                    }
                }
                
                hasArea.Add(p, 6 - numN);
            }

            var visited = new HashSet<Vector3>();
            var queue = new Queue<Vector3>();

            visited.Add(new Vector3(-2, -2, -2));
            queue.Enqueue(new Vector3(-2, -2, -2));
            int surface = 0;
            
            while(queue.Count > 0) {
                var p = queue.Dequeue();
                
                foreach(var offset in neighbors) {
                    var n = p + offset;
                    
                    if(n.X < -2 || n.X >= 21 
                        || n.Y < -2 || n.Y >= 21
                        || n.Z < -2 || n.Z >= 21)
                        continue;
                    
                    if(visited.Contains(n))
                        continue;

                    if(hasArea.ContainsKey(n)) {
                        surface++;
                        continue;
                    }
                    
                    visited.Add(n);
                    queue.Enqueue(n);
                }
            }
            
            return surface;
        }
    }
}