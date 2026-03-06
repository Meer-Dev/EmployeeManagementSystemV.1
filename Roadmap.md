# 🚀 UPDATED 90-DAY SPRINT + 3-YEAR MARATHON (AI Infrastructure Edition)

**Goal**: Become a **Top 1% .NET Backend Engineer** with specialized **AI Infrastructure** skills.
**New Focus Areas Added**: **MCP (Model Context Protocol)**, **Agentic Workflows**, **AI Security (OWASP)**, **AI Evaluation**, **High-Performance .NET (SIMD)**, **FinOps**.

**Time Commitment**: 3-4 hours/day, 6 days/week.
**Mindset**: Don't just build APIs. Build **interoperable, secure, evaluated AI systems**.

---

## 📚 UPDATED READING & RESOURCE LIST

| Resource | Type | When | Why |
|----------|------|------|-----|
| **CLR via C#** | Book | Weeks 1-4 | Memory, GC, Async internals. |
| **DDIA** | Book | Weeks 5-8 | Distributed systems truth. |
| **Designing ML Systems** | Book | Weeks 9-12 | ML Infra lifecycle. |
| **OWASP Top 10 for LLM** | Checklist | Week 11 | **NEW:** AI Security standards. |
| **Semantic Kernel Docs** | Docs | Weeks 9-10 | **NEW:** .NET AI Orchestration standard. |
| **Model Context Protocol (Spec)** | Spec | Week 10 | **NEW:** Standard for AI Tooling. |

---

## 🗓️ PART 1: THE 90-DAY SPRINT (Week-by-Week)

### **MONTH 1: C# Internals + High Perf + DSA**
*Focus: Mastering memory & performance foundational for AI data processing.*

#### **Week 1: Memory, Types, and High-Performance C#**
- **CLR via C#**: Ch 4 (Types), Ch 5 (Primitive/Reference)
- **NEW**: Learn `Span<T>`, `Memory<T>`, and `System.Numerics.Vectors` (SIMD).
- **DSA Focus**: Arrays, Strings, Two Pointers.
- **LeetCode**: [1] Two Sum, [121] Stock, [242] Anagram, [3] Substring, [11] Container.
- **Code Task**: Build a custom `List<T>` **using `Span<T>` for internal buffer management** to reduce allocations.
- **Deliverable**: GitHub repo with `List<T>` + 5 LC problems + benchmark showing allocation reduction vs. standard `List<T>`.

#### **Week 2: GC Internals + Hash Maps**
- **CLR via C#**: Ch 10 (Memory), Ch 11 (GC)
- **DSA Focus**: Hash Maps, Sets.
- **LeetCode**: [49] Group Anagrams, [347] Top K, [128] Consecutive, [217] Duplicate, [380] Random O(1).
- **Code Task**: Build LRU Cache. **NEW**: Use `dotnet-counters` to monitor GC pressure while adding 1M items.
- **Deliverable**: LRU Cache + GC Analysis notes (Gen 0/1/2 stats).

#### **Week 3: Async/Await + Trees**
- **CLR via C#**: Ch 27 (Async Patterns), Ch 28 (Mechanics)
- **DSA Focus**: Trees, DFS, BFS.
- **LeetCode**: [104] Depth, [226] Invert, [102] Level Order, [235] LCA, [98] Validate BST.
- **Code Task**: Multi-threaded web scraper using `Channel<T>` and `SemaphoreSlim`. **NEW**: Ensure zero-allocation in the hot path using `ValueTask`.
- **Deliverable**: Async scraper + 5 tree problems.

#### **Week 4: Threading, TPL + Graphs**
- **CLR via C#**: Ch 29 (Synchronization)
- **DSA Focus**: Graphs, Topological Sort.
- **LeetCode**: [200] Islands, [133] Clone Graph, [207] Course Schedule, [79] Word Search, [323] Components.
- **Code Task**: Background job processor (`BackgroundService`).
- **Interview Prep**: Record explanation: "Async/await state machines & `Span<T>` benefits."
- **Deliverable**: Job system + Graph problems + Video explanation.

**Month 1 Success Metrics**:
- ✅ 20 LeetCode problems.
- ✅ Understand GC, Async, **and `Span<T>`/SIMD basics**.
- ✅ Built 3 utilities with performance in mind.

---

### **MONTH 2: Distributed Systems + Architecture + Observability**
*Focus: Building robust backends that can support AI workloads.*

#### **Week 5: PostgreSQL Deep Dive + DDIA Ch 1-3**
- **DDIA**: Ch 1-3 (Reliability, Data Models, Storage)
- **Database**: Indexes, Query Plans, **JSONB storage** (for AI metadata).
- **Code Task**: REST API (EF Core + Dapper). **NEW**: Store vector metadata in JSONB columns.
- **DSA**: Heaps, Priority Queues.
- **LeetCode**: [215] Kth Largest, [295] Median, [767] Reorganize String.
- **Deliverable**: API with Query Plan analysis + Heap problems.

