# FEDProtect

**Protect yourself from surveillance before, during, and after protests**

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

---

## What Is This?

**FEDProtect** is a free, open-source application that helps you protect your phone and computer from government surveillance when attending protests, rallies, or demonstrations.

**In plain English:** When you go to a protest, law enforcement agencies can use various technologies to identify you, track your location, intercept your messages, and access your devices. This tool tells you exactly what those threats are and gives you step-by-step instructions to protect yourself.

---

## Why Do I Need This?

### The Problem

When you attend a protest in the United States, you may be subject to:

| Surveillance Method | What It Means For You |
|---------------------|----------------------|
| **Facial Recognition** | Cameras at protests feed into databases that can identify you by your face, even in a crowd |
| **Geofence Warrants** | Government can demand Google/Apple hand over data on everyone who was in a specific area at a specific time |
| **Cell-Site Simulators (Stingrays)** | Fake cell towers that trick your phone into connecting, allowing interception of calls and texts |
| **Social Media Monitoring** | Agencies actively monitor hashtags, posts, and accounts related to protests |
| **Device Seizure** | If arrested, police may attempt to search your phone—and courts have ruled this legal in many cases |
| **IMSI Catchers** | Devices that collect the unique ID of every phone in an area |

**These aren't conspiracy theories.** They're documented tactics used by ICE, FBI, DHS, and local police departments. The ACLU, EFF, and investigative journalists have exposed these programs through FOIA requests and court cases.

### The Solution

FEDProtect takes complex security research and turns it into simple, actionable steps:

1. **Select a threat profile** that matches your situation (ICE raids, urban protests, network surveillance)
2. **See the specific threats** you face with severity ratings
3. **Get clear mitigations** - exactly what to do, step by step
4. **Generate configurations** - Firefox privacy settings, firewall rules, checklists you can print

---

## Who Should Use This?

- **Protesters** attending demonstrations, marches, or rallies
- **Activists** organizing in their communities
- **Journalists** covering civil unrest or immigration enforcement
- **Legal observers** documenting police activity
- **Anyone** attending public gatherings where surveillance is likely

---

## What Does It Actually Do?

### 1. Shows You The Threats

The app includes "threat profiles" - detailed descriptions of surveillance capabilities based on public research, court documents, and investigative reporting. Each threat includes:

- **What it is** (e.g., "Facial Recognition at Protests")
- **How severe it is** (High, Medium, Low)
- **Where the information comes from** (cited sources you can verify)
- **What you can do about it** (specific mitigations)

### 2. Gives You A Checklist

Generate a printable checklist you can follow before attending a protest:

```
□ Enable full-disk encryption on your phone
□ Use a strong alphanumeric passcode (not fingerprint/face)
□ Disable location services
□ Turn off WiFi and Bluetooth
□ Install Signal for encrypted messaging
□ Write lawyer's phone number on your arm (not in your phone)
```

### 3. Configures Your Browser

The app can generate a `user.js` file for Firefox that:

- Blocks trackers and fingerprinting
- Disables WebRTC (which can leak your real IP)
- Enables privacy-focused DNS
- Hardens against common surveillance techniques

### 4. Checks Your Computer's Security

Run security checks to verify:

- Is your firewall enabled?
- Is your disk encrypted?
- Is Windows Defender running?
- Is Remote Desktop disabled?

---

## Important: What This Tool Does NOT Do

**This tool does not:**

