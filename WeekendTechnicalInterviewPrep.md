### Part 1: The Role of DSA in 0–5 Years Experience

**Yes, Data Structures and Algorithms (DSA) are critical for 0–5 years experience.**
*   **0–2 Years:** Expect heavy focus on syntax and basic data structures (Arrays, HashMaps, Strings).
*   **2–5 Years:** Expect Medium-level problems focusing on optimization (Time/Space Complexity), Trees, Graphs, and System Design integration.
*   **5+ Years:** DSA becomes less about solving puzzles and more about recognizing patterns in system bottlenecks.

**The "Elite" Approach:** Do not memorize 1,000 solutions. Master the **14 Patterns** that solve 90% of problems. The list below covers the **Top 100 High-Yield Problems** (based on Blind 75, NeetCode 150, and FAANG frequency), tailored for **C#/.NET**.

---

### Part 2: The Top 100 DSA Problems (C# Focused)

I have categorized these into **10 Core Patterns**. For each problem, I provide the **Algorithmic Solution**, **C# Specifics**, and the **Why/When/Reason**.

#### 🟢 Category 1: Arrays & Hashing (The Foundation)
*Focus: Memory layout, O(1) lookups, Frequency counting.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 1 | **Two Sum** | Easy | HashMap | `Dictionary<int, int>` | **How:** Store `complement = target - num`. **Why:** Reduces O(N²) to O(N). **When:** Finding pairs. |
| 2 | **Contains Duplicate** | Easy | HashSet | `HashSet<int>` | **How:** `Add()` returns false if exists. **Why:** O(1) lookup vs O(N) list scan. **When:** Validating uniqueness. |
| 3 | **Valid Anagram** | Easy | Frequency Map | `int[26]` or `Dictionary` | **How:** Count chars in s1, decrement in s2. **Why:** Order doesn't matter, composition does. **When:** String comparison. |
| 4 | **Group Anagrams** | Medium | HashMap Key | `Dictionary<string, List>` | **How:** Sort string to use as Key. **Why:** Groups permutations together. **When:** Categorizing data. |
| 5 | **Top K Frequent Elements** | Medium | Heap/Map | `PriorityQueue` + `Dict` | **How:** Count freq, push to Min-Heap of size K. **Why:** O(N log K) vs O(N log N) sort. **When:** Ranking/Leaderboards. |
| 6 | **Product of Array Except Self** | Medium | Prefix/Suffix | `int[]` | **How:** Multiply all left, then all right. **Why:** Avoids division (handles zeros). **When:** Accumulative metrics. |
| 7 | **Valid Sudoku** | Medium | HashSet | `HashSet<string>` | **How:** Track "row r", "col c", "box b". **Why:** Validates constraints efficiently. **When:** Grid validation. |
| 8 | **Encode/Decode Strings** | Medium | String Manip | `StringBuilder` | **How:** Prepend length to string. **Why:** Handles delimiters safely. **When:** Network serialization. |
| 9 | **Longest Consecutive Sequence** | Medium | HashSet | `HashSet<int>` | **How:** Only count if `num-1` doesn't exist. **Why:** Ensures O(N) by skipping starts. **When:** Range finding. |
| 10 | **First Missing Positive** | Hard | Index Mapping | `int[]` (In-place) | **How:** Place `num` at index `num-1`. **Why:** O(1) space using array indices as hash. **When:** Memory constrained. |