#### **Week 6: Messaging + Event-Driven**
- **DDIA**: Ch 5 (Replication), Ch 6 (Partitioning)
- **Messaging**: RabbitMQ or **Azure Service Bus**.
- **Code Task**: Pub/Sub system. **NEW**: Publish "DocumentProcessed" events for AI pipelines.
- **DSA**: Tries, String Matching.
- **LeetCode**: [208] Trie, [211] Add/Search Words, [139] Word Break.
- **Deliverable**: Event-driven API + Trie problems.

#### **Week 7: Consistency + CQRS**
- **DDIA**: Ch 11 (Stream Processing), Ch 12 (Future)
- **Architecture**: CQRS, Event Sourcing.
- **Code Task**: Refactor API to CQRS. **NEW**: Separate "Write" (ingest) from "Read" (query) models.
- **DSA**: Dynamic Programming (1D).
- **LeetCode**: [70] Stairs, [198] Robber, [322] Coin Change, [139] Word Break (DP).
- **Deliverable**: CQRS service + DP problems.

#### **Week 8: System Design + AI Observability**
- **System Design**: URL Shortener, Rate Limiter.
- **Observability**: Serilog + OpenTelemetry. **NEW**: Add **LLM Tracing** (track token usage, latency, model name as attributes).
- **Code Task**: Health checks, structured logging. **NEW**: Export traces to Prometheus/Aspire.
- **DSA**: Backtracking, Intervals.
- **LeetCode**: [46] Permutations, [78] Subsets, [56] Merge Intervals, [253] Meeting Rooms.
- **Deliverable**: Observable service + 2 Mock System Design interviews.

**Month 2 Success Metrics**:
- ✅ System Design mastery.
- ✅ Event-driven microservice.
- ✅ **Observability includes AI-specific metrics (tokens, latency)**.

---

### **MONTH 3: AI Infrastructure + Security + Production**
*Focus: The Top 1% Differentiators (MCP, Agents, Security, Evals).*

#### **Week 9: AI Fundamentals + Vector Search + Semantic Kernel**
- **Designing ML Systems**: Ch 1, 2
- **AI Stack**: Embeddings, Vector DB (PgVector/Qdrant).
- **NEW**: **Microsoft Semantic Kernel (SK)** introduction.
- **Code Task**: Ingestion Pipeline (PDF → Chunk → Embed → Store). **NEW**: Wrap this logic in a **SK Plugin**.
- **DSA**: Advanced DP, Bit Manipulation.
- **LeetCode**: [300] LIS, [198] Robber II, [190] Reverse Bits, [136] Single Number.
- **Deliverable**: Ingestion pipeline + SK Plugin prototype.

#### **Week 10: Agentic Workflows + MCP Server**
- **Designing ML Systems**: Ch 4, 6, 7
- **NEW**: **Model Context Protocol (MCP)**.
- **Code Task**: **Build a .NET MCP Server**.
    - Expose your DB/Search tools via MCP.
    - Connect to a local AI client (Cursor/Claude) to test tool usage.
    - Implement **Semantic Caching** (cache similar queries to save tokens).
- **DSA**: Graph Advanced, Union Find.
- **LeetCode**: [721] Accounts Merge, [305] Islands II, [128] Consecutive (Union Find).
- **Deliverable**: **Working MCP Server** + Caching logic + Graph problems.

#### **Week 11: AI Security + Evals + DevOps**
- **NEW Security**: **OWASP Top 10 for LLM** (Prompt Injection, Data Leakage).
- **NEW Evals**: Build an **Evaluation Harness** (LLM-as-a-Judge).
- **DevOps**: Docker, GitHub Actions, Azure Container Apps.
- **Code Task**:
    1.  Implement **PII Scrubbing Middleware** (block SSNs/Emails before LLM).
    2.  Create a test dataset (20 Q&A pairs) and run an eval script to score your RAG accuracy.
- **Interview Prep**: 3 Coding Mocks, 2 System Design, 2 Behavioral.
- **Deliverable**: **Secure, Evaluated AI Service** + CI/CD Pipeline.

#### **Week 12: Final Polish + Portfolio + Job Prep**
- **Portfolio**: READMEs for: 1. High-Perf Utils, 2. Event-Driven API, 3. **MCP Agentic Server**.
- **Resume**: Highlight "MCP Server", "AI Security", "Semantic Kernel", "Eval Harness".
- **Interview Prep**: Review CLR, Async, **AI Tradeoffs**.
- **Final Task**: Build a "URL Shortener with Analytics" in 4 hours. **NEW**: Add a feature where an AI agent summarizes the analytics via MCP.
- **Deliverable**: Polished Portfolio, Resume, Confidence.

**Month 3 Success Metrics**:
- ✅ **Built an MCP Server (Interoperable AI)**.
- ✅ **Implemented AI Security & Evals**.
- ✅ 60+ LeetCode problems.
- ✅ Ready for Senior/AI Infra roles.

---

