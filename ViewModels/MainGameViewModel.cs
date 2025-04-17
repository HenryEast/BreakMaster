using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BreakMaster.ViewModels
{
    public class MainGameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _player1Score;
        public int Player1Score
        {
            get => _player1Score;
            set { _player1Score = value; OnPropertyChanged(); }
        }

        private int _player2Score;
        public int Player2Score
        {
            get => _player2Score;
            set { _player2Score = value; OnPropertyChanged(); }
        }

        private int _currentBreak;
        public int CurrentBreak
        {
            get => _currentBreak;
            set { _currentBreak = value; OnPropertyChanged(); }
        }

        public ICommand PotCommand => new Command<string>(OnPotBall);

        private void OnPotBall(string ballType)
        {
            int points = ballType switch
            {
                "Red" => 1,
                "Yellow" => 2,
                "Green" => 3,
                "Brown" => 4,
                "Blue" => 5,
                "Pink" => 6,
                "Black" => 7,
                _ => 0
            };

            CurrentBreak += points;
            Player1Score += points; // Later: switch player logic
        }
    }
}