#### 🔵 Category 2: Two Pointers (Optimization)
*Focus: Sorted arrays, reducing nested loops.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 11 | **Valid Palindrome** | Easy | Two Pointers | `string` | **How:** Pointers at start/end, skip non-alnum. **Why:** O(N) single pass. **When:** Symmetry check. |
| 12 | **3Sum** | Medium | Sorted + Pointers | `List<IList<int>>` | **How:** Fix one, use 2Sum on rest. **Why:** Reduces O(N³) to O(N²). **When:** Triplets matching sum. |
| 13 | **Container With Most Water** | Medium | Greedy Pointers | `Math.Max` | **How:** Move shorter line inward. **Why:** Width decreases, need height increase. **When:** Max area/volume. |
| 14 | **Trapping Rain Water** | Hard | Two Pointers | `Math.Min` | **How:** Track max Left and max Right. **Why:** Water trapped by min boundary. **When:** Histogram analysis. |
| 15 | **Best Time to Buy/Sell Stock** | Easy | Min Tracker | `int minPrice` | **How:** Track min so far, calc diff. **Why:** O(N) single pass. **When:** Max profit/delta. |
| 16 | **Buy/Sell Stock II** | Medium | Greedy | `int profit` | **How:** Sum all positive differences. **Why:** Capture every upswing. **When:** Multiple transactions. |
| 17 | **Buy/Sell Stock III** | Medium | DP/State | `int[5]` | **How:** Track 4 states (buy1, sell1, buy2, sell2). **Why:** Limits transaction count. **When:** Limited trades. |
| 18 | **Subarray Sum Equals K** | Medium | Prefix Sum | `Dictionary<int, int>` | **How:** `currSum - k` exists in map. **Why:** Handles negative numbers. **When:** Range sum queries. |
| 19 | **Sort Colors** | Medium | Dutch Flag | `int[]` swaps | **How:** 3 pointers (low, mid, high). **Why:** O(N) sort without library. **When:** Partitioning data. |
| 20 | **Minimum Size Subarray Sum** | Medium | Sliding Window | `while` loop | **How:** Expand right, shrink left while valid. **Why:** Finds smallest valid window. **When:** Threshold checks. |

#### 🟠 Category 3: Sliding Window (Subarrays/Strings)
*Focus: Continuous subsets, dynamic sizing.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 21 | **Longest Substring Without Repeating** | Medium | HashSet/Map | `HashSet<char>` | **How:** Shrink window until unique. **Why:** Maintains validity constraint. **When:** String parsing. |
| 22 | **Longest Repeating Character Replacement** | Medium | Frequency Map | `int[26]` | **How:** `windowLen - maxFreq <= k`. **Why:** Allows k replacements. **When:** Error tolerance. |
| 23 | **Permutation in String** | Medium | Frequency Map | `int[26]` | **How:** Compare freq maps of window & target. **Why:** Order doesn't matter for permutation. **When:** Pattern matching. |
| 24 | **Minimum Window Substring** | Hard | Frequency Map | `Dictionary` | **How:** Expand until valid, shrink to minimize. **Why:** Finds tightest bound. **When:** Dependency resolution. |
| 25 | **Sliding Window Maximum** | Hard | Deque | `LinkedList<int>` | **How:** Store indices, remove smaller from back. **Why:** O(N) vs O(NK) heap. **When:** Stream max monitoring. |
| 26 | **Longest Continuous Increasing Subsequence** | Easy | Counter | `int count` | **How:** Reset count if `nums[i] <= nums[i-1]`. **Why:** Simple state tracking. **When:** Trend analysis. |
| 27 | **Find All Anagrams in a String** | Medium | Frequency Map | `List<int>` | **How:** Slide window, check map equality. **Why:** Reuses Category 3 logic. **When:** Security signature detection. |
| 28 | **Minimum Size Subarray Sum** | Medium | Two Pointers | `int sum` | **How:** Same as #20 (Duplicate in patterns). **Why:** Reinforces window shrinking. **When:** Load balancing thresholds. |
| 29 | **Fruit Into Baskets** | Medium | HashMap | `Dictionary` | **How:** Max 2 keys in map. **Why:** Constraint based window. **When:** Resource limitation. |
| 30 | **Count Number of Nice Subarrays** | Medium | Prefix Sum | `Dictionary` | **How:** Count odd numbers, use prefix sum logic. **Why:** Transforms to Subarray Sum Equals K. **When:** Parity checks. |