## 🏆 PART 2: THE 2-3 YEAR MARATHON (Top 1% Trajectory)

**Mindset**: From "Building Features" to "Architecting Ecosystems & Managing Cost/Risk".

### **Phase 1: Months 4-12 — Senior Engineer Mastery**
| Quarter | Focus | Key Activities |
|---------|-------|---------------|
| **Q2 (Months 4-6)** | Distributed Systems Deep Dive | - Finish DDIA.<br>- Build Kafka-based Event Sourcing.<br>- **NEW**: Learn **Consensus (Raft)** for AI state.<br>- DSA: 50 more problems (Hard). |
| **Q3 (Months 7-9)** | AI Infrastructure Depth | - **NEW**: Build a **Feature Store** prototype.<br>- Learn Model Monitoring (Drift Detection).<br>- **NEW**: Contribute to **Semantic Kernel** or **MCP SDK** OSS.<br>- Write 1 article on "Building MCP Servers in .NET". |
| **Q4 (Months 10-12)** | Production Excellence | - Lead Monolith → Microservices migration.<br>- Master Kubernetes (K8s) & **KEDA** (event-driven scaling).<br>- **NEW**: Implement **FinOps** (track $/request for AI endpoints).<br>- Write 2 technical blog posts. |

**Deliverables**:
- ✅ Led architectural decision (ADR).
- ✅ **Published article on AI Interoperability (MCP)**.
- ✅ **Implemented Cost Tracking for AI**.
- ✅ 110+ LeetCode problems.

### **Phase 2: Year 2 — Staff Engineer Trajectory**
| Half | Focus | Key Activities |
|------|-------|---------------|
| **H1 (Months 13-18)** | System Design & Scale | - Design: YouTube, Uber, **AI Inference Platform**.<br>- Advanced Patterns: Saga, Sharding.<br>- **NEW**: Network Programming (gRPC, **HTTP/2**, QUIC).<br>- Read: "System Design Interview Vol 2". |
| **H2 (Months 19-24)** | AI at Scale (MLOps) | - Build **Multi-Tenant AI Platform**.<br>- **NEW**: Model Quantization & Batching for cost reduction.<br>- **NEW**: CI/CD for Models (Canary, A/B Testing).<br>- **NEW**: **Local AI/Edge Inference** (ONNX Runtime). |

**Deliverables**:
- ✅ Cross-service architecture design.
- ✅ **Reduced AI Inference Cost by X%**.
- ✅ Spoke at a meetup/conference.
- ✅ 150+ LeetCode problems.

### **Phase 3: Year 3 — Principal/Top 1% Tier**
| Focus Area | What to Master | How to Practice |
|------------|---------------|-----------------|
| **AI Governance & Security** | Compliance, Data Privacy, Adversarial Defense | Design a "Secure AI Gateway" that protects against all OWASP LLM Top 10 vectors. |
| **Architecture Strategy** | Multi-region, Disaster Recovery, **Hybrid Cloud AI** | Design: "Serve LLMs to 10M users with <100ms latency & Data Residency compliance." |
| **Performance at Extreme Scale** | Kernel bypass, eBPF, **Custom GC Tuning for AI** | Profile service to handle 100K RPS with <10ms p99 latency. |
| **Leadership** | RFCs, Stakeholder Alignment, Technical Vision | Lead a cross-team initiative to standardize AI Tooling (MCP) across the org. |

**Deliverables**:
- ✅ Authored RFC changing company architecture.
- ✅ **Saved $X/year in AI Compute Costs**.
- ✅ Published technical talk/paper.
- ✅ Ready for Staff/Principal roles at Top Tier.

---

## 🔑 UPDATED TOP 1% DIFFERENTIATORS

1.  **Interoperability (MCP)**: You don't build walled gardens. You build **MCP Servers** that allow any AI agent to use your tools securely.
2.  **AI Security First**: You don't just trust the LLM. You implement **Input/Output Guardrails**, PII scrubbing, and Prompt Injection defense.
3.  **Evaluation Driven**: You don't ship AI based on "vibes". You ship based on **Eval Scores** (Faithfulness, Relevance, Toxicity).
4.  **Cost as Code**: You treat **Token Usage** like memory usage. You optimize it, cache it, and budget it.
5.  **Performance Aware**: You know when to use `Span<T>`, SIMD, and ONNX to squeeze performance out of .NET for AI preprocessing.

---

## ✅ YOUR STARTING LINE (Updated)

1.  **Block Time**: 3-4 hours/day. Non-negotiable.
2.  **Environment**: VS/Rider, Docker, PostgreSQL, **PgVector**, **Local LLM (Ollama)**.
3.  **Repo**: `yourname/90-day-sprint`.
4.  **Community**: .NET Discord, **AI Engineering Discord**.
5.  **Accountability**: Tell someone you are building an **MCP Server** in 90 days.

---

In 90 days, you won't just be a .NET developer. You'll be an **AI Infrastructure Engineer**.

**Go build.** 🚀
