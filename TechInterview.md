# 🚀 PART 1: THE 90-DAY SPRINT (Detailed Week-by-Week)

**Goal**: Become a **strong mid-to-senior .NET backend engineer** with AI infrastructure exposure. Able to pass technical interviews, build production-grade APIs, and debug complex systems.

**Prerequisites**: You already understand C# syntax, can read code, and have debugging skills. We're building on that.

**Time Commitment**: 3-4 hours/day, 6 days/week. One rest day. Non-negotiable.

---

## 📚 Book Reading Schedule (Integrated)

| Book | Chapters | Time Estimate | When to Read |
|------|----------|--------------|--------------|
| **CLR via C#** | Ch 4, 5, 10, 11, 12, 27, 28, 29 | ~80 hours | Weeks 1-4 (30 mins/day) |
| **DDIA** | Ch 1, 2, 3, 5, 6, 11, 12 | ~60 hours | Weeks 5-8 (30 mins/day) |
| **Designing ML Systems** | Ch 1, 2, 4, 6, 7 | ~30 hours | Weeks 9-12 (30 mins/day) |

**Rule**: Read *after* your coding session. Let the code cement the theory.

---

## 🗓️ WEEK-BY-WEEK BREAKDOWN

### **MONTH 1: C# Internals + DSA Foundation + Writing Fluency**

