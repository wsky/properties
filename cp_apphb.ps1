$source='build\'+$args[0]+'\properties_web'
$target='..\apphb-properties'
get-childitem $target | remove-item -recurse -force -confirm:$false
xcopy $source $target  /Y /E /I /R 