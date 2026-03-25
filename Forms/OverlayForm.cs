using ChatBlocker.Models;
using ChatBlocker.Services;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChatBlocker.Forms
{
    public partial class OverlayForm : Form
    {
        private readonly Config _config;
        private readonly ConfigService _configService;
        
        private bool _isEditMode = false;
        private bool _isDragging = false;
        private bool _isResizing = false;
        private Point _dragStartPoint;
        private Rectangle _dragStartRect;
        private ResizeMode _resizeMode = ResizeMode.None;
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WM_HOTKEY = 0x0312;
        
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        private const int MOD_NONE = 0x0000;
        private const int ID_HOTKEY_EDITMODE = 1;
        private const int ID_HOTKEY_TOGGLE = 2;
        private const int ID_HOTKEY_EXIT = 3;
        private const int ID_HOTKEY_CONFIG = 4;
        
        public OverlayForm()
        {
            _configService = new ConfigService();
            _config = _configService.LoadConfig();
            
            SetupForm();
            InitializeOverlay();
        }
        
        private void InitializeOverlay()
        {
            // Apply config position and color immediately
            var rect = _config.GetRectangle();
            this.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            this.BackColor = _config.GetOverlayColor(); // Use configured color
            
            Console.WriteLine($"Overlay positioned at: {rect} with color: {_config.GetOverlayColor()}");
            
            // Set window properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            
            // Set transparency and click-through
            UpdateTransparency();
            
            // Setup mouse events for editing
            SetupMouseEvents();
            
            // Setup timer (disabled)
            SetupTimer();
            
            // Setup hotkeys
            SetupHotkeys();
            
            Console.WriteLine("Overlay initialized successfully");
        }
        
        private void SetupMouseEvents()
        {
            // Mouse events are handled by override methods, no need to add handlers
            // The override methods OnMouseDown, OnMouseMove, OnMouseUp, OnPaint will be called automatically
        }
        
        private void SetupForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            UpdateTransparency();
        }
        
        private void UpdateTransparency()
        {
            if (_isEditMode)
            {
                this.Opacity = 0.3; // Semi-transparent for editing
                SetWindowLong(this.Handle, -20, 
                    GetWindowLong(this.Handle, -20) | WS_EX_LAYERED);
                // Don't make transparent in edit mode so we can move it
            }
            else
            {
                // Always show black rectangle when coverage is enabled
                if (_config.IsEnabled)
                {
                    this.Opacity = _config.OpacityWhenDetected; // Use configured opacity
                    SetWindowLong(this.Handle, -20, 
                        GetWindowLong(this.Handle, -20) | WS_EX_LAYERED);
                }
                else
                {
                    this.Opacity = 0.0; // Hidden when disabled
                    SetWindowLong(this.Handle, -20, 
                        GetWindowLong(this.Handle, -20) | WS_EX_LAYERED);
                }
            }
        }
        
        private void SetupTimer()
        {
            // DISABLED - No detection needed, just black rectangle
        }
        
        private void SetupHotkeys()
        {
            this.KeyPreview = true;
            this.KeyDown += OverlayForm_KeyDown;
            
            // Register global hotkeys
            RegisterGlobalHotKeys();
        }
        
        private void RegisterGlobalHotKeys()
        {
            try
            {
                // First unregister all existing hotkeys to avoid conflicts
                UnregisterAllHotkeys();
                
                // Wait a moment for unregistration to complete
                System.Threading.Thread.Sleep(100);
                
                // Register new hotkeys that work even when app doesn't have focus
                RegisterHotKey(this.Handle, ID_HOTKEY_EDITMODE, 
                    GetModifierFlags(_config.EditModeHotkey), 
                    (int)GetKeyFromConfig(_config.EditModeHotkey.Key));
                    
                RegisterHotKey(this.Handle, ID_HOTKEY_TOGGLE, 
                    GetModifierFlags(_config.ToggleHotkey), 
                    (int)GetKeyFromConfig(_config.ToggleHotkey.Key));
                    
                RegisterHotKey(this.Handle, ID_HOTKEY_EXIT, 
                    GetModifierFlags(_config.ExitHotkey), 
                    (int)GetKeyFromConfig(_config.ExitHotkey.Key));
                    
                RegisterHotKey(this.Handle, ID_HOTKEY_CONFIG, MOD_NONE, (int)Keys.F7);
                
                Console.WriteLine($"Global hotkeys registered successfully:");
                Console.WriteLine($"  Edit Mode: {GetHotkeyDisplay(_config.EditModeHotkey)}");
                Console.WriteLine($"  Toggle: {GetHotkeyDisplay(_config.ToggleHotkey)}");
                Console.WriteLine($"  Exit: {GetHotkeyDisplay(_config.ExitHotkey)}");
                Console.WriteLine($"  Config: F7");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to register global hotkeys: {ex.Message}");
                MessageBox.Show($"Failed to register hotkeys: {ex.Message}", "Hotkey Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private string GetHotkeyDisplay(HotkeyConfig hotkeyConfig)
        {
            var parts = new System.Collections.Generic.List<string>();
            
            if (hotkeyConfig.Ctrl) parts.Add("Ctrl");
            if (hotkeyConfig.Alt) parts.Add("Alt");
            if (hotkeyConfig.Shift) parts.Add("Shift");
            
            parts.Add(hotkeyConfig.Key);
            
            return string.Join(" + ", parts);
        }
        
        private int GetModifierFlags(HotkeyConfig hotkeyConfig)
        {
            int flags = MOD_NONE;
            if (hotkeyConfig.Ctrl) flags |= 0x0002; // MOD_CONTROL
            if (hotkeyConfig.Alt) flags |= 0x0001;   // MOD_ALT
            if (hotkeyConfig.Shift) flags |= 0x0004; // MOD_SHIFT
            return flags;
        }
        
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int hotkeyId = m.WParam.ToInt32();
                HandleGlobalHotkey(hotkeyId);
            }
            base.WndProc(ref m);
        }
        
        private void HandleGlobalHotkey(int hotkeyId)
        {
            try
            {
                switch (hotkeyId)
                {
                    case ID_HOTKEY_EDITMODE:
                        ToggleEditMode();
                        break;
                    case ID_HOTKEY_TOGGLE:
                        ToggleCoverage();
                        break;
                    case ID_HOTKEY_EXIT:
                        Application.Exit();
                        break;
                    case ID_HOTKEY_CONFIG:
                        OpenConfigWindow();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling global hotkey {hotkeyId}: {ex.Message}");
            }
        }
        
        private void OverlayForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == GetKeyFromConfig(_config.EditModeHotkey.Key))
            {
                ToggleEditMode();
            }
            else if (e.KeyCode == GetKeyFromConfig(_config.ToggleHotkey.Key))
            {
                // Toggle overlay coverage (enable/disable)
                ToggleCoverage();
            }
            else if (e.KeyCode == Keys.F11)
            {
                // Show current position and size
                ShowCurrentPosition();
            }
            else if (e.KeyCode == Keys.F12)
            {
                // Set current position as default
                SetAsDefault();
            }
            else if (e.KeyCode == Keys.F7)
            {
                // Open configuration window
                OpenConfigWindow();
            }
            else if (e.KeyCode == GetKeyFromConfig(_config.ExitHotkey.Key))
            {
                Application.Exit();
            }
        }
        
        private Keys GetKeyFromConfig(string keyName)
        {
            // Try to parse the key name directly
            if (Enum.TryParse<Keys>(keyName, out var key))
            {
                return key;
            }
            
            // Fallback to F8 if parsing fails
            return Keys.F8;
        }
        
        private void OpenConfigWindow()
        {
            try
            {
                // Unregister current hotkeys before opening config
                UnregisterAllHotkeys();
                
                var configWindow = new ConfigWindow(_config, _configService);
                configWindow.ShowDialog();
                
                // Reload config after closing window
                var newConfig = _configService.LoadConfig();
                _config.X = newConfig.X;
                _config.Y = newConfig.Y;
                _config.Width = newConfig.Width;
                _config.Height = newConfig.Height;
                _config.OverlayColorArgb = newConfig.OverlayColorArgb;
                _config.OpacityWhenDetected = newConfig.OpacityWhenDetected;
                _config.EditModeHotkey = newConfig.EditModeHotkey;
                _config.ToggleHotkey = newConfig.ToggleHotkey;
                _config.ExitHotkey = newConfig.ExitHotkey;
                
                // Apply the new color immediately
                this.BackColor = _config.GetOverlayColor();
                
                // Re-register hotkeys with new configuration
                RegisterGlobalHotKeys();
                UpdateTransparency();
                
                Console.WriteLine($"Configuration window closed, settings reloaded. New color: {_config.GetOverlayColor()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening config window: {ex.Message}");
            }
        }
        
        private void UnregisterAllHotkeys()
        {
            try
            {
                UnregisterHotKey(this.Handle, ID_HOTKEY_EDITMODE);
                UnregisterHotKey(this.Handle, ID_HOTKEY_TOGGLE);
                UnregisterHotKey(this.Handle, ID_HOTKEY_EXIT);
                UnregisterHotKey(this.Handle, ID_HOTKEY_CONFIG);
                Console.WriteLine("Hotkeys unregistered");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unregistering hotkeys: {ex.Message}");
            }
        }
        
        private void ToggleCoverage()
        {
            try
            {
                _config.IsEnabled = !_config.IsEnabled;
                var status = _config.IsEnabled ? "ENABLED" : "DISABLED";
                var message = $"Coverage {status}";
                Console.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(message);
                
                // Update overlay visibility immediately
                if (!_config.IsEnabled)
                {
                    this.Opacity = 0.0; // Hide completely
                    this.TopMost = false; // Allow other windows to be on top
                }
                else
                {
                    this.TopMost = true; // Always on top when enabled
                    if (!_isEditMode)
                    {
                        this.Opacity = _config.OpacityWhenDetected; // Use configured opacity
                    }
                    else
                    {
                        this.Opacity = 0.3; // Edit mode opacity
                    }
                }
                
                // Show on screen overlay if in edit mode
                if (_isEditMode)
                {
                    this.Text = $"Coverage {status}";
                    this.Invalidate(); // Refresh to show updated status
                }
                
                // Save the setting
                _configService.SaveConfig(_config);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ToggleCoverage error: {ex.Message}");
            }
        }
        
        private void ToggleEditMode()
        {
            try
            {
                _isEditMode = !_isEditMode;
                UpdateTransparency();
                this.Invalidate();
                Console.WriteLine($"Edit mode: {_isEditMode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ToggleEditMode error: {ex.Message}");
                // Ensure we're in a valid state
                _isEditMode = false;
                UpdateTransparency();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_isEditMode)
            {
                using (var pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
                
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Arial", 10))
                {
                    var rect = _config.GetRectangle();
                    var status = _config.IsEnabled ? "ENABLED" : "DISABLED";
                    var info = $"EDIT MODE - Drag to move, corners to resize\nPosition: {rect.X},{rect.Y} Size: {rect.Width}x{rect.Height}\nCoverage: {status}\nF9: Toggle Coverage | F11: Show Position | F12: Set Default | F10: Exit";
                    e.Graphics.DrawString(info, font, brush, 10, 10);
                }
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_isEditMode) return;
            
            try
            {
                _dragStartPoint = e.Location;
                _dragStartRect = this.Bounds;
                
                var resizeMode = GetResizeMode(e.Location);
                if (resizeMode != ResizeMode.None)
                {
                    _isResizing = true;
                    _resizeMode = resizeMode;
                }
                else
                {
                    _isDragging = true;
                }
                
                this.Capture = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnMouseDown error: {ex.Message}");
            }
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isEditMode)
            {
                UpdateCursor(e.Location);
                return;
            }
            
            try
            {
                if (_isDragging)
                {
                    var deltaX = e.X - _dragStartPoint.X;
                    var deltaY = e.Y - _dragStartPoint.Y;
                    
                    // Calculate new position with bounds checking
                    var newX = _dragStartRect.X + deltaX;
                    var newY = _dragStartRect.Y + deltaY;
                    
                    // Keep within screen bounds
                    var screenBounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                    newX = Math.Max(0, Math.Min(newX, screenBounds.Width - this.Width));
                    newY = Math.Max(0, Math.Min(newY, screenBounds.Height - this.Height));
                    
                    this.Location = new Point(newX, newY);
                }
                else if (_isResizing)
                {
                    ResizeWindow(e.Location);
                }
                else
                {
                    UpdateCursor(e.Location);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnMouseMove error: {ex.Message}");
                // Reset state on error
                _isDragging = false;
                _isResizing = false;
                this.Capture = false;
            }
        }
        
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_isDragging || _isResizing)
            {
                _config.SetRectangle(this.Bounds);
                _configService.SaveConfig(_config);
            }
            
            _isDragging = false;
            _isResizing = false;
            _resizeMode = ResizeMode.None;
            this.Capture = false;
        }
        
        private ResizeMode GetResizeMode(Point point)
        {
            const int cornerSize = 10;
            
            if (point.X <= cornerSize && point.Y <= cornerSize) return ResizeMode.TopLeft;
            if (point.X >= this.Width - cornerSize && point.Y <= cornerSize) return ResizeMode.TopRight;
            if (point.X <= cornerSize && point.Y >= this.Height - cornerSize) return ResizeMode.BottomLeft;
            if (point.X >= this.Width - cornerSize && point.Y >= this.Height - cornerSize) return ResizeMode.BottomRight;
            
            return ResizeMode.None;
        }
        
        private void UpdateCursor(Point point)
        {
            if (!_isEditMode)
            {
                this.Cursor = Cursors.Default;
                return;
            }
            
            var resizeMode = GetResizeMode(point);
            this.Cursor = resizeMode switch
            {
                ResizeMode.TopLeft => Cursors.SizeNWSE,
                ResizeMode.TopRight => Cursors.SizeNESW,
                ResizeMode.BottomLeft => Cursors.SizeNESW,
                ResizeMode.BottomRight => Cursors.SizeNWSE,
                _ => Cursors.SizeAll
            };
        }
        
        private void ResizeWindow(Point point)
        {
            try
            {
                var deltaX = point.X - _dragStartPoint.X;
                var deltaY = point.Y - _dragStartPoint.Y;
                
                var newRect = _dragStartRect;
                
                switch (_resizeMode)
                {
                    case ResizeMode.TopLeft:
                        newRect.X += deltaX;
                        newRect.Y += deltaY;
                        newRect.Width -= deltaX;
                        newRect.Height -= deltaY;
                        break;
                    case ResizeMode.TopRight:
                        newRect.Y += deltaY;
                        newRect.Width += deltaX;
                        newRect.Height -= deltaY;
                        break;
                    case ResizeMode.BottomLeft:
                        newRect.X += deltaX;
                        newRect.Width -= deltaX;
                        newRect.Height += deltaY;
                        break;
                    case ResizeMode.BottomRight:
                        newRect.Width += deltaX;
                        newRect.Height += deltaY;
                        break;
                }
                
                // Validate minimum size only
                if (newRect.Width > 50 && newRect.Height > 50)
                {
                    // Keep within screen bounds but don't limit maximum size too much
                    var screenBounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                    newRect.X = Math.Max(0, Math.Min(newRect.X, screenBounds.Width - newRect.Width));
                    newRect.Y = Math.Max(0, Math.Min(newRect.Y, screenBounds.Height - newRect.Height));
                    newRect.Width = Math.Min(newRect.Width, screenBounds.Width - newRect.X);
                    newRect.Height = Math.Min(newRect.Height, screenBounds.Height - newRect.Y);
                    
                    this.SetBounds(newRect.X, newRect.Y, newRect.Width, newRect.Height);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ResizeWindow error: {ex.Message}");
            }
        }
        
        private void ShowCurrentPosition()
        {
            try
            {
                var rect = _config.GetRectangle();
                var message = $"=== CURRENT POSITION ===\nX: {rect.X}\nY: {rect.Y}\nWidth: {rect.Width}\nHeight: {rect.Height}\n========================";
                Console.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(message);
                
                // Also show on screen overlay if in edit mode
                if (_isEditMode)
                {
                    this.Text = $"X:{rect.X} Y:{rect.Y} W:{rect.Width} H:{rect.Height}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ShowCurrentPosition error: {ex.Message}");
            }
        }
        
        private void SetAsDefault()
        {
            try
            {
                // Save current position as default
                _configService.SaveConfig(_config);
                var rect = _config.GetRectangle();
                var message = $"Set as default: X={rect.X}, Y={rect.Y}, Width={rect.Width}, Height={rect.Height}";
                Console.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(message);
                
                // Also show on screen overlay if in edit mode
                if (_isEditMode)
                {
                    this.Text = "DEFAULT SAVED!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SetAsDefault error: {ex.Message}");
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unregister global hotkeys
                UnregisterHotKey(this.Handle, ID_HOTKEY_EDITMODE);
                UnregisterHotKey(this.Handle, ID_HOTKEY_TOGGLE);
                UnregisterHotKey(this.Handle, ID_HOTKEY_EXIT);
                UnregisterHotKey(this.Handle, ID_HOTKEY_CONFIG);
            }
            base.Dispose(disposing);
        }
        
        private enum ResizeMode
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
    }
}
