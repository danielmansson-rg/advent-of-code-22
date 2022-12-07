namespace Days {
    public class Day7 : DaySolverBase {
        public override string Example1 =>
            @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";

        List<string> Transform(string raw) {
            return raw
                .Split("\n")
                .ToList();
        }

        class Dir {
            public string name;
            public int size;
            public List<Dir> dirs = new();
            public List<File> files = new();
        }

        class File {
            public string name;
            public int size;
        }

        public override object Solve1(string raw) {
            var input = Transform(raw);

            var root = BuildTree(input);
            int totalSize = GetAndSetSize(root);

            List<int> dirSizes = new();
            GetAllFilter(root, dirSizes, d => d.size);
            
            return dirSizes
                .Where(s => s <= 100000)
                .Sum();
        }

        void GetAllFilter<T>(Dir dir, List<T> list, Func<Dir, T> f) {
            list.Add(f(dir));
            foreach(Dir d in dir.dirs) {
                GetAllFilter<T>(d, list, f);
            }
        }
        
        int GetAndSetSize(Dir dir) {
            int size = dir.files.Sum(f => f.size);
            size += dir.dirs.Select(GetAndSetSize).Sum();
            dir.size = size;
            return size;
        }
        
        private static Dir BuildTree(List<string> input) {
            Dir root = new() {
                name = "/"
            };
            List<Dir> wd = new() {
                root
            };

            for(int i = 1; i < input.Count; i++) {
                var line = input[i];
                var split = line.Split(" ");
                switch(split[1]) {
                    case "cd":
                        if(split[2] == "..") {
                            wd.RemoveAt(wd.Count - 1);
                        }
                        else {
                            var next = wd.Last().dirs
                                .FirstOrDefault(d => d.name == split[2]);
                            wd.Add(next);
                        }
                        break;
                    case "ls":
                        i++;
                        for(; i < input.Count; i++) {
                            if(input[i].StartsWith("$")) {
                                i--;
                                break;
                            }

                            var s = input[i].Split(" ");
                            if(s[0] == "dir") {
                                wd.Last().dirs.Add(new() {
                                    name = s[1]
                                });
                            }
                            else {
                                wd.Last().files.Add(new() {
                                    name = s[1],
                                    size = int.Parse(s[0])
                                });
                            }
                        }
                        break;
                }
            }

            return root;
        }

        public override object Solve2(string raw) {
            var input = Transform(raw);

            var root = BuildTree(input);
            int totalSize = GetAndSetSize(root);
            
            List<Dir> dirs = new();
            GetAllFilter(root, dirs, d => d);
            dirs = dirs.OrderBy(d => d.size).ToList();

            int available = 70000000 - totalSize;
            int needed = 30000000 - available;
            return dirs
                .First(d => d.size >= needed)
                .size;
        }
    }
}