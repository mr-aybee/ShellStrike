# ShellStrike

A C# command automation and administration solution targeting .NET Framework 4.6.

## Architecture

| Component | Responsibility |
| --- | --- |
| `ShellStrike/` | Command models and execution components |
| `ShellStrikeController/` | Controller |
| `ShellStrikeService/` | Service host |
| `ShellStrikeTest/`, `TestPlink/`, `TestPlinkApp/` | Test and demonstration projects |

## Development

Open `ShellStrike.sln` with compatible .NET Framework tooling and restore dependencies. Review service configuration before launching any component.

## Status and security

Legacy implementation. Build and runtime behavior are unverified. Evaluate authentication, command authorization and network exposure in an isolated test environment before deploying. Do not expose the service to untrusted networks.