#### 🟣 Category 4: Stack (LIFO Operations)
*Focus: Parsing, Undo operations, Monotonic trends.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 31 | **Valid Parentheses** | Easy | Stack | `Stack<char>` | **How:** Push open, pop/close on match. **Why:** Ensures nesting order. **When:** Code parsing/JSON. |
| 32 | **Min Stack** | Medium | Dual Stack | `Stack<int>` | **How:** Second stack tracks min at each level. **Why:** O(1) retrieval of min. **When:** Monitoring lows. |
| 33 | **Evaluate Reverse Polish Notation** | Medium | Stack | `Stack<int>` | **How:** Push nums, pop 2 on operator, push result. **Why:** Handles order of operations. **When:** Calculator/Expression engines. |
| 34 | **Generate Parentheses** | Medium | Backtracking | `StringBuilder` | **How:** Recurse, track open/close count. **Why:** Ensures validity during generation. **When:** Template generation. |
| 35 | **Daily Temperatures** | Medium | Monotonic Stack | `Stack<int>` (indices) | **How:** Store indices, pop if current > top. **Why:** Finds next greater element efficiently. **When:** Alert triggers. |
| 36 | **Car Fleet** | Medium | Stack/Sort | `Array.Sort` | **How:** Sort by position, check time to target. **Why:** Faster cars blocked by slower. **When:** Traffic/Queue simulation. |
| 37 | **Largest Rectangle in Histogram** | Hard | Monotonic Stack | `Stack<int>` | **How:** Pop when height decreases, calc area. **Why:** Finds max bound for each bar. **When:** Skyline analysis. |
| 38 | **Maximal Rectangle** | Hard | Histogram + Stack | `int[][]` | **How:** Treat each row as histogram. **Why:** Reduces 2D problem to 1D. **When:** Image processing. |
| 39 | **Remove K Digits** | Medium | Monotonic Stack | `StringBuilder` | **How:** Pop if current digit < top. **Why:** Ensures smallest lexicographical number. **When:** Data compression. |
| 40 | **Asteroid Collision** | Medium | Stack | `Stack<int>` | **How:** Push positive, resolve negative against top. **Why:** Simulates physical collision. **When:** Event processing. |

#### 🔴 Category 5: Binary Search (Search Space)
*Focus: Sorted data, O(log N) reduction.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 41 | **Binary Search** | Easy | Standard | `int left, right` | **How:** `mid = left + (right-left)/2`. **Why:** Halves search space every step. **When:** Sorted lookup. |
| 42 | **Search a 2D Matrix** | Medium | Virtual 1D | `rows, cols` | **How:** Treat 2D as 1D sorted array. **Why:** Reuses standard BS logic. **When:** Database indexing. |
| 43 | **Koko Eating Bananas** | Medium | Search Answer | `Math.Ceil` | **How:** BS on possible speed values. **Why:** Monotonic function (faster speed = less time). **When:** Resource sizing. |
| 44 | **Find Minimum in Rotated Sorted** | Medium | Modified BS | `nums[mid] > nums[right]` | **How:** Compare mid to right to find pivot. **Why:** Handles rotated offset. **When:** Log file rotation. |
| 45 | **Search in Rotated Sorted** | Medium | Modified BS | `if (nums[left] <= nums[mid])` | **How:** Determine which side is sorted. **Why:** Narrows down valid range. **When:** Versioned data. |
| 46 | **Time Based Key-Value Store** | Medium | BS on List | `List<ValueTime>` | **How:** Store list of values, BS on timestamp. **Why:** Retrieves historical state. **When:** Configuration history. |
| 47 | **Median of Two Sorted Arrays** | Hard | Partitioning | `int partitionX` | **How:** Partition arrays so left == right. **Why:** O(log(min(m,n))). **When:** Statistical aggregation. |
| 48 | **Find Peak Element** | Medium | Gradient BS | `nums[mid] < nums[mid+1]` | **How:** Move towards higher neighbor. **Why:** Guarantees a peak exists. **When:** Optimization landscapes. |
| 49 | **Find First/Last Position** | Medium | Modified BS | `findBound()` | **How:** Run BS twice (once for left, once for right). **Why:** Handles duplicates. **When:** Range queries. |
| 50 | **Capacity To Ship Packages** | Medium | Search Answer | `int sum` | **How:** BS on capacity (min=max weight, max=total). **Why:** Monotonic constraint. **When:** Batch processing limits. |