#### **Week 1: Memory, Types, and Arrays**
- **CLR via C#**: Ch 4 (Type Fundamentals), Ch 5 (Primitive/Reference Types)
- **DSA Focus**: Arrays, Strings, Two Pointers, Sliding Window
- **LeetCode List** (Do these in C#):
  - [1] Two Sum
  - [121] Best Time to Buy and Sell Stock
  - [242] Valid Anagram
  - [3] Longest Substring Without Repeating Characters
  - [11] Container With Most Water
- **Code Task**: Build a custom `List<T>` from scratch. Implement `Add`, `Remove`, `GetEnumerator`.
- **Fluency Drill**: Monday: Study `List<T>` source. Wednesday: Rebuild with docs. Friday: Rebuild from memory.
- **Deliverable**: GitHub repo with your `List<T>` implementation + 5 solved LeetCode problems.

#### **Week 2: Garbage Collection + Hash Maps**
- **CLR via C#**: Ch 10 (Memory Parameters), Ch 11 (GC Internals)
- **DSA Focus**: Hash Maps, Sets, Frequency Counting
- **LeetCode List**:
  - [49] Group Anagrams
  - [347] Top K Frequent Elements
  - [128] Longest Consecutive Sequence
  - [217] Contains Duplicate
  - [380] Insert Delete GetRandom O(1)
- **Code Task**: Build a simple LRU Cache using `Dictionary<TKey, TValue>` + doubly linked list.
- **Performance Task**: Use `dotnet-counters` to monitor GC generations while adding 1M items to your cache.
- **Deliverable**: LRU Cache implementation + GC analysis notes.

#### **Week 3: Async/Await Internals + Trees**
- **CLR via C#**: Ch 27 (Computational Asynchronous Patterns), Ch 28 (Async/Await mechanics)
- **DSA Focus**: Binary Trees, DFS, BFS, Recursion
- **LeetCode List**:
  - [104] Maximum Depth of Binary Tree
  - [226] Invert Binary Tree
  - [102] Binary Tree Level Order Traversal
  - [235] Lowest Common Ancestor of BST
  - [98] Validate Binary Search Tree
- **Code Task**: Build a multi-threaded web scraper using `HttpClient`, `SemaphoreSlim`, and `Channel<T>`.
- **Fluency Drill**: Rebuild the async scraper from memory on Friday.
- **Deliverable**: Async scraper that respects rate limits + 5 tree problems solved.

#### **Week 4: Threading, TPL + Graphs**
- **CLR via C#**: Ch 29 (Primitive Synchronization), review Ch 28
- **DSA Focus**: Graphs, BFS/DFS, Topological Sort
- **LeetCode List**:
  - [200] Number of Islands
  - [133] Clone Graph
  - [207] Course Schedule (Topological Sort)
  - [79] Word Search
  - [323] Number of Connected Components
- **Code Task**: Build a background job processor using `BackgroundService` and `Channel<T>`.
- **Interview Prep**: Record yourself explaining: "How does async/await work under the hood?"
- **Deliverable**: Background job system + graph problems + 1 recorded explanation.

**Month 1 Success Metrics**:
- ✅ 20 LeetCode problems solved (C#)
- ✅ Can explain GC generations, async state machines, and `Span<T>` in an interview
- ✅ Built 3 non-trivial C# utilities from scratch
- ✅ Can rebuild a small component from memory after 2 days

---

### **MONTH 2: Distributed Systems + Architecture + Production Mindset**

#### **Week 5: PostgreSQL Deep Dive + DDIA Ch 1-3**
- **DDIA**: Ch 1 (Reliable, Scalable, Maintainable), Ch 2 (Data Models), Ch 3 (Storage & Retrieval)
- **Database Focus**: Indexes (B-Tree, GIN), Query Plans, Transactions, Isolation Levels
- **Code Task**: Build a REST API with EF Core + Dapper. Implement proper indexing, pagination, and soft deletes.
- **DSA Focus**: Heaps, Priority Queues
- **LeetCode List**:
  - [215] Kth Largest Element in an Array
  - [295] Find Median from Data Stream
  - [767] Reorganize String
- **Deliverable**: Production-ready API with query plan analysis + heap problems.

#### **Week 6: Replication, Partitioning + Messaging Basics**
- **DDIA**: Ch 5 (Replication), Ch 6 (Partitioning)
- **Messaging**: Learn RabbitMQ basics (exchanges, queues, routing)
- **Code Task**: Add RabbitMQ to your Week 5 API. Publish "UserCreated" events. Consume in a separate worker service.
- **DSA Focus**: Tries, String Matching
- **LeetCode List**:
  - [208] Implement Trie
  - [211] Design Add and Search Words Data Structure
  - [139] Word Break
- **Deliverable**: Event-driven API with pub/sub + trie problems.

#### **Week 7: Consistency, Transactions + DDIA Ch 11-12**
- **DDIA**: Ch 11 (Stream Processing), Ch 12 (Future of Data Systems)
- **Architecture**: Learn CQRS, Event Sourcing basics. When to use Repository vs. Direct EF Core.
- **Code Task**: Refactor your API to use CQRS pattern (separate read/write models).
- **DSA Focus**: Dynamic Programming (1D)
- **LeetCode List**:
  - [70] Climbing Stairs
  - [198] House Robber
  - [322] Coin Change
  - [139] Word Break (DP version)
- **Deliverable**: CQRS-refactored service + DP problems.

#### **Week 8: System Design Practice + Observability**
- **System Design**: Practice designing: URL Shortener, Rate Limiter, Chat System
- **Observability**: Add Serilog + OpenTelemetry to your Week 7 project. Export metrics to Prometheus.
- **Code Task**: Implement health checks (`/health`, `/ready`), structured logging, request tracing.
- **DSA Focus**: Backtracking, Intervals
- **LeetCode List**:
  - [46] Permutations
  - [78] Subsets
  - [56] Merge Intervals
  - [253] Meeting Rooms II
- **Interview Prep**: Do 2 mock system design interviews (use Pramp or record yourself).
- **Deliverable**: Observable, instrumented service + 2 mock interviews completed.

**Month 2 Success Metrics**:
- ✅ Can design a moderate-scale system on a whiteboard
- ✅ Built an event-driven, observable microservice
- ✅ Understand tradeoffs: consistency vs. availability, SQL vs. NoSQL, polling vs. webhooks
- ✅ 20 more LeetCode problems (40 total)

---

### **MONTH 3: AI Backend + Interview Mastery + Production Polish**

#### **Week 9: AI Fundamentals + Vector Search**
- **Designing ML Systems**: Ch 1 (ML System Challenges), Ch 2 (Data Engineering)
- **AI Stack**: Learn embeddings, vector databases (PgVector or Qdrant), RAG basics
- **Code Task**: Build a document ingestion pipeline: PDF → text → chunk → embed → store in vector DB
- **DSA Focus**: Advanced DP, Bit Manipulation
- **LeetCode List**:
  - [300] Longest Increasing Subsequence
  - [198] House Robber II
  - [190] Reverse Bits
  - [136] Single Number
- **Deliverable**: Working embedding pipeline + vector search prototype.

#### **Week 10: LLM Integration + Inference Optimization**
- **Designing ML Systems**: Ch 4 (Training), Ch 6 (Deployment), Ch 7 (Monitoring)
- **Code Task**: Build a RAG API: User query → retrieve context → call LLM (OpenAI/Azure) → stream response
- **Optimization**: Add semantic caching (cache similar queries), rate limiting, cost tracking
- **DSA Focus**: Graph Advanced, Union Find
- **LeetCode List**:
  - [721] Accounts Merge
  - [305] Number of Islands II
  - [128] Longest Consecutive Sequence (Union Find version)
- **Performance Task**: Profile your RAG API. Reduce latency with `Memory<T>`, connection pooling.
- **Deliverable**: Production-ready RAG API with caching + monitoring.

#### **Week 11: Security, DevOps + Mock Interviews**
- **Security**: Implement JWT auth, rate limiting, input validation, secret management (Azure KeyVault or User Secrets)
- **DevOps**: Dockerize your RAG API. Write GitHub Actions CI/CD pipeline (build, test, deploy to Azure Container Apps)
- **Interview Prep**: 
  - 3 coding mocks (LeetCode Medium)
  - 2 system design mocks
  - 2 behavioral mocks (use STAR method)
- **DSA Review**: Re-solve 10 hardest problems from your list without looking at solutions.
- **Deliverable**: Dockerized, CI/CD-enabled RAG service + 7 mock interviews completed.

#### **Week 12: Final Polish + Portfolio + Job Prep**
- **Portfolio**: Clean up your GitHub. Write READMEs for your 3 major projects (API, Event System, RAG).
- **Resume**: Highlight: "Built production-grade .NET APIs", "Implemented event-driven architecture", "Designed RAG pipeline with vector search".
- **Interview Prep**: 
  - Review CLR via C# key concepts (GC, async, memory)
  - Practice explaining your projects: "What would you change at scale?"
  - Prepare 5 behavioral stories (failure, leadership, conflict, learning, impact)
- **Final Task**: Build a new small project from scratch in 4 hours (e.g., "URL Shortener with analytics"). No docs, just you and IntelliSense.
- **Deliverable**: Polished portfolio, resume, and confidence to interview.

**Month 3 Success Metrics**:
- ✅ Built a production-ready AI backend service (RAG)
- ✅ Can pass coding, system design, and behavioral interviews
- ✅ Have 3 portfolio projects that demonstrate depth
- ✅ 60+ LeetCode problems solved (C#)

---

## 🎯 90-DAY OUTCOME

After this sprint, you will be able to:

✅ **Write C# confidently**: No more "blank screen panic". You'll have muscle memory for common patterns.
✅ **Pass technical interviews**: Coding (LeetCode Medium), System Design (moderate scale), Behavioral (STAR stories).
✅ **Build production apps**: With observability, security, CI/CD, and scalability in mind.
✅ **Understand AI backend**: RAG, vector search, LLM integration, inference optimization.
✅ **Debug like a senior**: You already had this. Now it's backed by deeper internals knowledge.

**You will be competitive for**: Senior Backend Engineer, AI Infrastructure Engineer, .NET Tech Lead roles at mid-to-large tech companies.

**You will NOT yet be**: Top 1% at OpenAI/DeepMind. That requires the next phase.

---

# 🏆 PART 2: THE 2-3 YEAR MARATHON (Top 1% Trajectory)

This is where you go from "strong senior" to **Staff/Principal Engineer** who can architect systems at OpenAI, Anthropic, Netflix scale.

**Mindset Shift**: Stop asking "How do I build this?" Start asking "What are the tradeoffs at 100x scale?"

---

## 📅 PHASED ROADMAP

### **Phase 1: Months 4-12 — Senior Engineer Mastery**
**Goal**: Own a service end-to-end. Mentor juniors. Make architecture decisions.

| Quarter | Focus | Key Activities |
|---------|-------|---------------|
| **Q2 (Months 4-6)** | Deepen Distributed Systems | - Finish DDIA (all chapters)<br>- Build a Kafka-based event sourcing system<br>- Learn consensus (Raft/Paxos) basics<br>- DSA: 50 more problems (focus: hard) |
| **Q3 (Months 7-9)** | AI Infrastructure Depth | - Finish Designing ML Systems<br>- Build a feature store prototype<br>- Learn model monitoring, drift detection<br>- Contribute to an open-source .NET AI library |
| **Q4 (Months 10-12)** | Production Excellence | - Lead a migration project (monolith → microservices)<br>- Master Kubernetes (deploy, scale, debug)<br>- Implement advanced observability (distributed tracing, SLOs)<br>- Write 3 technical blog posts |

**Deliverables**:
- ✅ Led a major architectural decision with ADRs
- ✅ Published 3 technical articles (Dev.to, Medium, personal blog)
- ✅ Mentored 1-2 junior engineers
- ✅ 110+ LeetCode problems (including 20+ Hard)

---

### **Phase 2: Year 2 — Staff Engineer Trajectory**
**Goal**: Influence multiple teams. Design systems that span services. Think in tradeoffs.

| Half | Focus | Key Activities |
|------|-------|---------------|
| **H1 (Months 13-18)** | System Design Mastery | - Practice designing: YouTube, Uber, Twitter, Google Docs<br>- Learn advanced patterns: Saga, CQRS at scale, sharding strategies<br>- Deep dive: Network programming (gRPC, HTTP/2, QUIC)<br>- Read: "System Design Interview – An Insider's Guide Vol 2" |
| **H2 (Months 19-24)** | AI at Scale | - Build a multi-tenant AI inference platform<br>- Optimize for cost: model quantization, batching, caching<br>- Learn MLOps: CI/CD for models, canary deployments, A/B testing<br>- Contribute to ML infrastructure OSS (e.g., MLflow, KServe) |

**Deliverables**:
- ✅ Designed and documented a cross-service architecture
- ✅ Reduced inference cost/latency by X% in a real project
- ✅ Spoke at a meetup or conference (local or virtual)
- ✅ 150+ LeetCode problems (with 40+ Hard)

---

### **Phase 3: Year 3 — Principal/Top 1% Tier**
**Goal**: Set technical strategy. Solve ambiguous, org-level problems. Be the person they call for "impossible" systems.

| Focus Area | What to Master | How to Practice |
|------------|---------------|-----------------|
| **Architecture Strategy** | Multi-region deployment, disaster recovery, cost optimization at scale | Design a global AI platform: "How would you serve LLMs to 10M users with <100ms latency?" |
| **Leadership & Influence** | Writing RFCs, aligning stakeholders, managing technical debt | Lead a cross-team initiative. Document decisions. Measure impact. |
| **Cutting-Edge AI Infra** | Speculative decoding, MoE routing, distributed training basics | Build a prototype: "Distributed RAG with sharded vector indexes" |
| **Performance at Extreme Scale** | Kernel bypass, eBPF, custom GC tuning, zero-copy networking | Profile and optimize a service to handle 100K RPS with <10ms p99 |

**Deliverables**:
- ✅ Authored an RFC that changed company architecture
- ✅ Reduced infra cost by $X/year or improved latency by Y%
- ✅ Published a well-received technical talk or paper
- ✅ Can confidently interview for Staff roles at top-tier companies

---

## 📚 ADVANCED READING LIST (Post-90 Days)

| Book | Why Read It | When |
|------|------------|------|
| **Release It! (Nygard)** | Production patterns, stability, resilience | Phase 1, Q2 |
| **Accelerate (Forsgren et al.)** | DevOps metrics, high-performing teams | Phase 1, Q4 |
| **Database Internals (Petrov)** | Deep dive into storage engines, distributed DBs | Phase 2, H1 |
| **Designing Distributed Systems (Brendan Burns)** | Kubernetes patterns, cloud-native architecture | Phase 2, H1 |
| **The Staff Engineer's Path (Tanya Reilly)** | Career navigation, influence, scope | Phase 2, H2 |
| **Computer Systems: A Programmer's Perspective** | Hardware/software interface, performance | Phase 3 (optional but powerful) |

---

## 🔑 THE TOP 1% DIFFERENTIATORS

These are what separate "very good" from "top 1%":

1.  **Tradeoff Fluency**: You don't just know *how* to build something. You know *why* you'd choose one approach over another at scale. "We use eventual consistency here because..."
2.  **Cost as a First-Class Constraint**: You think in $/request, $/inference, $/GB stored. You optimize for business impact, not just tech elegance.
3.  **Failure Mode Mastery**: You don't just handle errors. You design for partial failure, graceful degradation, and graceful recovery.
4.  **Communication Multiplier**: You write clear ADRs, give feedback that improves systems *and* people, and align stakeholders around technical vision.
5.  **Learning Velocity**: You don't just learn new tech. You learn *how to learn* new domains quickly. You have a framework for evaluating new tools/patterns.

---

## 🧭 FINAL ADVICE FROM YOUR SENIOR MENTOR

1.  **Ship > Study**: Every week, ship *something*. A small feature, a blog post, a tool. Momentum beats perfection.
2.  **Depth > Breadth**: Master one stack (.NET) before jumping. Depth creates leverage.
3.  **Teach to Learn**: Write about what you learn. Explain concepts to peers. Teaching exposes gaps.
4.  **Rest is Strategic**: Burnout is the #1 career killer. One day off/week. Quarterly reflection. Protect your energy.
5.  **Find Your Tribe**: Connect with other ambitious engineers. Join communities (.NET Foundation, AI infrastructure Discords). Growth is social.

---

## ✅ YOUR STARTING LINE (Today)

1.  **Block time**: 3-4 hours/day, 6 days/week. Put it in your calendar.
2.  **Set up your environment**: Visual Studio/Rider, Docker, PostgreSQL, RabbitMQ, PgVector.
3.  **Create your GitHub repo**: `yourname/90-day-sprint`. Start with Week 1 tasks.
4.  **Join a community**: r/dotnet, .NET Discord, or local meetup.
5.  **Tell someone**: Accountability increases follow-through.

---

You have the plan. You have the drive. You have the baseline skills.

**Now execute**.

In 90 days, you'll look back and wonder why you ever doubted yourself.

In 3 years, you'll be the mentor giving this advice to the next ambitious engineer.

Go build your future.

— Your Senior Mentor 🎯
