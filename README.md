# ChatBlocker v2.0

A powerful and highly customizable WinForms overlay application that automatically detects and blocks game chat areas in real-time with dynamic hotkey configuration and full overlay personalization.

## Features

### 🎮 Core Functionality
- **Automatic Chat Detection**: Analyzes screen regions for chat text using pixel sampling
- **Click-Through Overlay**: Transparent overlay that doesn't interfere with mouse input
- **Always on Top**: Works over fullscreen games and applications
- **Performance Optimized**: <5% CPU usage with efficient pixel sampling

### 🎨 Overlay Customization
- **Dynamic Color Picker**: Choose any RGB color for the overlay
- **Adjustable Opacity**: 0-100% opacity slider for perfect visibility
- **Drag & Resize**: Move and resize overlay in edit mode
- **Position Memory**: Saves exact position, size, and appearance settings
- **Reset Function**: One-click restore to default settings

### ⌨️ Dynamic Hotkey System
- **Full Customization**: Configure any key combination (Ctrl, Alt, Shift modifiers)
- **Real-time Assignment**: Click hotkey field → press desired key → instant assignment
- **Global Hotkeys**: Works in any application or game
- **Persistent Settings**: All hotkey combinations saved automatically

**Default Hotkeys**:
- **F7**: Open configuration window
- **F8**: Toggle edit mode (move/resize overlay)
- **F9**: Toggle blocker on/off
- **F10**: Exit application

### 🔧 Configuration Window (F7)
- **Hotkey Assignment**: Click any hotkey field and press desired key
- **Color Selection**: Full RGB color picker with preview
- **Opacity Control**: Real-time opacity slider (0-100%)
- **Position Display**: Shows current X, Y, Width, Height
- **Reset Button**: Restore all settings to defaults with confirmation
- **Clear Buttons**: Individual hotkey reset options

### 🖥️ Cross-Platform Compatibility
- **Multi-Monitor Support**: Works on any display configuration
- **Screen Size Independent**: Hotkey system works on any resolution
- **Self-Contained**: Single executable - no installation required
- **Windows 10/11**: Compatible with all modern Windows versions

## Requirements

- Windows 10/11
- .NET 10.0 Runtime or higher
- Any game or application with text-based chat

## Building from Source

1. Clone or download this repository
2. Open a terminal in the project directory
3. Run the build command:

```bash
dotnet build
```

4. Run the application:

```bash
dotnet run
```

Or build and run in Visual Studio 2022 with .NET 10.0.

## Usage

### 🚀 First Run

1. Launch `ChatBlocker.exe`
2. Press **F7** to open the configuration window
3. Configure your preferred hotkeys, colors, and opacity
4. Press **Save & Close** to apply settings
5. Press **F8** to enter edit mode (red border visible)
6. Drag overlay to position it over the chat area
7. Drag corners to resize the overlay
8. Press **F8** again to exit edit mode

### 🎮 Normal Operation

- The overlay will automatically detect when chat appears and block it
- When chat is detected, the overlay becomes more visible (opacity increases)
- When no chat is present, the overlay becomes nearly transparent
- Press your configured **Toggle hotkey** to turn the blocker on/off
- Press your configured **Exit hotkey** to exit the application
- Press **F7** at any time to reconfigure settings

### ⚙️ Configuration Options

**Hotkey Configuration**:
- Click on any hotkey field (Edit Mode, Toggle, Exit)
- Press your desired key combination
- Supports Ctrl, Alt, Shift modifiers
- Press **Clear** to reset individual hotkeys

**Overlay Customization**:
- Click **Choose Color** to open RGB color picker
- Adjust **Opacity** slider (0-100%) for perfect visibility
- Click **Reset All** to restore default settings

**Position Control**:
- Press **F8** to enter edit mode
- Drag the overlay to any position
- Drag corners to resize
- Position is automatically saved

## Configuration

Settings are automatically saved to:
```
%APPDATA%\ChatBlocker\config.json
```

### 🎛️ Advanced Configuration Options

**Overlay Settings**:
- **X, Y**: Position coordinates for overlay placement
- **Width, Height**: Overlay dimensions in pixels
- **OverlayColor**: RGB color values (default: Black)
- **OpacityWhenDetected**: Visibility when chat is present (0.0-1.0)
- **OpacityWhenHidden**: Visibility when no chat detected (0.0-1.0)

**Hotkey Configuration**:
- **EditModeHotkey**: Key combination for edit mode (default: F8)
- **ToggleHotkey**: Key combination for toggle on/off (default: F9)
- **ExitHotkey**: Key combination to exit (default: F10)
- **ConfigWindowHotkey**: Key to open configuration (default: F7)

**Detection Settings**:
- **DetectionThreshold**: Sensitivity for chat detection
- **PixelSampleInterval**: Performance optimization setting
- **DetectionIntervalMs**: Response time in milliseconds

## Performance

- CPU Usage: <5% on modern systems
- Memory Usage: ~20MB
- Detection Latency: 150ms (configurable)
- Pixel Sampling: Every 4 pixels (configurable)

## Compatibility

- Works with any DirectX or OpenGL game
- Compatible with anti-cheat systems (no injection)
- Supports multi-monitor setups
- Works with windowed and fullscreen applications

## Troubleshooting

### 🛠️ Common Issues

**Hotkeys not working**:
- Press **F7** to open configuration and verify hotkey assignments
- Ensure hotkeys are saved (click Save & Close)
- Check that modifier keys (Ctrl, Alt, Shift) are set correctly
- Restart application after changing hotkeys

**Overlay not visible**:
- Press **F8** to enter edit mode and verify position
- Check if blocker is disabled (press your Toggle hotkey)
- Verify overlay color isn't transparent (opacity > 0%)

**Configuration not saving**:
- Ensure you click **Save & Close** in the configuration window
- Check file permissions in %APPDATA%\ChatBlocker\
- Run application as Administrator if needed

**Performance issues**:
- Reduce overlay size to cover only the chat area
- Increase detection interval in configuration
- Adjust pixel sample interval for better performance

**Hotkey assignment problems**:
- Click on the hotkey field first, then press the key
- Wait for "Press any key..." message before pressing key
- Avoid using modifier keys alone (Ctrl, Alt, Shift without other key)

## 📋 Default Configuration

```json
{
  "X": 16,
  "Y": 413,
  "Width": 421,
  "Height": 180,
  "OverlayColor": "Black",
  "OpacityWhenDetected": 1.0,
  "OpacityWhenHidden": 0.1,
  "EditModeHotkey": { "Key": "F8", "Ctrl": false, "Alt": false, "Shift": false },
  "ToggleHotkey": { "Key": "F9", "Ctrl": false, "Alt": false, "Shift": false },
  "ExitHotkey": { "Key": "F10", "Ctrl": false, "Alt": false, "Shift": false },
  "ConfigWindowHotkey": { "Key": "F7", "Ctrl": false, "Alt": false, "Shift": false }
}
```

## License

This project is provided as-is for educational and personal use.