- Connect to the internet (it's completely offline)
- Collect any data about you
- Install VPNs or Tor (it tells you how to do it yourself)
- Make you invisible to surveillance (nothing can do that)
- Provide legal advice (consult a lawyer)
- Guarantee your safety (the threat landscape evolves)

**This tool does:**

- Educate you about real, documented threats
- Give you actionable steps based on expert recommendations
- Help you make informed decisions about your digital security
- Run entirely on your own computer with no external connections

---

## Download & Installation

### Option 1: Download Pre-Built Application (Easiest)

Go to the [Releases](https://github.com/seloc0des/FEDProtect/releases) page and download:

| Your Computer | Download This |
|---------------|---------------|
| Windows | `FEDProtect-Windows-GUI.zip` |
| Linux | `FEDProtect-Linux-GUI.zip` |

**Windows:**
1. Download and extract the ZIP file
2. Double-click `FEDProtect.exe`
3. That's it - no installation needed

**Linux:**
1. Download and extract the ZIP file
2. Open a terminal in the extracted folder
3. Run: `chmod +x FEDProtect && ./FEDProtect`

### Option 2: Build From Source

If you want to verify the code yourself (recommended for maximum trust):

```bash
# Install .NET 8.0 SDK from https://dotnet.microsoft.com/download

# Clone this repository
git clone https://github.com/seloc0des/FEDProtect.git
cd protest-hardener

# Build and run
dotnet build
cd src/FEDProtect.UI
dotnet run
```

---

## How To Use It

### Step 1: Launch the Application

Double-click the application or run it from the terminal.

### Step 2: Select a Threat Profile

On the left side, you'll see available threat profiles:

- **us-ice-raids-2026** - For situations involving immigration enforcement
- **us-high-risk-protest-2026** - For large urban protests with expected police presence
- **us-network-lockdown-2026** - For situations where network surveillance is likely

Click on a profile to load it.

### Step 3: Review the Threats

The main panel shows you the surveillance capabilities in that profile:

- **Facial Recognition** (HIGH) - Police use cameras and AI to identify protesters
- **Geofence Warrants** (HIGH) - Government can get location data for everyone in an area
- **Device Seizure** (MEDIUM) - If arrested, your phone may be searched

Click on any threat to see the recommended mitigations.

### Step 4: Take Action

Use the buttons at the top to:

- **Generate Checklist** - Creates a printable markdown file with everything you need to do
- **Create Firefox Profile** - Generates a privacy-hardened browser configuration
- **Run Security Checks** - Verifies your computer's security settings

### Step 5: Follow The Recommendations

The checklist will include things like:

**Before the Protest:**
- Back up your phone and leave the backup at home
- Enable airplane mode or leave your phone at home entirely
- If you must bring your phone, disable biometrics and use a strong passcode
- Write important phone numbers on paper or your arm

**During the Protest:**
- Keep your phone locked at all times
- Don't take photos that show other protesters' faces
- Use Signal for any communications

**After the Protest:**
- Check for any new apps or profiles installed on your device
- Review which apps accessed your location
- Document any interactions with law enforcement

---

## Command Line Interface (For Advanced Users)

If you prefer the terminal, there's also a CLI version:

```bash
# List available profiles
./protest-hardener list-profiles

# Show details of a specific profile
./protest-hardener show-profile us-ice-raids-2026

# Generate a checklist
./protest-hardener generate-checklist us-ice-raids-2026 --output checklist.md

# Validate a custom profile
./protest-hardener validate-profile my-custom-profile.yaml

# Preview what hardening would do (without making changes)
./protest-hardener apply us-ice-raids-2026 --dry-run
```

---

## Threat Profiles Explained

Threat profiles are simple text files (YAML format) that describe:

```yaml
name: US ICE Raids 2026
description: Threat profile for ICE enforcement operations

capabilities:
  - id: facial_recognition
    name: Facial Recognition
    severity: high
    description: ICE uses facial recognition to identify individuals
    sources:
      - title: "ACLU Report on ICE Facial Recognition"
        url: https://www.aclu.org/...
    mitigations:
      - action: Wear face covering
        description: Masks, sunglasses, and hats reduce facial recognition accuracy
        difficulty: easy
        effectiveness: high
```

**Why YAML files?**
- Human-readable (you can open and verify them yourself)
- Community-maintainable (anyone can contribute)
- No code execution (they're just data, not programs)
- Fully auditable (you can see exactly what the app knows)

---

## Frequently Asked Questions

### Is this legal?

Yes. Protecting your own privacy is legal. This tool recommends standard security practices like encryption, strong passwords, and privacy-focused browsers - all completely legal.

### Does this make me look suspicious?

Using encryption and privacy tools is increasingly common and is recommended by security experts for everyone, not just protesters. Many of these recommendations (like using Signal) are standard advice from organizations like the EFF.

### Can this tool be trusted?

The code is 100% open source. You can:
- Read every line of code
- Build it yourself from source
- Verify it never connects to the internet
- Have security researchers audit it

### What if I'm not technical?

That's exactly who this tool is for. The GUI is designed to be simple:
1. Pick a profile
2. Click "Generate Checklist"
3. Follow the checklist

### Will this protect me completely?

**No.** No tool can guarantee protection. Surveillance technology evolves constantly. This tool helps you make informed decisions and take reasonable precautions, but it's one layer of defense, not a magic shield.

### Why doesn't this tool install Tor/VPN for me?

We intentionally don't bundle or auto-install other software because:
- You should understand what you're installing
- Pre-bundled software could be tampered with
- Official sources are always more trustworthy
- It keeps this tool simple and auditable

---

## Privacy & Security

### This Tool's Privacy

- **Zero internet connections** - Works completely offline
- **Zero data collection** - Nothing is logged or sent anywhere
- **Zero telemetry** - No analytics, no tracking, no phone-home
- **100% local** - Everything stays on your computer

### Your Security

This tool helps with digital security, but remember:

- **Physical security matters too** - Buddy systems, legal observers, know your rights
- **OpSec is holistic** - One weak link can compromise everything
- **Stay informed** - Threats evolve; check for profile updates
- **Trust but verify** - Read the sources cited in threat profiles

---

## Contributing

### Adding Threat Profiles

If you have documented information about surveillance tactics:

1. Create a YAML file following the existing format
2. **Cite all sources** - news articles, FOIA documents, academic papers
3. Test it with `protest-hardener validate-profile your-file.yaml`
4. Submit a pull request

### Reporting Issues

- Found a bug? [Open an issue](https://github.com/seloc0des/FEDProtect/issues)
- Security vulnerability? See [SECURITY.md](SECURITY.md)
- Question? [Start a discussion](https://github.com/seloc0des/FEDProtect/discussions)

---

## Resources

Learn more about protecting yourself:

- **[EFF Surveillance Self-Defense](https://ssd.eff.org/)** - Comprehensive security guides
- **[ACLU Know Your Rights](https://www.aclu.org/know-your-rights/protesters-rights/)** - Legal rights at protests
- **[Tor Project](https://www.torproject.org/)** - Anonymous browsing
- **[Signal](https://signal.org/)** - Encrypted messaging
- **[Freedom of the Press Foundation](https://freedom.press/)** - Security for journalists

---

## Legal Disclaimer

**This software is provided "as-is" without warranty of any kind.**

- This is not legal advice. Consult an attorney for legal questions.
- This does not guarantee protection from surveillance.
- Users are responsible for understanding and complying with local laws.
- The creators assume no liability for use or misuse of this tool.

**License:** GPLv3 - Free and open source forever.

---

## Acknowledgments

This project is informed by research from:
- American Civil Liberties Union (ACLU)
- Electronic Frontier Foundation (EFF)
- Amnesty International
- The Tor Project
- Citizen Lab
- Investigative journalists exposing surveillance programs

---

**FEDProtect** - Because knowing the threat is the first step to protecting yourself.

*Your privacy is a right. Protect it.*
