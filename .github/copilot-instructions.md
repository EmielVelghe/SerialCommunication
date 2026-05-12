# Copilot instructions for SerialCommunication

Purpose
- Provide repository-specific guidance for Copilot-assisted sessions.

Quick commands
- Build (Windows/.NET GUI): Open SerialCommunication.slnx in Visual Studio and Build Solution.
- CLI build (MSBuild): msbuild "SerialCommunication.slnx" /p:Configuration=Debug
- Arduino (sketch): Use Arduino IDE (open SerialCommunication.ino) or arduino-cli:
  - Compile: arduino-cli compile --fqbn <fqbn> SerialCommunication.ino
  - Upload: arduino-cli upload -p <port> --fqbn <fqbn> SerialCommunication
- Tests: No automated unit tests present. For a quick smoke test:
  1) Upload the sketch to the board.
  2) Run the Windows app (SerialCommunication\bin\Debug\SerialCommunication.exe) or start from Visual Studio.
  3) Open a serial monitor at 115200 baud and send commands like `ping\n`, `get a0\n`, `set d2 on\n`.
- Linting: No linter configured for C# or Arduino/C in repo.

High-level architecture
- Two primary components:
  1) Windows WinForms GUI (SerialCommunication/): C# app that opens a serial port and provides a user interface to send/receive textual commands.
  2) Arduino firmware (SerialCommunication.ino + analog.c + SerialCommand.*): listens on serial, tokenizes commands and executes hardware I/O.
- SerialCommand (SerialCommand.h/.cpp): an Arduino-side lightweight command parser. The sketch registers callbacks (set, get, toggle, ping, help, debug) and a default handler.
- Communication: ASCII text commands, tokenized by space, terminated by newline. Default baud rate in sketch: 115200.

Key conventions and repo specifics
- Command syntax: `verb [args...]`. Common verbs: set, get, toggle, ping, help, debug.
- Pin naming conventions in the sketch: digital = dN (e.g., d2), PWM = pwmN, analog input = aN.
- Input validation is performed in the sketch (ranges enforced); keep command strings within SERIALCOMMANDBUFFER (32 bytes by default).
- SerialCommand constants can be changed but be mindful of buffer sizes and MAXSERIALCOMMANDS.
- Debug mode on Arduino: remove or comment out `#undef SERIALCOMMANDDEBUG` in SerialCommand.h to enable serial debug prints.
- Baudrate: The sketch defines `Baudrate 115200`. Any PC-side client must use the same baud rate.
- The repository contains generated artifacts (.vs/, SerialCommunication\bin/, obj/). Prefer building locally rather than editing committed binaries.

Files to check before changing behavior
- SerialCommand.h / SerialCommand.cpp: command parsing, delimiter/terminator, buffer sizes.
- SerialCommunication.ino and analog.c: command handlers and hardware I/O ranges.
- SerialCommunication\Form1.cs: GUI serial port usage and message framing (if adjusting protocol).

AI-related / other configs
- No CLAUDE.md, .cursorrules, AGENTS.md, CONVENTIONS.md, or similar AI-assistant config files detected.

If anything here should be expanded (examples, more CLI snippets, board fqbn used for CI), say which areas to cover and Copilot can update this file.

Suggested improvements for Copilot sessions

- Add an explicit Arduino example fqbn so Copilot can autocomplete a working compile/upload command, e.g.:
  - arduino-cli compile --fqbn arduino:avr:uno SerialCommunication.ino
  - arduino-cli upload -p <port> --fqbn arduino:avr:uno SerialCommunication
  (Keep <fqbn> and <port> as placeholders if board varies.)

- Add a Release build example for msbuild:
  - msbuild "SerialCommunication.slnx" /p:Configuration=Release

- Note a subtle terminator detail useful for protocol changes: SerialCommand.h comments reference '\r' as the default terminator, but SerialCommand.cpp constructs the object with term = '\n'. Form1.cs sets SerialPort.NewLine = "\n" and the sketch uses Baudrate 115200 — check CR vs LF handling if clients behave unexpectedly.

- Recommend removing committed build artifacts (bin/, obj/, .vs/) from the repository history or keeping them out of future commits so Copilot doesn't surface compiled binaries as sources of truth.

If helpful, Copilot can apply these edits, add a short README with example fqbn values, or open a PR with the suggested removals. Which would you like next?
