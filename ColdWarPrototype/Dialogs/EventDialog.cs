using System;
using System.Drawing;
using System.Windows.Forms;
using ColdWarGameLogic.GameLogic;
using ColdWarGameLogic.GameWorld;
using OpenGSGLibrary.Events;

namespace ColdWarPrototype.Dialogs
{
    /// <summary>
    /// WinForms dialog for displaying game events to the player.
    /// Acts as an adapter between GameEvent model and the UI controls.
    /// </summary>
    public partial class EventDialog : UserControl
    {
        private GameEvent? _currentEvent;
        private EventEvaluationContext? _currentContext;
        private Action? _onEventCompleted;

        public EventDialog()
        {
            InitializeComponent();

            // Hide option buttons by default
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;

            // Wire up event handlers
            button1.Click += OptionButton_Click;
            button2.Click += OptionButton_Click;
            button3.Click += OptionButton_Click;
        }

        /// <summary>
        /// Displays an event to the player.
        /// </summary>
        /// <param name="gameEvent">The event to display.</param>
        /// <param name="context">Context for executing event effects.</param>
        /// <param name="onCompleted">Callback to invoke when player selects an option.</param>
        public void ShowEvent(
            GameEvent gameEvent,
            EventEvaluationContext context,
            Action onCompleted
        )
        {
            _currentEvent = gameEvent ?? throw new ArgumentNullException(nameof(gameEvent));
            _currentContext = context ?? throw new ArgumentNullException(nameof(context));
            _onEventCompleted = onCompleted;

            // Set event title and description
            // TODO: Implement localization lookup for Title/Description keys
            EventTextBox.Clear();
            EventTextBox.AppendText($"{gameEvent.Title}\n\n");
            EventTextBox.AppendText(gameEvent.Description);

            // Load event picture
            LoadEventPicture(gameEvent.Picture);

            // Setup option buttons
            SetupOptionButtons(gameEvent.Options);

            // Show the dialog
            this.Visible = true;
            this.BringToFront();
        }

        /// <summary>
        /// Loads the event picture from resources or file system.
        /// </summary>
        private void LoadEventPicture(string pictureName)
        {
            if (string.IsNullOrEmpty(pictureName))
            {
                EventPictureBox.Image = null;
                return;
            }

            try
            {
                var eventImagePath = Path.Combine(
                    MasterController.GAMEDATA_PATH,
                    "gfx",
                    "event_pictures",
                    pictureName + ".png"
                );

                //Load image from file
                Image? image = Image.FromFile(eventImagePath);

                if (image != null)
                {
                    EventPictureBox.Image = image;
                }
                else
                {
                    EventPictureBox.Image = null;
                }
            }
            catch
            {
                EventPictureBox.Image = null;
            }
        }

        /// <summary>
        /// Sets up the option buttons based on event options.
        /// </summary>
        private void SetupOptionButtons(List<EventOption> options)
        {
            var buttons = new[] { button1, button2, button3 };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < options.Count)
                {
                    buttons[i].Text = options[i].Name; // TODO: Localization
                    buttons[i].Tag = options[i];
                    buttons[i].Visible = true;
                }
                else
                {
                    buttons[i].Visible = false;
                    buttons[i].Tag = null;
                }
            }
        }

        /// <summary>
        /// Handles option button clicks.
        /// </summary>
        private void OptionButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag is not EventOption option)
                return;

            if (_currentContext == null)
                return;

            // Execute the selected option's effects
            option.Execute(_currentContext);

            // Hide the dialog
            this.Visible = false;

            // Notify completion
            _onEventCompleted?.Invoke();

            // Clear state
            _currentEvent = null;
            _currentContext = null;
            _onEventCompleted = null;
        }
    }
}
