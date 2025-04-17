using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BreakMaster.Services;

namespace BreakMaster.ViewModels
{
    public class MainGameViewModel : INotifyPropertyChanged
    {
        private readonly GameLogicService _logic = new();

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Player1Score => _logic.Player1Score;
        public int Player2Score => _logic.Player2Score;
        public int CurrentBreak => _logic.CurrentBreak;
        public string CurrentPlayerDisplay => $"Player {_logic.CurrentPlayer}'s Turn";
        public string RemainingPointsDisplay => $"Points Remaining: {_logic.RemainingPoints}";
        public string RemainingRedsDisplay => $"Reds Remaining: {_logic.RemainingReds}";

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

        private bool _isFreeBallVisible;
        public bool IsFreeBallVisible
        {
            get => _isFreeBallVisible;
            set { _isFreeBallVisible = value; OnPropertyChanged(); }
        }

        public MainGameViewModel()
        {
            UpdateAllBindings();
        }

        public ICommand PotCommand => new Command<string>(OnPotBall);

        private void OnPotBall(string ballName)
        {
            if (ballName == "FreeBall")
            {
                _logic.PotBall("FreeBall");
            }
            else
            {
                if (!_logic.CanPot(ballName)) return;
                _logic.PotBall(ballName);
            }

            UpdateAllBindings();
        }

        public ICommand EndBreakCommand => new Command(OnEndBreak);

        private void OnEndBreak()
        {
            _logic.EndBreak();
            UpdateAllBindings();
        }

        public ICommand ResetCommand => new Command(OnReset);

        private void OnReset()
        {
            _logic.ResetMatch();
            UpdateAllBindings();
        }

        public ICommand FoulCommand => new Command(async () => await HandleFoulAsync());

        private async Task HandleFoulAsync()
        {
            string pointsStr = await Application.Current.MainPage.DisplayActionSheet(
                "Select Foul Points Given to Opponent", "Cancel", null, "4", "5", "6", "7");

            if (pointsStr == "Cancel" || string.IsNullOrWhiteSpace(pointsStr)) return;

            int foulPoints = int.Parse(pointsStr);

            string decision = await Application.Current.MainPage.DisplayActionSheet(
                "What happens after the foul?", "Cancel", null,
                "Free Ball", "Play On (Opponent Plays)", "Force Continue (Same Player)");

            if (decision == "Cancel" || string.IsNullOrWhiteSpace(decision)) return;

            bool switchTurn = (decision == "Play On (Opponent Plays)");
            _logic.ApplyFoul(foulPoints, switchTurn);

            if (decision == "Free Ball")
            {
                _logic.ActivateFreeBall();
            }

            UpdateAllBindings();
        }

        private void UpdateAllBindings()
        {
            OnPropertyChanged(nameof(Player1Score));
            OnPropertyChanged(nameof(Player2Score));
            OnPropertyChanged(nameof(CurrentBreak));
            OnPropertyChanged(nameof(CurrentPlayerDisplay));
            OnPropertyChanged(nameof(RemainingPointsDisplay));
            OnPropertyChanged(nameof(RemainingRedsDisplay));

            // ===== Free Ball is Active =====
            if (_logic.IsFreeBallActive)
            {
                IsFreeBallVisible = true;

                if (_logic.RemainingReds > 0)
                {
                    // Awaiting pot of free ball - show Red and Free Ball only
                    IsRedVisible = false;
                    AreColorsVisible = false;
                    VisibleFinalColour = "";
                }
                else
                {
                    // Final sequence - only show legal colour
                    IsRedVisible = false;
                    AreColorsVisible = true;
                    VisibleFinalColour = _logic.GetNextFinalColour();
                }

                return;
            }

            // ===== After Free Ball was Potted (With Reds Remaining) =====
            if (_logic.RemainingReds > 0 && _logic.IsAwaitingColourAfterFreeBall())
            {
                IsFreeBallVisible = false;
                IsRedVisible = false;
                AreColorsVisible = true;
                VisibleFinalColour = "Any";
            }
            // ===== After Free Ball in Final Colours =====
            else if (_logic.IsAwaitingColourAfterFreeBall())
            {
                IsFreeBallVisible = false;
                IsRedVisible = false;
                AreColorsVisible = true;
                VisibleFinalColour = _logic.GetNextFinalColour();
            }
            // ===== Standard Play: Reds Phase =====
            else if (_logic.RemainingReds > 0)
            {
                IsFreeBallVisible = false;
                IsRedVisible = !_logic.RedWasJustPotted;
                AreColorsVisible = _logic.RedWasJustPotted;
                VisibleFinalColour = _logic.RedWasJustPotted ? "Any" : "";
            }
            // ===== After Last Red Potted, Awaiting Colour =====
            else if (_logic.IsAwaitingFinalColourAfterLastRed())
            {
                IsFreeBallVisible = false;
                IsRedVisible = false;
                AreColorsVisible = true;
                VisibleFinalColour = "Any";
            }
            // ===== Final Colour Sequence =====
            else
            {
                IsFreeBallVisible = false;
                IsRedVisible = false;
                AreColorsVisible = true;
                VisibleFinalColour = _logic.GetNextFinalColour();
            }

            System.Diagnostics.Debug.WriteLine($"Updated: RedVisible={IsRedVisible}, ColorsVisible={AreColorsVisible}, FinalColour={VisibleFinalColour}, FreeBallVisible={IsFreeBallVisible}");
        }





    }
}