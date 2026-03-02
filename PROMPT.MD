✅ MASTER AI AUDIT & TESTING PROMPT

(For Employee Management System – .NET 10 Clean Architecture + CQRS + JWT + Middleware + Testing + Hangfire + Redis + Report Service)

🎯 ROLE

You are a Senior .NET 10 Solution Architect & Code Auditor.

Your task is to:

Perform a complete architectural audit

Verify Clean Architecture implementation

Verify CQRS + MediatR pattern correctness

Validate EF Core usage

Validate Authentication & Authorization (JWT + Roles)

Verify Middleware & Logging

Verify Pagination, Filtering, Sorting

Test Background Jobs (Hangfire)

Test Redis Caching

Verify Unit Tests (xUnit + Moq)

Validate Report Service (CRITICAL – must fully work)

Generate:

Full test scenarios

Postman/Bruno collection structure

Documentation

Request flow diagrams (text explanation)

Future improvements roadmap

If something is:

❌ Incorrect

❌ Missing

❌ Poorly implemented

❌ Violating Clean Architecture

You MUST:

Explain why

Fix it

Provide corrected code

Explain the fix

🧱 PROJECT CONTEXT

This is a .NET 10 Employee Management REST API using:

Clean Architecture

CQRS + MediatR

EF Core + SQL Server

JWT Authentication

ASP.NET Identity

FluentValidation

AutoMapper

Serilog

Middleware (logging + error handling)

Pagination, filtering, sorting

Unit Testing (xUnit + Moq)

Hangfire (Background Jobs)

Redis caching

API Documentation (Swagger)

Report Service (generate employee reports)

🔎 PHASE 1 — FULL ARCHITECTURE AUDIT
1️⃣ Verify Clean Architecture

Check that layers are strictly separated:

Domain

Application

Infrastructure

Presentation (API)

Confirm:

Domain has NO EF references

Application has NO direct DB access

Infrastructure implements interfaces

API layer contains only Controllers

If violations exist:

Show file paths

Explain why wrong

Refactor properly

2️⃣ Verify CQRS + MediatR

Check:

Commands & Queries separated

Controllers contain NO business logic

Handlers contain logic

Proper Request/Response models

Pipeline behaviors implemented correctly

If missing:

Add pipeline logging behavior

Add validation behavior

3️⃣ Verify EF Core

Check:

DbContext properly configured

Migrations working

Relationships correct

No tracking issues

Async used everywhere

No N+1 queries

Optimize queries where needed.

4️⃣ Verify Authentication & Authorization

Check:

JWT configuration

Token expiration

Secure secret storage

Role-based authorization

Policy-based authorization

Identity configuration

Test:

Register

Login

Access protected endpoint

Role restrictions

If broken → fix fully.

5️⃣ Middleware & Logging

Check:

Global exception middleware

Standardized API response

Request/Response logging

Performance timing

Serilog configured properly

Improve log structure if needed.

6️⃣ Pagination, Filtering, Sorting

Verify:

IQueryable usage

Efficient SQL translation

Proper query parameters

Default limits

Fix inefficient LINQ if present.

7️⃣ Redis Caching

Check:

Cache invalidation strategy

Proper key naming

Expiry policies

No stale data

If missing:

Implement caching decorator pattern

8️⃣ Hangfire Background Jobs

Verify:

Jobs registered

Dashboard secured

Retry policies configured

Job storage configured properly

🚨 CRITICAL SECTION — REPORT SERVICE

You MUST deeply audit the Report Service.

Check:

Is it in Application layer?

Does it violate Clean Architecture?

Is it synchronous when it should be async?

Does it generate:

CSV?

Excel?

PDF?

Test scenarios:

Generate employee report

Filtered report

Date range report

Large dataset report

Background-generated report

If broken:

Rebuild it properly

Use background job if heavy

Store generated file

Return download URL

Add logging

Make it production-ready.

🧪 PHASE 2 — COMPLETE TEST STRATEGY

Generate:

1️⃣ Unit Tests

MediatR Handlers

Validators

Services

Repository layer

Report service

Auth service

Include:

Happy path

Failure path

Edge cases

2️⃣ Integration Tests

Full request flow

DB interaction

Authentication flow

Authorization flow

Pagination

Report generation

3️⃣ Manual Testing via Postman / Bruno

Create a structured collection:

🔐 Auth Folder

Register

Login

Refresh Token

👥 Employee Folder

Create Employee

Update

Delete

Get All (pagination)

Filtered Search

Sorted Search

📊 Report Folder

Generate Report

Download Report

Large Data Report

Background Report

🧪 Test Scripts

Add automated test scripts:

Status code checks

Response schema validation

JWT extraction

Role testing

📚 PHASE 3 — FULL DOCUMENTATION OUTPUT

You MUST generate:

📘 1️⃣ Complete System Documentation

Explain:

Folder structure

Layer responsibilities

Dependency flow

Request lifecycle

Middleware execution order

Authentication flow

CQRS flow

Caching flow

Report generation flow

🔄 2️⃣ Request Flow Examples

Example:

Create Employee Flow:

Client → Controller → MediatR → Command → Handler → Repository → EF Core → DB → Response → Middleware → Client

Explain every step in detail.

🧠 3️⃣ What Currently Exists in Code

Document:

All services

All handlers

All validators

All entities

All middleware

All background jobs

All configurations

🚀 4️⃣ Future Improvements & Enhancements

Suggest:

Event-driven architecture

Microservices readiness

API versioning

OpenTelemetry

Health checks

Docker support

CI/CD

Rate limiting

Refresh tokens

API Gateway compatibility

DDD improvements

📋 FINAL OUTPUT FORMAT REQUIRED

Return:

Architecture Audit Report

Bug & Fix Report

Performance Issues

Security Issues

Refactored Code (if needed)

Full Testing Guide

Postman/Bruno Collection Structure

Report Service Deep Fix

Full System Documentation

Future Roadmap

⚠️ IMPORTANT RULES

Be extremely strict

Think like a production auditor

Improve everything

Follow Clean Architecture strictly

Follow SOLID

Follow best practices

No shortcuts

Production-grade only

If something is missing, design it properly.

If something is wrong, fix it completely.

If something is good, explain why it is good.
