# Run once to install the service and necessary environment variables
# Make sure to run as Administrator!
$servicePath = Resolve-Path "./HeliosDiscordBot.exe"
New-Service "HeliosDiscordBot" -BinaryPathName "$servicePath"
[System.Environment]::SetEnvironmentVariable('HeliosDiscordBot_DiscordSettings__Token','<TOKEN>',[System.EnvironmentVariableTarget]::Machine)
Start-Service "HeliosDiscordBot"
