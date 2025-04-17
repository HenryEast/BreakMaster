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
        public int Player1Score => _logic.Player1Score;

        // ===== Player 2 Score =====
        public int Player2Score => _logic.Player2Score;

        // ===== Current Break =====
        public int CurrentBreak => _logic.CurrentBreak;

        // ===== Current Player Label =====
        public string CurrentPlayerDisplay => $"Player {_logic.CurrentPlayer}'s Turn";

        // ===== Remaining Points =====
        public string RemainingPointsDisplay =>
            $"Points Remaining: {_logic.RemainingPoints}";

        // ===== Remaining Reds =====
        public string RemainingRedsDisplay =>
            $"Reds Remaining: {_logic.RemainingReds}";

        // ===== Ball Visibility Flags =====
        private bool _isRedVisible;
        public bool IsRedVisible
        {
            get => _isRedVisible;
            set { _isRedVisible = value; OnPropertyChanged(); }
        }

        private bool _areColorsVisible;
        public bool AreColorsVisible
        {
            get => _areColorsVisible;
            set { _areColorsVisible = value; OnPropertyChanged(); }
        }

        private string _visibleFinalColour;
        public string VisibleFinalColour
        {
            get => _visibleFinalColour;
            set { _visibleFinalColour = value; OnPropertyChanged(); }
        }

        // ===== Constructor =====
        public MainGameViewModel()
        {
            UpdateAllBindings();
        }

        // ===== Pot Command =====
        public ICommand PotCommand => new Command<string>(OnPotBall);

        private void OnPotBall(string ballName)
        {
            if (!_logic.CanPot(ballName)) return;

            _logic.PotBall(ballName);
            UpdateAllBindings();
        }

        // ===== End Break Command =====
        public ICommand EndBreakCommand => new Command(OnEndBreak);

        private void OnEndBreak()
        {
            _logic.EndBreak();
            UpdateAllBindings();
        }

        // ===== Update All Bindings =====
        private void UpdateAllBindings()
        {
            OnPropertyChanged(nameof(Player1Score));
            OnPropertyChanged(nameof(Player2Score));
            OnPropertyChanged(nameof(CurrentBreak));
            OnPropertyChanged(nameof(CurrentPlayerDisplay));
            OnPropertyChanged(nameof(RemainingPointsDisplay));
            OnPropertyChanged(nameof(RemainingRedsDisplay));

            // ===== Visibility Logic =====
            if (_logic.RemainingReds > 0)
            {
                IsRedVisible = !_logic.RedWasJustPotted;
                AreColorsVisible = _logic.RedWasJustPotted;
                VisibleFinalColour = ""; // Not used yet
            }
            else if (_logic.IsAwaitingFinalColourAfterLastRed())
            {
                IsRedVisible = false;
                AreColorsVisible = true;
                VisibleFinalColour = "Any"; // All colours visible
            }
            else
            {
                IsRedVisible = false;
                AreColorsVisible = false;
                VisibleFinalColour = _logic.GetNextFinalColour();
            }

            System.Diagnostics.Debug.WriteLine($"Updated: RedVisible={IsRedVisible}, ColorsVisible={AreColorsVisible}, FinalColour={VisibleFinalColour}");
        }
    }
}
