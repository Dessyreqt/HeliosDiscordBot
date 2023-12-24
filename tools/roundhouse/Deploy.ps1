$roundhouse_exe_path = ".\rh.exe"
$db_dir = ".\scripts"
$db_name = "HeliosDiscordBot"
$db_server = "localhost"

&$roundhouse_exe_path /d=$db_name /f=$db_dir /s=$db_server /cds="$db_dir\create.sql" /silent /transaction

