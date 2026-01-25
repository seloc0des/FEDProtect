# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 0.1.x   | :white_check_mark: |

## Reporting a Vulnerability

FEDProtect takes security seriously. If you discover a security vulnerability, please follow responsible disclosure practices.

### How to Report

1. **DO NOT** open a public GitHub issue for security vulnerabilities
2. Email details to: selodev3d@gmail.com
3. Include:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Any suggested fixes

### What to Expect

- **Acknowledgment**: Within 48 hours of your report
- **Initial Assessment**: Within 7 days
- **Resolution Timeline**: Depends on severity, typically:
  - Critical: 24-72 hours
  - High: 1-2 weeks
  - Medium: 2-4 weeks
  - Low: Next scheduled release

### Scope

The following are in scope:

- **Application code**: CLI, UI, and Core library
- **Threat profiles**: Inaccurate or dangerous recommendations
- **Build/deployment**: Supply chain vulnerabilities
- **Documentation**: Security-sensitive errors

### Out of Scope

- Third-party dependencies (report to upstream maintainers)
- Social engineering attacks
- Physical security
- Denial of service attacks

## Security Design Principles

### What We Do

1. **No telemetry**: Zero data collection, no phone-home
2. **Offline-first**: Works without internet after initial setup
3. **No credentials storage**: We never store passwords or API keys
4. **Auditable**: Full source code available, all sources cited
5. **Minimal permissions**: Only request permissions we need

### What We Don't Do

1. **No embedded binaries**: We don't bundle VPNs, Tor, or other tools
2. **No auto-execution**: Firewall/system commands require manual confirmation
3. **No remote code**: Profiles are static YAML, not executable code
4. **No network requests**: Application never phones home

## Threat Model

### Trust Boundaries

1. **User's machine**: We trust the local environment
2. **Threat profiles**: Community-reviewed, but verify sources
3. **Generated configs**: Review before applying to system

### Known Limitations

1. **Not a silver bullet**: This tool is one layer of defense
2. **Profiles may be outdated**: Threat landscape evolves
3. **System commands**: UFW rules require sudo; always review
4. **Browser configs**: May break some websites

## Code Security Practices

### For Contributors

1. **Dependencies**: Keep minimal; update regularly
2. **Input validation**: Validate all YAML input
3. **Error handling**: Never expose stack traces to users
4. **Logging**: Never log sensitive information
5. **Code review**: All PRs require review

### For Users

1. **Verify downloads**: Check signatures on releases
2. **Review profiles**: Read threat sources before trusting
3. **Review commands**: Check generated shell scripts before running
4. **Stay updated**: Update to latest version regularly

## Contact

- **Security issues**: selodev3d@gmail.com
- **General questions**: [GitHub Discussions](https://github.com/seloc0des/FEDProtect/discussions)
- **Bug reports**: [GitHub Issues](https://github.com/seloc0des/FEDProtect/issues)
- **Website**: [selodev.com](https://selodev.com)

---

Thank you for helping keep FEDProtect and its users safe.
