dotnet restore src\PhilosophicalMonkey\project.json
dotnet pack src\PhilosophicalMonkey\project.json -c Release -o artifacts\bin\PhilosophicalMonkey\Release

set /p version="Version: "
nuget push artifacts\bin\PhilosophicalMonkey\Release\PhilosophicalMonkey.%version%.nupkg