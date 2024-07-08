$root = "$PSScriptRoot\src\Utility.Applications"
$csprojFiles = Get-ChildItem -Path $root -Recurse -Filter *.csproj

$csprojFiles | ForEach-Object { dotnet publish $_.FullName -r win-x64 -p:PublishSingleFile=true --self-contained true -o ./artifacts }

#dotnet publish  -r win-x64 -p:PublishSingleFile=true --self-contained true -o ./artifacts