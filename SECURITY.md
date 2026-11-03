# Security Policy

This policy covers vulnerability reporting and maintenance for the `NuciCLI.Menus` .NET library. The library is a lightweight, in-process menu system with no network, authentication, or sensitive-data handling; its attack surface is limited to consumer-provided command actions and console interaction.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)
- [Safe Harbour](#safe-harbour)
- [Recognition](#recognition)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nucicli.menus/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The original maintainers are not responsible for changes, modifications, or security issues introduced by downstream forks.

The subsequent report categories are in scope for this repository:
- Vulnerabilities in the `NuciCLI.Menus` package code published to NuGet
- Vulnerabilities in the build, packaging, or release pipeline for this repository

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in consumer applications that use the library
- Vulnerabilities in downstream forks or modified distributions
- Vulnerabilities in dependencies (`NuciCLI`, `NuciExtensions`, .NET runtime) — report to their respective maintainers
- Issues in host-provided command actions or application logic

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.