using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BreakMaster.Services;

namespace BreakMaster.ViewModels
{
    public class MainGameViewModel : INotifyPropertyChanged
    {
        // ===== Game Logic Service =====
        private readonly GameLogicService _logic = new();

        // ===== INotifyPropertyChanged Setup =====
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ===== Player 1 Score =====
        private int _player1Score;
        public int Player1Score
        {
            get => _player1Score;
            set { _player1Score = value; OnPropertyChanged(); }
        }

        // ===== Player 2 Score =====
        private int _player2Score;
        public int Player2Score
        {
            get => _player2Score;
            set { _player2Score = value; OnPropertyChanged(); }
        }

        // ===== Current Break =====
        private int _currentBreak;
        public int CurrentBreak
        {
            get => _currentBreak;
            set { _currentBreak = value; OnPropertyChanged(); }
        }

        // ===== Ball Visibility Flags =====
        private bool _isRedVisible = true;
        public bool IsRedVisible
        {
            get => _isRedVisible;
            set { _isRedVisible = value; OnPropertyChanged(); }
        }

        private bool _areColorsVisible = false;
        public bool AreColorsVisible
        {
            get => _areColorsVisible;
            set { _areColorsVisible = value; OnPropertyChanged(); }
        }

        // ===== Current Player Display =====
        private string _currentPlayerDisplay;
        public string CurrentPlayerDisplay
        {
            get => _currentPlayerDisplay;
            set { _currentPlayerDisplay = value; OnPropertyChanged(); }
        }

        // ===== Constructor =====
        public MainGameViewModel()
        {
            UpdatePlayerDisplay();
        }

        // ===== Potting Command =====
        public ICommand PotCommand => new Command<string>(OnPotBall);

        // ===== Potting Logic =====
        private void OnPotBall(string ballName)
        {
            // Apply points and game rules
            _logic.PotBall(ballName);

            // Update bound properties
            Player1Score = _logic.Player1Score;
            Player2Score = _logic.Player2Score;
            CurrentBreak = _logic.CurrentBreak;

            // Toggle red/colour visibility based on rules
            if (ballName == "Red")
            {
                IsRedVisible = false;
                AreColorsVisible = true;
            }
            else
            {
                IsRedVisible = true;
                AreColorsVisible = false;
            }
        }

        // ===== End Break Command =====
        public ICommand EndBreakCommand => new Command(OnEndBreak);

        private void OnEndBreak()
        {
            // End current player's break
            _logic.EndBreak();

            // Update properties
            CurrentBreak = _logic.CurrentBreak;
            Player1Score = _logic.Player1Score;
            Player2Score = _logic.Player2Score;

            // Reset visibility (ready for next red)
            IsRedVisible = true;
            AreColorsVisible = false;

            // Update player label
            UpdatePlayerDisplay();
        }

        // ===== Update Player Label =====
        private void UpdatePlayerDisplay()
        {
            CurrentPlayerDisplay = $"Player {_logic.CurrentPlayer}'s Turn";
        }
    }
}