using ChatBlocker.Models;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ChatBlocker.Services
{
    public class ConfigService
    {
        private readonly string _configPath;
        
        public ConfigService()
        {
            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChatBlocker");
            Directory.CreateDirectory(appDataPath);
            _configPath = Path.Combine(appDataPath, "config.json");
        }
        
        public Config LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    var config = JsonConvert.DeserializeObject<Config>(json) ?? new Config();
                    
                    // ALWAYS apply user's perfect position, override existing config
                    var currentScreenBounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                    config.InitializeForScreen(currentScreenBounds.Width, currentScreenBounds.Height);
                    
                    // Validate that the position is within screen bounds
                    if (config.Width <= 0 || config.Height <= 0 || 
                        config.X + config.Width > currentScreenBounds.Width || 
                        config.Y + config.Height > currentScreenBounds.Height)
                    {
                        Console.WriteLine("Config has invalid bounds, reinitializing for current screen");
                        config.InitializeForScreen(currentScreenBounds.Width, currentScreenBounds.Height);
                    }
                    
                    return config;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }
            
            // Create new config with user's perfect position
            var newConfig = new Config();
            var screenBounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
            newConfig.InitializeForScreen(screenBounds.Width, screenBounds.Height);
            return newConfig;
        }
        
        public void SaveConfig(Config config)
        {
            try
            {
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }
    }
}
