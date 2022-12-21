namespace Days {
    public class Day17 : DaySolverBase {
        public override string Example1 =>
            @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

        public List<List<(int x, int y)>> parts = new() {
            new() {
                (0, 0),
                (1, 0),
                (2, 0),
                (3, 0),
            },
            new() {
                (1, 0),
                (0, 1),
                (1, 1),
                (2, 1),
                (1, 2),
            },
            new() {
                (2, 0),
                (2, 1),
                (0, 2),
                (1, 2),
                (2, 2),
            },
            new() {
                (0, 0),
                (0, 1),
                (0, 2),
                (0, 3),
            },
            new() {
                (0, 0),
                (1, 0),
                (0, 1),
                (1, 1),
            },
        };

        private List<(int x, int y)> rockSize = new() {
            (4, 1),
            (3, 3),
            (3, 3),
            (1, 4),
            (2, 2)
        };

        class Rock {
            public int type;
            public (int x, int y) pos;
        }

        string Transform(string raw) {
            return raw;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);
            int caveWidth = 7;
            int highestClearY = 0;
            HashSet<(int x, int y)> resting = new();

            int[] heights = new int[caveWidth];
            int mod = input.Length * rockSize.Count;
            int jet = 0;
            for(int i = 0; i < 2022; i++) {
                var type = i % parts.Count;
                var size = rockSize[type];
                (int x, int y) pos = (2, highestClearY + size.y + 2);
                var limit = pos.y + 1;
                
                if(i % mod == 0) {
                   // Print(pos, resting, caveWidth, type, limit);
                }
                for(int it = 0; it < limit; it++) {

                    //Push
                    var dir = input[jet % input.Length];
                    jet++;
                    var np = (pos.x + (dir == '<' ? -1 : 1), pos.y);
                    if(!Collides(np, resting, caveWidth, type)) {
                        pos = np;
                    }
                    //Print(pos, resting, caveWidth, type, limit);

                    //Move
                    np = (pos.x, pos.y - 1);
                    if(!Collides(np, resting, caveWidth, type)) {
                        pos = np;
                        //Print(pos, resting, caveWidth, type, limit);
                        int a = 232;
                    }
                    else {
                        //Add to resting
                        foreach((int x, int y) localPart in parts[type]) {
                            (int x, int y) p = (pos.x + localPart.x, pos.y - localPart.y);
                            resting.Add(p);

                            for(int j = 0; j < caveWidth; j++) {
                                if(p.y > heights[j]) {
                                    heights[j] = p.y;
                                }
                            }
                            
                            if(p.y + 1 > highestClearY) {
                                highestClearY = p.y + 1;
                            }
                        }
                       // Print(pos, resting, caveWidth, type, limit);

                        break;
                    }
                }

              //  resting.RemoveWhere(p => p.y < highestClearY - 16);
            }

            return highestClearY;
        }

        private static int maxH = 0;
        ulong GetHash(int y, int[] heights) {
            ulong hash = 0UL;
            
            for(int i = 0; i < 7; i++) {
                var diff = y - heights[i];
                if(diff > 255) {
                    diff = 255;
                }
                if(diff > maxH) {
                    maxH = diff;
                }
                hash |= ((ulong)diff << (8 * i));
            }

            return hash;
        }


        public void Print((int x, int y) pos, HashSet<(int x, int y)> resting, int caveWidth, int rockType, int highestY) {
           // return;
            HashSet<(int x, int y)> active = new();
            foreach((int x, int y) localPart in parts[rockType]) {
                (int x, int y) p = (pos.x + localPart.x, pos.y - localPart.y);
                active.Add(p);
            }

            var end = Math.Max(highestY - 15, 0);
            
            for(int i = highestY ; i >= end; i--) {
                Console.Write('|');
                for(int j = 0; j < caveWidth; j++) {
                    if(resting.Contains((j, i))) {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('#');
                    }
                    else if(active.Contains((j, i))) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('@');
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write('.');
                    }
                    Console.ResetColor();
                }
                Console.Write('|');
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(i);
                Console.ResetColor();

            }

            Console.WriteLine("+-------+");
            Console.WriteLine();
            
            Console.ResetColor();
        }
        

        private bool Collides((int x, int y) pos, HashSet<(int x, int y)> resting, int caveWidth, int rockType) {
            if(pos.x < 0)
                return true;

            var size = rockSize[rockType];
            if(pos.x + size.x > caveWidth)
                return true;

            if(pos.y - size.y + 1 < 0) {
                return true;
            }

            foreach((int x, int y) localPart in parts[rockType]) {
                (int x, int y) p = (pos.x + localPart.x, pos.y - localPart.y);
                if(resting.Contains(p)) {
                    return true;
                }
            }

            return false;
        }
        
        public override object Solve2(string raw) {
           var input = Transform(raw);
            int caveWidth = 7;
            int highestClearY = 0;
            HashSet<(int x, int y)> resting = new();
            Dictionary<(ulong heightHash, int typeIdx, int jetIdx), int> prev = new(); 
            List<(ulong heightHash, int typeIdx, int jetIdx, int value)> prevList = new(); 

            int[] heights = new int[caveWidth];
            int mod = input.Length * rockSize.Count;
            int jet = 0;
            for(int i = 0; i < 2022; i++) {
                var type = i % parts.Count;
                var size = rockSize[type];
                (int x, int y) pos = (2, highestClearY + size.y + 2);
                var limit = pos.y + 1;

                var hash = GetHash(highestClearY, heights);
                (ulong hash, int type, int jet) o = (hash, type, jet % input.Length);
                if(prev.TryGetValue(o, out var idx)) {
                    Console.WriteLine($"Seen before at {idx}");
                    return Calc(prevList, idx, highestClearY, i);
                }
                /*
                 *   #####
                 *      ##
                 *      ##
                 *      ##
                 * #######
                 */
                prev.Add(o, prevList.Count);
                var prevHighClearY = highestClearY;
                
                for(int it = 0; it < limit; it++) {

                    //Push
                    var dir = input[jet % input.Length];
                    jet++;
                    var np = (pos.x + (dir == '<' ? -1 : 1), pos.y);
                    if(!Collides(np, resting, caveWidth, type)) {
                        pos = np;
                    }

                    //Move
                    np = (pos.x, pos.y - 1);
                    if(!Collides(np, resting, caveWidth, type)) {
                        pos = np;
                        int a = 232;
                    }
                    else {
                        //Add to resting
                        foreach((int x, int y) localPart in parts[type]) {
                            (int x, int y) p = (pos.x + localPart.x, pos.y - localPart.y);
                            resting.Add(p);

                                if(p.y > heights[p.x]) {
                                    heights[p.x] = p.y;
                                }
                            
                            if(p.y + 1 > highestClearY) {
                                highestClearY = p.y + 1;
                            }
                        }

                        break;
                    }
                }

                var v = highestClearY - prevHighClearY;
                prevList.Add((o.hash, o.type, o.jet, v));
            }

            return highestClearY;
        }
        private object Calc(List<(ulong heightHash, int typeIdx, int jetIdx, int value)> list, int startIdx, int value, int i) {
            ulong result = (ulong)value;
            ulong rem = 1000000000000 - (ulong)i;
            var l1 = list.TakeLast(list.Count - startIdx).ToList();

            ulong fullCycles = rem / (ulong)l1.Count;
            ulong sum = (ulong)l1.Sum(c => c.value);

            result += fullCycles * sum;
            rem -= fullCycles * (ulong)l1.Count;
            
            for(ulong j = 0; j < rem; j++) {
                int l = l1[(int)(j % (ulong)l1.Count)].value;
                result += (ulong)l;
            }
            
            return result;
        }
    }
}