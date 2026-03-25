using ChatBlocker.Models;
using ChatBlocker.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatBlocker.Forms
{
    public partial class ConfigWindow : Form
    {
        private readonly Config _config;
        private readonly ConfigService _configService;
        private Keys? _capturedKey = null;
        private bool _isCapturing = false;
        private Control _currentCaptureControl = null!;
        private Button _colorButton = null!;
        private Label _colorLabel = null!;
        private HotkeyConfig _currentHotkeyConfig = null!;
        
        public ConfigWindow(Config config, ConfigService configService)
        {
            _config = config;
            _configService = configService;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "ChatBlocker Configuration";
            this.Size = new Size(450, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += ConfigWindow_KeyDown;
            
            // Title label
            var titleLabel = new Label
            {
                Text = "ChatBlocker Configuration",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(410, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);
            
            // Color section
            var colorLabel = new Label
            {
                Text = "Rectangle Color:",
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                Location = new Point(20, 70),
                Size = new Size(150, 25)
            };
            this.Controls.Add(colorLabel);
            
            _colorButton = new Button
            {
                Text = "Choose Color",
                Font = new Font("Arial", 9),
                Location = new Point(20, 95),
                Size = new Size(100, 30),
                BackColor = _config.GetOverlayColor(),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _colorButton.Click += (s, e) => {
                var colorDialog = new ColorDialog
                {
                    Color = _config.GetOverlayColor(),
                    AllowFullOpen = true,
                    FullOpen = true
                };
                
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    _config.SetOverlayColor(colorDialog.Color);
                    _colorButton.BackColor = colorDialog.Color;
                    _colorLabel.Text = $"RGB({colorDialog.Color.R}, {colorDialog.Color.G}, {colorDialog.Color.B})";
                }
            };
            this.Controls.Add(_colorButton);
            
            _colorLabel = new Label
            {
                Text = $"RGB({_config.GetOverlayColor().R}, {_config.GetOverlayColor().G}, {_config.GetOverlayColor().B})",
                Font = new Font("Arial", 8),
                ForeColor = Color.LightGray,
                Location = new Point(130, 100),
                Size = new Size(150, 20)
            };
            this.Controls.Add(_colorLabel);
            
            // Opacity section
            var opacityLabel = new Label
            {
                Text = "Rectangle Opacity:",
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                Location = new Point(20, 140),
                Size = new Size(150, 25)
            };
            this.Controls.Add(opacityLabel);
            
            var opacityValueLabel = new Label
            {
                Text = $"{(int)(_config.OpacityWhenDetected * 100)}%",
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                Location = new Point(230, 170),
                Size = new Size(50, 25)
            };
            this.Controls.Add(opacityValueLabel);
            
            var opacityTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(_config.OpacityWhenDetected * 100),
                Location = new Point(20, 165),
                Size = new Size(200, 45),
                TickFrequency = 10
            };
            opacityTrackBar.ValueChanged += (s, e) => {
                _config.OpacityWhenDetected = opacityTrackBar.Value / 100.0;
                opacityValueLabel.Text = $"{opacityTrackBar.Value}%";
            };
            this.Controls.Add(opacityTrackBar);
            
            // Hotkeys section
            var hotkeysLabel = new Label
            {
                Text = "Hotkeys (Click to set):",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 220),
                Size = new Size(200, 25)
            };
            this.Controls.Add(hotkeysLabel);
            
            var instructionsLabel = new Label
            {
                Text = "Click on a hotkey field, then press the key you want",
                Font = new Font("Arial", 8),
                ForeColor = Color.LightGray,
                Location = new Point(20, 245),
                Size = new Size(400, 20)
            };
            this.Controls.Add(instructionsLabel);
            
            // Edit mode hotkey
            CreateHotkeyControl("Edit Mode:", _config.EditModeHotkey, 40, 270);
            
            // Toggle hotkey
            CreateHotkeyControl("Toggle:", _config.ToggleHotkey, 40, 310);
            
            // Exit hotkey
            CreateHotkeyControl("Exit:", _config.ExitHotkey, 40, 350);
            
            // Position info
            var positionLabel = new Label
            {
                Text = $"Current Position: X={_config.X}, Y={_config.Y}, W={_config.Width}, H={_config.Height}",
                Font = new Font("Arial", 9),
                ForeColor = Color.LightGray,
                Location = new Point(20, 400),
                Size = new Size(410, 40)
            };
            this.Controls.Add(positionLabel);
            
            // Instructions
            var instructionsLabel2 = new Label
            {
                Text = "Instructions:\n• Use Edit Mode to position the rectangle\n• Press F11 in Edit Mode to see position\n• Press F12 to save position as default\n• Click hotkey fields to set new keys",
                Font = new Font("Arial", 8),
                ForeColor = Color.LightGray,
                Location = new Point(20, 450),
                Size = new Size(410, 80)
            };
            this.Controls.Add(instructionsLabel2);
            
            // Buttons
            var resetButton = new Button
            {
                Text = "Reset All",
                Font = new Font("Arial", 10),
                Location = new Point(20, 540),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            resetButton.Click += (s, e) => {
                var result = MessageBox.Show(
                    "Are you sure you want to reset ALL settings to default values?\n\nThis will reset:\n• Position and size\n• Color and opacity\n• All hotkeys\n• Other settings",
                    "Reset to Defaults",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                
                if (result == DialogResult.Yes)
                {
                    _config.ResetToDefaults();
                    
                    // Update UI
                    _colorButton.BackColor = _config.GetOverlayColor();
                    _colorLabel.Text = $"RGB({_config.GetOverlayColor().R}, {_config.GetOverlayColor().G}, {_config.GetOverlayColor().B})";
                    opacityTrackBar.Value = (int)(_config.OpacityWhenDetected * 100);
                    opacityValueLabel.Text = $"{(int)(_config.OpacityWhenDetected * 100)}%";
                    
                    // Update hotkey displays
                    UpdateHotkeyDisplays();
                    
                    MessageBox.Show("All settings have been reset to defaults!", "Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            this.Controls.Add(resetButton);
            
            var cancelButton = new Button
            {
                Text = "Cancel",
                Font = new Font("Arial", 10),
                Location = new Point(160, 540),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.Click += (s, e) => this.Close();
            this.Controls.Add(cancelButton);
            
            var saveButton = new Button
            {
                Text = "Save & Close",
                Font = new Font("Arial", 10),
                Location = new Point(300, 540),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.Click += (s, e) => {
                _configService.SaveConfig(_config);
                MessageBox.Show("Configuration saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            };
            this.Controls.Add(saveButton);
            
            this.ResumeLayout(false);
        }
        
        private void UpdateHotkeyDisplays()
        {
            // This would need to be implemented to update the hotkey text boxes
            // For now, they will be updated when the form is reopened
        }
        
        private void CreateHotkeyControl(string labelText, HotkeyConfig hotkeyConfig, int x, int y)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Arial", 9),
                ForeColor = Color.White,
                Location = new Point(x, y),
                Size = new Size(80, 20)
            };
            this.Controls.Add(label);
            
            var hotkeyTextBox = new TextBox
            {
                Text = GetHotkeyDisplayText(hotkeyConfig),
                Font = new Font("Arial", 9),
                Location = new Point(x + 90, y - 2),
                Size = new Size(120, 25),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Tag = labelText // Use the label text as identifier
            };
            hotkeyTextBox.Click += (s, e) => StartKeyCapture(hotkeyTextBox, hotkeyConfig);
            this.Controls.Add(hotkeyTextBox);
            
            var clearButton = new Button
            {
                Text = "Clear",
                Font = new Font("Arial", 8),
                Location = new Point(x + 220, y - 2),
                Size = new Size(50, 25),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            clearButton.Click += (s, e) => {
                hotkeyConfig.Key = "";
                hotkeyConfig.Ctrl = false;
                hotkeyConfig.Alt = false;
                hotkeyConfig.Shift = false;
                hotkeyTextBox.Text = "Click to set";
            };
            this.Controls.Add(clearButton);
        }
        
        private void StartKeyCapture(TextBox textBox, HotkeyConfig hotkeyConfig)
        {
            _isCapturing = true;
            _currentCaptureControl = textBox;
            _currentHotkeyConfig = hotkeyConfig; // Store direct reference to the HotkeyConfig
            textBox.Text = "Press any key...";
            textBox.BackColor = Color.FromArgb(70, 70, 70);
        }
        
        private void ConfigWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Console.WriteLine($"=== KeyDown Event Triggered ===");
                Console.WriteLine($"_isCapturing: {_isCapturing}");
                Console.WriteLine($"_currentCaptureControl: {_currentCaptureControl?.GetType().Name ?? "null"}");
                Console.WriteLine($"_currentHotkeyConfig: {_currentHotkeyConfig?.GetType().Name ?? "null"}");
                
                if (!_isCapturing || _currentCaptureControl == null || _currentHotkeyConfig == null) 
                {
                    Console.WriteLine("Returning early - not capturing or missing references");
                    return;
                }
                
                Console.WriteLine($"Key pressed: {e.KeyCode}, Ctrl: {e.Control}, Alt: {e.Alt}, Shift: {e.Shift}");
                
                // Don't capture modifier keys alone
                if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu || 
                    e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin) 
                {
                    Console.WriteLine("Returning early - modifier key only");
                    return;
                }
                
                // Update the hotkey directly using the stored reference - no coordinate matching needed!
                Console.WriteLine($"Before update: {_currentHotkeyConfig.Key}");
                _currentHotkeyConfig.Key = e.KeyCode.ToString();
                _currentHotkeyConfig.Ctrl = e.Control;
                _currentHotkeyConfig.Alt = e.Alt;
                _currentHotkeyConfig.Shift = e.Shift;
                Console.WriteLine($"After update: {_currentHotkeyConfig.Key}");
                
                var displayText = GetHotkeyDisplayText(_currentHotkeyConfig);
                _currentCaptureControl.Text = displayText;
                _currentCaptureControl.BackColor = Color.FromArgb(50, 50, 50);
                
                Console.WriteLine($"Hotkey set successfully: {displayText}");
                
                _isCapturing = false;
                _currentCaptureControl = null;
                _currentHotkeyConfig = null;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in KeyDown: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error setting hotkey: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                _isCapturing = false;
                _currentCaptureControl = null;
                _currentHotkeyConfig = null;
                e.Handled = true;
            }
        }
        
        private string GetHotkeyDisplayText(HotkeyConfig hotkeyConfig)
        {
            if (string.IsNullOrEmpty(hotkeyConfig.Key))
                return "Click to set";
            
            var parts = new System.Collections.Generic.List<string>();
            
            if (hotkeyConfig.Ctrl) parts.Add("Ctrl");
            if (hotkeyConfig.Alt) parts.Add("Alt");
            if (hotkeyConfig.Shift) parts.Add("Shift");
            
            parts.Add(hotkeyConfig.Key);
            
            return string.Join(" + ", parts);
        }
    }
}
