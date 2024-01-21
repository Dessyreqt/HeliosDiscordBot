$roundhouse_exe_path = ".\rh.exe"
$db_dir = ".\scripts"
$connection_string = $OctopusParameters["DatabaseSettings:ConnectionString"]

&$roundhouse_exe_path /c=$connection_string /f=$db_dir /cds="$db_dir\create.sql" /silent /transaction

