# BreakMaster


### 📅 Changelog

#### 17/04/25:
- Initial project structure created in .NET MAUI
- MVVM architecture implemented
- Added ball potting buttons (Red + Colors)
- Implemented red/colour shot flow logic
- Current player display now shows on screen
- End Break logic switches player and resets break
- Bound to GitHub with first few commits
- Fixed logic to correctly transition from final red + colour into the final colour sequence. #NOT WORKING
- Updated ViewModel to recheck and display correct colour visibility after final red phase. #NOT WORKING
- Added support to show all colours briefly after final red is potted, allowing any colour to be chosen before moving onto the final colours. #NOT WORKING
- Fix: Prevent double deduction of points after final red when break ends
- Ensured RemainingPoints only updates inside PotBall().
- Final colour sequence now starts with accurate remaining points.
##### Free Ball System (Fully Implemented)
- Activated after a foul when selected via popup.
- Scoring based on phase:
  - 1 point if reds are still in play
  - Correct value of the next required colour if in the final sequence
- Control switches to the Free Ball player automatically
- Allows valid follow-up shot after Free Ball pot
- Prevents invalid Free Ball scenarios (e.g., potting red or another Free Ball)
- Red button hidden during Free Ball, correct colour buttons shown
- Automatically cleans up Free Ball state after potting
##### Foul Handling
- Foul popup allows selecting points awarded to opponent (4–7)
- Decision popup for:
  - Free Ball
  - Play On (switch turn)
  - Force Continue (same player continues)
- Foul updates game state, switches player if applicable, and resets current break
##### Reset Match Functionality
- Clears all game state: scores, breaks, remaining reds, points, and resets sequence flags
- ViewModel refreshes UI and visibility states cleanly
##### Other Improvements
- Refactored GameLogicService for clarity and maintainability
- Improved visibility handling in ViewModel and converters for each game phase
- Fixed bug where extra points were subtracted after final colours