#### 🟤 Category 6: Linked List (Pointer Manipulation)
*Focus: Memory references, in-place changes.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 51 | **Reverse Linked List** | Easy | Pointers | `ListNode prev` | **How:** Iterate, flip `next` to `prev`. **Why:** O(1) space, no recursion. **When:** Stack implementation. |
| 52 | **Merge Two Sorted Lists** | Easy | Dummy Head | `ListNode dummy` | **How:** Compare heads, attach smaller. **Why:** Simplifies edge cases. **When:** Stream merging. |
| 53 | **Reorder List** | Medium | Fast/Slow | `slow, fast` | **How:** Find mid, reverse second, merge. **Why:** In-place reordering. **When:** Data shuffling. |
| 54 | **Remove Nth Node From End** | Medium | Two Pointers | `fast, slow` | **How:** Gap `fast` by N, move together. **Why:** Single pass to find end-N. **When:** Log trimming. |
| 55 | **Copy List with Random Pointer** | Medium | HashMap | `Dictionary<Node, Node>` | **How:** Map old nodes to new nodes. **Why:** Handles random references. **When:** Deep cloning. |
| 56 | **Add Two Numbers** | Medium | Math | `int carry` | **How:** Simulate column addition. **Why:** Handles arbitrary precision. **When:** Financial calc. |
| 57 | **Linked List Cycle** | Easy | Floyd's Algo | `slow, fast` | **How:** Fast moves 2x, slow 1x. **Why:** They meet if cycle exists. **When:** Deadlock detection. |
| 58 | **Find Duplicate Number** | Medium | Floyd's Algo | `nums` as List | **How:** Treat array as linked list. **Why:** O(1) space, no modification. **When:** Integrity check. |
| 59 | **LRU Cache** | Medium | Dict + List | `Dictionary + LinkedList` | **How:** Map for O(1), List for order. **Why:** Combines lookup + recency. **When:** Memory caching. |
| 60 | **Reverse Nodes in k-Group** | Hard | Recursion | `ListNode next` | **How:** Reverse k, recurse on rest. **Why:** Batch processing. **When:** Packet chunking. |

#### 🟡 Category 7: Trees (Hierarchical Data)
*Focus: Recursion, DFS/BFS, Structure.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 61 | **Invert Binary Tree** | Easy | Recursion | `TreeNode` | **How:** Swap left/right children. **Why:** Simple structural change. **When:** UI mirroring. |
| 62 | **Maximum Depth of Binary Tree** | Easy | DFS | `Math.Max` | **How:** 1 + max(left, right). **Why:** Defines tree height. **When:** Balance checks. |
| 63 | **Same Tree** | Easy | DFS | `if (p.val != q.val)` | **How:** Compare value, recurse left/right. **Why:** Structural equality. **When:** Config diffing. |
| 64 | **Subtree of Another Tree** | Easy | DFS + Match | `IsSameTree()` | **How:** Check every node as root. **Why:** Pattern matching in hierarchy. **When:** Component reuse. |
| 65 | **Level Order Traversal** | Medium | BFS | `Queue<TreeNode>` | **How:** Queue per level. **Why:** Row-by-row processing. **When:** Org charts. |
| 66 | **Validate BST** | Medium | Bounds | `min, max` | **How:** Pass valid range down recursion. **Why:** Local check isn't enough. **When:** Data integrity. |
| 67 | **Kth Smallest in BST** | Medium | Inorder | `int count` | **How:** Inorder traversal is sorted. **Why:** Leverages BST property. **When:** Ranking. |
| 68 | **Construct Binary Tree from Pre/Inorder** | Medium | Recursion | `int preIndex` | **How:** Preorder gives root, Inorder splits left/right. **Why:** Rebuilds structure. **When:** Serialization. |
| 69 | **Binary Tree Level Order Zigzag** | Medium | BFS + Flag | `LinkedList` | **How:** Reverse list on alternate levels. **Why:** Visual variation. **When:** UI display. |
| 70 | **Serialize/Deserialize Binary Tree** | Hard | String | `StringBuilder` | **How:** Preorder with null markers. **Why:** Persistence/Network transfer. **When:** Save/Load state. |

