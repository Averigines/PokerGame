Run the standalone model regression checks without opening Unity:

    dotnet run --project Tests/HandEvaluation/HandEvaluation.csproj

Requires the .NET 10 SDK. The project compiles every actual model source and has no external package dependencies. Named scenarios print individual PASS results; a failing scenario reports its name and assertion message.
