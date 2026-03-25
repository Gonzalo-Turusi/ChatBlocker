# ChatBlocker

A simple overlay application for manually covering chat areas in games and applications.

## What It Does

ChatBlocker is a transparent window that you can position over any chat area to manually block it visually. It does not perform any automatic detection - it's a manual coverage tool.

## Features

- **Manual Overlay**: Window that covers the area you decide
- **Click-Through**: Transparent overlay that doesn't interfere with mouse input
- **Always on Top**: Works over fullscreen games and applications
- **Customizable**: Color, size, position, and opacity adjustable
- **Configurable Hotkeys**: Assign any key combination with modifiers

## Controls

### Default Hotkeys
- **F7**: Open configuration window
- **F8**: Toggle edit mode (move/resize overlay)
- **F9**: Toggle overlay visibility on/off
- **F10**: Exit application

### Edit Mode (F8)
- Drag the overlay to position it over chat
- Drag corners to resize the overlay
- Red border visible when in edit mode
- Position and size information displayed on screen

## Basic Usage

1. **Run**: `ChatBlocker.exe`
2. **Position**: Press F8 → drag over chat area
3. **Configure**: Press F7 → set color, opacity, hotkeys
4. **Use**: Press F9 to show/hide overlay

## Configuration (F7)

### Hotkey Assignment
1. Click on any hotkey field
2. Press your desired key combination
3. Supports Ctrl, Alt, Shift modifiers
4. Settings are saved automatically

### Visual Customization
- **Color**: Click "Choose Color" → select any RGB color
- **Opacity**: Slider 0-100% for transparency control
- **Reset**: Button to restore default settings

## Technologies Used

- **Language**: C# with .NET 10.0
- **Interface**: Windows Forms (Win32 API)
- **Graphics**: System.Drawing for rendering
- **Configuration**: JSON for settings persistence
- **Hotkeys**: Windows API for global registration

## Build and Run

### From Source
1. Clone or download this repository
2. Open terminal in project directory
3. Run build command:

```bash
dotnet build
```

4. Run application:

```bash
dotnet run
```

### Generate Executable
1. Ensure .NET 10.0 SDK is installed
2. Run publish command:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

3. Executable generated at: `bin\Release\net10.0-windows\win-x64\publish\ChatBlocker.exe`

## Default Configuration

```json
{
  "X": 16,
  "Y": 413,
  "Width": 421,
  "Height": 180,
  "OverlayColor": "Black",
  "OpacityWhenDetected": 1.0,
  "EditModeHotkey": { "Key": "F8", "Ctrl": false, "Alt": false, "Shift": false },
  "ToggleHotkey": { "Key": "F9", "Ctrl": false, "Alt": false, "Shift": false },
  "ExitHotkey": { "Key": "F10", "Ctrl": false, "Alt": false, "Shift": false }
}
```

## Troubleshooting

**Overlay not visible**:
- Press F8 to enter edit mode
- Check if overlay is disabled (press your Toggle hotkey)

**Hotkeys not working**:
- Press F7 to verify hotkey assignments
- Ensure settings are saved (click Save & Close)

**Performance issues**:
- Reduce overlay size to cover only chat area
- Use simpler colors (black requires less rendering)
- Close other applications if needed

## License

This project is provided as-is for educational and personal use.

---

**ChatBlocker** - Simple, direct, and functional.
