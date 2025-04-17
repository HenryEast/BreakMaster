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

        // ===== Colour Sequence After Reds =====
        private readonly List<string> finalColourOrder = new()
        {
            "Yellow", "Green", "Brown", "Blue", "Pink", "Black"
        };
        private int finalColourIndex = 0;

        // ===== Tracking =====
        private string lastBallPotted = "";
        public bool RedWasJustPotted { get; private set; } = false;

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

            if (CurrentPlayer == 1)
                Player1Score += points;
            else
                Player2Score += points;

            // ===== Handle Logic by Ball Type =====
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
                // Colour potted after final red
                awaitingFinalColourAfterLastRed = false;
                inFinalColourSequence = true;
                finalColourIndex = 0;
                RemainingPoints -= 8;
                RedWasJustPotted = false;
            }
            else if (inFinalColourSequence && ball == GetNextFinalColour())
            {
                RemainingPoints -= points;
                finalColourIndex++;
                RedWasJustPotted = false;
            }
            else if (lastBallPotted != "Red")
            {
                RemainingPoints -= points;
                RedWasJustPotted = false;
            }
            else
            {
                // Colour potted after a red (before final red) — no deduction
                RedWasJustPotted = false;
            }

            lastBallPotted = ball;
        }

        // ===== Validate Potting Option =====
        public bool CanPot(string ball)
        {
            if (RemainingReds > 0)
            {
                return ball == "Red" || lastBallPotted == "Red";
            }

            if (awaitingFinalColourAfterLastRed)
            {
                return ball != "Red"; // Any colour allowed
            }

            if (inFinalColourSequence && finalColourIndex < finalColourOrder.Count)
            {
                return ball == finalColourOrder[finalColourIndex];
            }

            return false;
        }

        // ===== End the Current Break =====
        public void EndBreak()
        {
            // If the final red was potted but colour missed
            if (awaitingFinalColourAfterLastRed)
            {
                awaitingFinalColourAfterLastRed = false;
                inFinalColourSequence = true;
                finalColourIndex = 0;
                // RemainingPoints -= 8; // Pevious temp fix # Don't deduct points again — already done when red was potted
            }

            RedWasJustPotted = false; // Reset so red is visible again (if reds remain)

            CurrentBreak = 0;
            CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
        }

        // ===== Expose State to ViewModel =====
        public bool IsAwaitingFinalColourAfterLastRed() => awaitingFinalColourAfterLastRed;

        public string GetNextFinalColour()
        {
            if (inFinalColourSequence && finalColourIndex < finalColourOrder.Count)
                return finalColourOrder[finalColourIndex];

            return "";
        }

        // ===== Reset Full Match State =====
        public void ResetMatch()
        {
            Player1Score = 0;
            Player2Score = 0;
            CurrentBreak = 0;
            CurrentPlayer = 1;
            RemainingPoints = 147;
            RemainingReds = 15;

            awaitingFinalColourAfterLastRed = false;
            inFinalColourSequence = false;
            finalColourIndex = 0;
            lastBallPotted = "";
            RedWasJustPotted = false;
        }
    }
}
