$wait = 60
Write-Host "Shutting Down Non-DC VMs" -ForegroundColor DarkRed
get-vm | Where-Object {($_.Name -notlike 'DC*') -and ($_.Name -notlike '*DISABLED') -and ($_.State -ne 'Off')} | stop-vm -force
Start-Sleep -Seconds $wait
Write-Host "Shutting Down DCs" -ForegroundColor Red
get-vm | Where-Object {($_.Name -like 'DC*') -and ($_.Name -notlike '*DISABLED') -and ($_.State -ne 'Off')}  | stop-vm -force
