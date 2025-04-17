using System.Windows.Input;

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

        // ===== Phase Management =====
        private bool awaitingFinalColourAfterLastRed = false;
        private bool inFinalColourSequence = false;
        private readonly List<string> finalColourOrder = new() { "Yellow", "Green", "Brown", "Blue", "Pink", "Black" };
        private int finalColourIndex = 0;

        // ===== State Tracking =====
        private string lastBallPotted = "";
        public bool RedWasJustPotted { get; private set; } = false;

        // ===== Free Ball State =====
        private bool isFreeBallActive = false;
        private bool freeBallJustPotted = false;
        public bool IsFreeBallActive => isFreeBallActive;

        // ===== Free Ball Activation =====
        public void ActivateFreeBall()
        {
            SwitchPlayer();
            isFreeBallActive = true;
            freeBallJustPotted = false;
        }

        // ===== Potting Logic =====
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
                "FreeBall" => 0,
                _ => 0
            };

            if (ball == "FreeBall")
            {
                HandleFreeBallPot();
                return;
            }

            HandleRegularPot(ball, points);
        }

        private void HandleFreeBallPot()
        {
            freeBallJustPotted = true;
            isFreeBallActive = false;

            int awardedPoints = RemainingReds > 0 ? 1 : GetCurrentFinalColourPoints();
            AddPointsToCurrentPlayer(awardedPoints);
            CurrentBreak += awardedPoints;
        }

        private void HandleRegularPot(string ball, int points)
        {
            CurrentBreak += points;
            AddPointsToCurrentPlayer(points);

            if (ball == "Red")
            {
                RemainingReds--;
                RemainingPoints -= 8;
                RedWasJustPotted = true;

                if (RemainingReds == 0)
                    awaitingFinalColourAfterLastRed = true;
            }
            else if (awaitingFinalColourAfterLastRed)
            {
                awaitingFinalColourAfterLastRed = false;
                inFinalColourSequence = true;
                finalColourIndex = 0;
                RedWasJustPotted = false;
            }
            else if (inFinalColourSequence && ball == GetNextFinalColour())
            {
                if (!freeBallJustPotted)
                    RemainingPoints -= points;

                finalColourIndex++;
                RedWasJustPotted = false;
            }
            else if (!freeBallJustPotted && lastBallPotted != "Red")
            {
                RemainingPoints -= points;
                RedWasJustPotted = false;
            }
            else
            {
                RedWasJustPotted = false; // Colour after red — no deduction
            }

            lastBallPotted = ball;
            freeBallJustPotted = false;
        }

        private void AddPointsToCurrentPlayer(int points)
        {
            if (CurrentPlayer == 1)
                Player1Score += points;
            else
                Player2Score += points;
        }

        // ===== Rule Checking =====
        public bool CanPot(string ball)
        {
            if (freeBallJustPotted)
            {
                if (RemainingReds > 0)
                    return ball != "Red" && ball != "FreeBall";

                if (awaitingFinalColourAfterLastRed)
                    return ball != "Red" && ball != "FreeBall";

                if (inFinalColourSequence)
                    return ball == finalColourOrder[finalColourIndex];

                return false;
            }

            if (RemainingReds > 0)
                return ball == "Red" || lastBallPotted == "Red";

            if (awaitingFinalColourAfterLastRed)
                return ball != "Red";

            if (inFinalColourSequence && finalColourIndex < finalColourOrder.Count)
                return ball == finalColourOrder[finalColourIndex];

            return false;
        }

        // ===== Game Flow =====
        public void EndBreak()
        {
            if (awaitingFinalColourAfterLastRed)
            {
                awaitingFinalColourAfterLastRed = false;
                inFinalColourSequence = true;
                finalColourIndex = 0;
            }

            isFreeBallActive = false;
            freeBallJustPotted = false;
            RedWasJustPotted = false;
            CurrentBreak = 0;
            SwitchPlayer();
        }

        public void ResetMatch()
        {
            Player1Score = 0;
            Player2Score = 0;
            CurrentBreak = 0;
            RemainingPoints = 147;
            RemainingReds = 15;
            CurrentPlayer = 1;

            awaitingFinalColourAfterLastRed = false;
            inFinalColourSequence = false;
            finalColourIndex = 0;

            lastBallPotted = "";
            RedWasJustPotted = false;
            isFreeBallActive = false;
            freeBallJustPotted = false;
        }

        public void ApplyFoul(int foulPoints, bool switchTurn)
        {
            if (CurrentPlayer == 1)
                Player2Score += foulPoints;
            else
                Player1Score += foulPoints;

            if (awaitingFinalColourAfterLastRed)
            {
                awaitingFinalColourAfterLastRed = false;
                inFinalColourSequence = true;
                finalColourIndex = 0;
            }

            if (switchTurn)
                SwitchPlayer();

            isFreeBallActive = false;
            freeBallJustPotted = false;
            CurrentBreak = 0;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
        }

        // ===== Helpers =====
        public bool IsAwaitingFinalColourAfterLastRed() => awaitingFinalColourAfterLastRed;

        public string GetNextFinalColour()
        {
            if (inFinalColourSequence && finalColourIndex < finalColourOrder.Count)
                return finalColourOrder[finalColourIndex];

            return "";
        }

        public bool IsAwaitingColourAfterFreeBall()
        {
            return freeBallJustPotted;
        }

        private int GetCurrentFinalColourPoints()
        {
            if (inFinalColourSequence && finalColourIndex < finalColourOrder.Count)
            {
                string colour = finalColourOrder[finalColourIndex];
                return colour switch
                {
                    "Yellow" => 2,
                    "Green" => 3,
                    "Brown" => 4,
                    "Blue" => 5,
                    "Pink" => 6,
                    "Black" => 7,
                    _ => 0
                };
            }
            return 0;
        }
    }
}
