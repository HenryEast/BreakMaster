# BreakMaster


## 📅 Changelog

### 17/04/25:
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
