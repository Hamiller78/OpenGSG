using System;
using System.Drawing;
using System.Windows.Forms;
using ColdWarGameLogic.GameLogic;

namespace ColdWarPrototype
{
    /// <summary>
    /// Debug console for runtime game manipulation.
    /// Supports commands like: event <eventId> <countryTag>
    /// </summary>
    public partial class DebugConsole : Form
    {
        private readonly MasterController _gameController;
        private TextBox _outputBox;
        private TextBox _inputBox;

        public DebugConsole(MasterController gameController)
        {
            _gameController =
                gameController ?? throw new ArgumentNullException(nameof(gameController));
            SetupConsole();

            // Start hidden
            this.Visible = false;
        }

        private void SetupConsole()
        {
            this.Text = "Debug Console";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.TopMost = true; // Stay on top

            // Output box (read-only)
            _outputBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.None,
            };
            this.Controls.Add(_outputBox);

            // Input box at bottom
            _inputBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.FixedSingle,
            };
            _inputBox.KeyDown += InputBox_KeyDown;
            this.Controls.Add(_inputBox);

            // Welcome message
            WriteOutput("=== OpenGSG Debug Console ===");
            WriteOutput("Commands:");
            WriteOutput("  event <eventId> <countryTag> - Fire an event");
            WriteOutput("  help - Show available commands");
            WriteOutput("  clear - Clear console output");
            WriteOutput("");
        }

        private void InputBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string command = _inputBox.Text.Trim();
                if (!string.IsNullOrEmpty(command))
                {
                    WriteOutput($"> {command}");
                    ExecuteCommand(command);
                    _inputBox.Clear();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
        }

        private void ExecuteCommand(string command)
        {
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            var cmd = parts[0].ToLower();

            switch (cmd)
            {
                case "help":
                    WriteOutput("Available commands:");
                    WriteOutput("  event <eventId> <countryTag> - Fire an event for a country");
                    WriteOutput("  clear - Clear console output");
                    WriteOutput("  help - Show this help");
                    break;

                case "clear":
                    _outputBox.Clear();
                    break;

                case "event":
                    if (parts.Length < 3)
                    {
                        WriteOutput("ERROR: Usage: event <eventId> <countryTag>");
                        WriteOutput("Example: event acheson_speech.1 USA");
                    }
                    else
                    {
                        string eventId = parts[1];
                        string countryTag = parts[2];
                        FireEvent(eventId, countryTag);
                    }
                    break;

                default:
                    WriteOutput(
                        $"ERROR: Unknown command '{cmd}'. Type 'help' for available commands."
                    );
                    break;
            }
        }

        private void FireEvent(string eventId, string countryTag)
        {
            try
            {
                bool success = _gameController.TickHandler.FireEventDebug(eventId, countryTag);

                if (success)
                {
                    WriteOutput($"SUCCESS: Fired event '{eventId}' for country '{countryTag}'");
                }
                else
                {
                    WriteOutput($"ERROR: Failed to fire event '{eventId}' for '{countryTag}'");
                    WriteOutput("  Possible reasons:");
                    WriteOutput("  - Event ID does not exist");
                    WriteOutput("  - Country tag does not exist");
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"ERROR: Exception while firing event: {ex.Message}");
            }
        }

        private void WriteOutput(string text)
        {
            if (_outputBox.InvokeRequired)
            {
                _outputBox.BeginInvoke(() => WriteOutput(text));
                return;
            }

            _outputBox.AppendText(text + Environment.NewLine);
            _outputBox.SelectionStart = _outputBox.Text.Length;
            _outputBox.ScrollToCaret();
        }

        /// <summary>
        /// Shows or focuses the console window.
        /// </summary>
        public new void Show()
        {
            base.Show();
            this.BringToFront();
            _inputBox.Focus();
        }

        /// <summary>
        /// Toggles console visibility.
        /// </summary>
        public void Toggle()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
    }
}
