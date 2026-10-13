# Project Poker

Open this folder with Unity **6000.5.4f1**.

## Scenes

- `Assets/Scenes/PokerTable.unity`: press Play to deal one round to three players, display their cards and the board, and log all winners. The GameController can play subsequent rounds through `PlayRound()`.
- `Assets/Scenes/PokerSimulation.unity`: editor-only seeded simulation. Configure player names, seats, hand count, and seed on PokerSimulationRunner. It logs category totals without updating the table display. This scene is not included in player builds.

## Structure

- `Assets/Scripts/Model`: plain C# cards, deck, players, seating, hand evaluation, and round results.
- `Assets/Scripts/Runtime`: Unity game controller and presentation.
- `Assets/Scripts/Debug`: editor-only simulation runner.
- `Assets/Art/Cards`: source artwork, imported sprites, and the CardSprites asset. The asset retains the original 52 serialized suit/rank/sprite assignments and builds its lookup directly from them.
- `Assets/Prefabs/UI`: card, player-hand, and board prefabs.
- `Assets/Settings`: rendering assets and profiles.
- `Tests/HandEvaluation`: standalone model regression checks.

Player stores identity. PokerRound owns dealing and returns immutable snapshots of board cards, each player's hole cards and rank, and all tied winners. The view consumes those results. Previous results remain valid after another round.

## Tests

Install the .NET 10 SDK, then run:

```sh
dotnet run --project Tests/HandEvaluation/HandEvaluation.csproj
```

The test project compiles all actual model sources without Unity or external test packages. It checks hand categories, tie breaks, invalid inputs, deck integrity, and round dealing.

## Dependencies and asset moves

URP, Unity UI, and Input System remain in use by the scene. Editor tooling and Unity's built-in modules remain installed. Unused inference, navigation, multiplayer-center, timeline, and visual-scripting packages have been removed from the manifest and unreachable lock entries removed. Unity may normalize the lock file on its next package resolution.

Move Unity assets together with their `.meta` files to preserve GUID references. Scripts use Unity's default Assembly-CSharp assembly, so Visual Studio displays Model, Runtime, and Debug together under Assets/Scripts, matching the disk layout. PokerSimulationRunner is guarded by UNITY_EDITOR and excluded from player builds. The standalone regression project continues to verify that the model compiles without Unity references.

After Unity imports script changes, use Edit > Preferences > External Tools > Regenerate project files if Visual Studio still displays the old Poker projects, then reopen the solution. Unity owns the generated solution and projects; the handwritten regression project is maintained separately.
