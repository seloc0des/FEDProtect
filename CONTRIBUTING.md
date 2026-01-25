# Contributing to FEDProtect

Thank you for your interest in contributing to FEDProtect! This project aims to protect protesters from surveillance through community-driven threat intelligence and automated device hardening.

## Ways to Contribute

### 1. Add or Update Threat Profiles

Threat profiles are the core of FEDProtect. They document surveillance tactics and provide concrete mitigations.

**Requirements:**
- All threats must be cited with credible sources (ACLU, EFF, news, academic research)
- Mitigations must be specific, testable, and actionable
- Follow the YAML schema (see existing profiles in `/profiles`)

**Process:**
1. Research the surveillance threat thoroughly
2. Create a new YAML file in `/profiles` directory
3. Include all required fields: name, description, capabilities, sources, mitigations
4. Test the profile: `dotnet run --project src/FEDProtect.CLI -- show-profile your-profile-name`
5. Submit a pull request with:
   - Description of the threat
   - Links to all sources
   - Real-world context

### 2. Improve Code

**Areas for contribution:**
- Core services (ProfileManager, HardeningApplier, OsChecker)
- CLI commands and output formatting
- GUI features and usability
- Cross-platform compatibility
- Performance optimizations

**Process:**
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Write code following C# conventions
4. Add unit tests in `/tests`
5. Ensure all tests pass: `dotnet test`
6. Submit a pull request

### 3. Documentation

Help make FEDProtect more accessible:
- Improve README clarity
- Add usage examples
- Create tutorials or guides
- Translate documentation
- Document threat modeling

### 4. Testing & Bug Reports

- Test on different platforms (Linux, Windows, macOS)
- Report bugs with detailed reproduction steps
- Suggest usability improvements
- Verify threat profile accuracy

## Code Style Guidelines

### C# Conventions
- Use PascalCase for public members
- Use camelCase for private fields (prefix with `_`)
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Keep methods focused and small

### YAML Profiles
- Use snake_case for field names
- Include all required fields
- Cite sources with title, URL, author, and date
- Rate difficulty: trivial, easy, medium, hard
- Rate effectiveness: low, medium, high, critical
- Rate severity: low, medium, high

## Pull Request Process

1. **Before submitting:**
   - Ensure code builds: `dotnet build`
   - Run tests: `dotnet test`
   - Update documentation if needed
   - Add yourself to contributors list

2. **PR description should include:**
   - What changes were made
   - Why the changes are needed
   - How to test the changes
   - Any breaking changes

3. **Review process:**
   - Maintainers will review within 7 days
   - Address feedback and requested changes
   - Once approved, maintainers will merge

## Threat Profile Schema

```yaml
name: "Profile Name"
description: "Brief description"
version: "1.0"
last_updated: "YYYY-MM-DD"
regions: ["US", "EU"]
contexts: ["protests", "activism"]

capabilities:
  - id: "unique_id"
    name: "Threat Name"
    severity: "high"  # high, medium, low
    description: "Detailed description"
    sources:
      - title: "Source Title"
        url: "https://..."
        author: "Organization"
        date: "YYYY-MM-DD"
    mitigations:
      - id: "mitigation_id"
        action: "action_name"
        description: "What to do"
        difficulty: "easy"  # trivial, easy, medium, hard
        effectiveness: "high"  # low, medium, high, critical
        resource: "https://..."  # optional
        os: ["linux", "windows"]  # optional

countermeasures:
  browser_profile:
    name: "Browser Profile Name"
    description: "Description"
    settings:
      - "setting.name=value"
    extensions:
      - name: "Extension Name"
        description: "What it does"
        url: "https://..."
    notes: "Additional notes"

legal_notes: |
  Legal guidance and disclaimers

sources:
  - "https://source1.org"
  - "https://source2.org"

tags: ["tag1", "tag2"]
```

## Community Guidelines

### Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Assume good intentions
- No harassment or discrimination
- Protect user privacy and safety

### Ethical Guidelines

FEDProtect is designed to:
- ✅ Protect human rights and civil liberties
- ✅ Enable safe protest participation
- ✅ Counter unjust surveillance
- ✅ Empower marginalized communities

FEDProtect should NOT be used to:
- ❌ Surveil others
- ❌ Target marginalized groups
- ❌ Enable illegal activity
- ❌ Circumvent legitimate security

## Security

If you discover a security vulnerability:
1. **Do NOT** open a public issue
2. Email security details to: selodev3d@gmail.com
3. Include steps to reproduce
4. Allow time for a fix before disclosure

See [SECURITY.md](SECURITY.md) for full policy.

## License

By contributing, you agree that your contributions will be licensed under GPLv3.

## Questions?

- Open a [GitHub Discussion](https://github.com/seloc0des/FEDProtect/discussions)
- Visit [selodev.com](https://selodev.com)
- Check existing issues and PRs
- Read the documentation in `/docs`

---

Thank you for helping protect protesters worldwide! 🛡️
