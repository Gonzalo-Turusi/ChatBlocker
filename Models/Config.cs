using System.Drawing;

namespace ChatBlocker.Models
{
    public class Config
    {
        // User's perfect position for their chat area
        public int X { get; set; } = 16;
        public int Y { get; set; } = 413;
        public int Width { get; set; } = 421;
        public int Height { get; set; } = 180;
        
        // Color configuration (ARGB format)
        public int OverlayColorArgb { get; set; } = Color.Black.ToArgb();
        
        public double OpacityWhenDetected { get; set; } = 1.0;
        public double OpacityWhenHidden { get; set; } = 0.0;
        public bool IsEnabled { get; set; } = true;
        
        public HotkeyConfig EditModeHotkey { get; set; } = new HotkeyConfig { Key = "F8" };
        public HotkeyConfig ToggleHotkey { get; set; } = new HotkeyConfig { Key = "F9" };
        public HotkeyConfig ExitHotkey { get; set; } = new HotkeyConfig { Key = "F10" };
        
        public Rectangle GetRectangle()
        {
            return new Rectangle(X, Y, Width, Height);
        }
        
        public void SetRectangle(Rectangle rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }
        
        public Color GetOverlayColor()
        {
            return Color.FromArgb(OverlayColorArgb);
        }
        
        public void SetOverlayColor(Color color)
        {
            OverlayColorArgb = color.ToArgb();
        }
        
        // Initialize with user's perfect position
        public void InitializeForScreen(int screenWidth, int screenHeight)
        {
            // Use the user's perfect coordinates
            X = 16;
            Y = 413;
            Width = 421;
            Height = 180;
            
            Console.WriteLine($"Initialized config for {screenWidth}x{screenHeight}: {GetRectangle()}");
        }
        
        // Reset all values to defaults
        public void ResetToDefaults()
        {
            X = 16;
            Y = 413;
            Width = 421;
            Height = 180;
            OverlayColorArgb = Color.Black.ToArgb();
            OpacityWhenDetected = 1.0;
            OpacityWhenHidden = 0.0;
            IsEnabled = true;
            EditModeHotkey = new HotkeyConfig { Key = "F8" };
            ToggleHotkey = new HotkeyConfig { Key = "F9" };
            ExitHotkey = new HotkeyConfig { Key = "F10" };
        }
    }
    
    public class HotkeyConfig
    {
        public string Key { get; set; } = "";
        public bool Ctrl { get; set; } = false;
        public bool Alt { get; set; } = false;
        public bool Shift { get; set; } = false;
    }
}