#### 🟢 Category 8: Heaps / Priority Queues (Ordering)
*Focus: Top K, Min/Max tracking, Scheduling.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 71 | **Kth Largest Element** | Medium | Min-Heap | `PriorityQueue` | **How:** Keep heap size K, pop min. **Why:** O(N log K). **When:** Leaderboards. |
| 72 | **Top K Frequent Elements** | Medium | Heap + Map | `Dictionary + PQ` | **How:** Count freq, heap by freq. **Why:** Efficient ranking. **When:** Analytics. |
| 73 | **Merge K Sorted Lists** | Hard | Min-Heap | `PriorityQueue` | **How:** Push head of all lists, pop min. **Why:** O(N log K) merge. **When:** Log aggregation. |
| 74 | **Find Median from Data Stream** | Hard | Two Heaps | `MaxHeap, MinHeap` | **How:** Split data into lower/higher halves. **Why:** O(1) median access. **When:** Real-time stats. |
| 75 | **Task Scheduler** | Medium | Max-Heap | `int[26]` | **How:** Process most frequent task first. **Why:** Minimizes idle time. **When:** CPU scheduling. |
| 76 | **Design Twitter** | Medium | Heap + List | `List<Tweet>` | **How:** Merge recent tweets from followees. **Why:** Feed generation. **When:** Social feeds. |
| 77 | **Cheapest Flights Within K Stops** | Medium | Bellman-Ford | `int[] prices` | **How:** Relax edges K times. **Why:** Handles stop constraint. **When:** Routing. |
| 78 | **Network Delay Time** | Medium | Dijkstra | `PriorityQueue` | **How:** Greedy shortest path. **Why:** Finds max latency to all nodes. **When:** Monitoring. |
| 79 | **Swim in Rising Water** | Hard | Dijkstra/BS | `PriorityQueue` | **How:** Min-heap on max elevation path. **Why:** Finds safest path. **When:** Risk analysis. |
| 80 | **Reorganize String** | Medium | Max-Heap | `PriorityQueue` | **How:** Place most freq char, skip 1. **Why:** Prevents adjacent duplicates. **When:** Data encoding. |

#### 🔵 Category 9: Graphs (Connectivity)
*Focus: Nodes, Edges, Traversal, Dependencies.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 81 | **Number of Islands** | Medium | DFS/BFS | `bool[][] visited` | **How:** Sink island on visit. **Why:** Counts connected components. **When:** Cluster detection. |
| 82 | **Clone Graph** | Medium | DFS + Map | `Dictionary<Node, Node>` | **How:** Map old to new during traversal. **Why:** Handles cycles. **When:** Deep copying objects. |
| 83 | **Max Area of Island** | Medium | DFS | `int area` | **How:** Sum cells during DFS. **Why:** Measures component size. **When:** Resource sizing. |
| 84 | **Pacific Atlantic Water Flow** | Medium | DFS from Edge | `bool[][]` | **How:** Flow upwards from ocean. **Why:** Reverse thinking simplifies logic. **When:** Drainage analysis. |
| 85 | **Surrounded Regions** | Medium | DFS | `border` mark | **How:** Mark non-surrounded, flip rest. **Why:** Identifies safe zones. **When:** Game logic (Go). |
| 86 | **Course Schedule (Topological Sort)** | Medium | Kahn's Algo | `int[] indegree` | **How:** Process 0 indegree nodes. **Why:** Detects cycles/dependencies. **When:** Build pipelines. |
| 87 | **Course Schedule II** | Medium | Topo Sort | `List<int>` | **How:** Return order of processing. **Why:** Execution plan. **When:** Task orchestration. |
| 88 | **Graph Valid Tree** | Medium | Union Find | `int[] parent` | **How:** Check edges = n-1 and connected. **Why:** Defines tree structure. **When:** Network topology. |
| 89 | **Number of Connected Components** | Medium | Union Find | `int count` | **How:** Union sets, decrement count. **Why:** Tracks grouping. **When:** Microservice clusters. |
| 90 | **Redundant Connection** | Medium | Union Find | `Find()` | **How:** Return edge that connects existing set. **Why:** Identifies cycle source. **When:** Debugging loops. |

#### 🟣 Category 10: Dynamic Programming (Optimization)
*Focus: Breaking problems into subproblems, Memoization.*

