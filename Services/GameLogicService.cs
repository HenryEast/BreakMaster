namespace BreakMaster.Services
{
    public class GameLogicService
    {
        // ===== Core Game State =====
        public int Player1Score { get; private set; } = 0;
        public int Player2Score { get; private set; } = 0;
        public int CurrentBreak { get; private set; } = 0;
        public int RemainingPoints { get; private set; } = 147;
        public int RemainingReds { get; private set; } = 15;

        public int CurrentPlayer { get; private set; } = 1;

        // ===== Pot a Ball =====
        public void PotBall(string ball)
        {
            int points = ball switch
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
            RemainingPoints -= points;

            // Update score for the current player
            if (CurrentPlayer == 1)
                Player1Score += points;
            else
                Player2Score += points;
        }

        // ===== End the Current Break =====
        public void EndBreak()
        {
            CurrentBreak = 0;
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
        }

        // ===== Reset All Match State =====
        public void ResetMatch()
        {
            Player1Score = 0;
            Player2Score = 0;
            CurrentBreak = 0;
            CurrentPlayer = 1;
            RemainingPoints = 147;
            RemainingReds = 15;
        }
    }
}