| # | Problem | Diff | Pattern | C# Key Structure | Solution Logic (How/Why) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| 91 | **Climbing Stairs** | Easy | 1D DP | `int[] dp` | **How:** `dp[i] = dp[i-1] + dp[i-2]`. **Why:** Fibonacci pattern. **When:** Step combinations. |
| 92 | **Min Cost Climbing Stairs** | Easy | 1D DP | `Math.Min` | **How:** Min cost to reach step i. **Why:** Path optimization. **When:** Cost routing. |
| 93 | **House Robber** | Medium | 1D DP | `rob, skip` | **How:** Max(rob current + skip prev, skip current). **Why:** Adjacency constraint. **When:** Resource selection. |
| 94 | **House Robber II** | Medium | 1D DP | `RobRange()` | **How:** Run Robber twice (exclude first/last). **Why:** Handles circular constraint. **When:** Cycle constraints. |
| 95 | **Longest Palindromic Substring** | Medium | 2D DP | `bool[,]` | **How:** Expand around center or DP table. **Why:** Checks sub-string validity. **When:** Data validation. |
| 96 | **Coin Change** | Medium | Unbounded Knapsack | `int[] dp` | **How:** `dp[i] = min(dp[i-coin] + 1)`. **Why:** Min items for sum. **When:** Change making/Optimization. |
| 97 | **Longest Increasing Subsequence** | Medium | DP + Binary Search | `int[] tails` | **How:** Maintain smallest tail for length i. **Why:** O(N log N) optimization. **When:** Trend detection. |
| 98 | **Longest Common Subsequence** | Medium | 2D DP | `int[,]` | **How:** Match chars, diag + 1. **Why:** Measures similarity. **When:** Diff tools (Git). |
| 99 | **Word Break** | Medium | DP + Set | `HashSet` | **How:** `dp[i]` true if prefix valid. **Why:** Segmentation validity. **When:** Parsing. |
| 100| **Partition Equal Subset Sum** | Medium | 0/1 Knapsack | `bool[]` | **How:** Can we make sum/2? **Why:** Balancing loads. **When:** Load balancing. |

---

### Part 3: How to Practice (Sustainable Pace)

Do not try to solve all 100 in a week. Use the **Pattern-Based Approach**.

#### 📅 The Weekend Routine (2 Hours)
1.  **Pick One Pattern:** (e.g., Sliding Window).
2.  **Learn the Template:** Understand the generic code structure for that pattern.
3.  **Solve 3 Problems:**
    *   1 Easy (Warm-up)
    *   1 Medium (Core)
    *   1 Hard (Stretch)
4.  **Review:** Compare your C# solution with top-voted solutions. Look for LINQ usage vs. loops, memory allocations, etc.

#### 🛠️ C# Specific Tips for DSA Interviews
1.  **Use `HashSet<T>` and `Dictionary<TKey, TValue>`:** Know their O(1) complexity.
2.  **Avoid `LINQ` in Hot Paths:** Interviewers want to see algorithmic logic, not `list.Where()`. Use loops for DSA problems.
3.  **Know `PriorityQueue<TElement, TPriority>`:** Introduced in .NET 6, essential for Heap problems.
4.  **String Manipulation:** Use `StringBuilder` for mutable strings to avoid memory churn.
5.  **Null Checks:** C# is verbose. Always check `if (node == null)` explicitly.

#### 🧠 The "Elite" Mindset for DSA
*   **Don't Memorize Code:** Memorize the **Pattern**. (e.g., "This looks like a Sliding Window problem").
*   **Talk While Coding:** Explain your thought process. "I'm using a HashMap here to reduce lookup time..."
*   **Clarify Constraints:** Ask "Can the array be negative?" "Does it fit in memory?" before coding.
*   **Test Cases:** Run through a small example manually before submitting.

### 🚀 Final Recommendation
Start with **Category 1 (Arrays & Hashing)** this weekend. Solve **Two Sum**, **Contains Duplicate**, and **Valid Anagram**. Master the `Dictionary` and `HashSet` in C#. Once comfortable, move to **Two Pointers**.

This list covers the **canonical 100** that appear in 95% of interviews for 0–5 years experience. Master these, and you will pass the technical screen.
