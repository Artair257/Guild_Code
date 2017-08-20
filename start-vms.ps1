$wait = 200
Write-Host "Activating DCs" -ForegroundColor Green
get-vm | where-object {($_.Name -like 'DC*') -and ($_.Name -notlike '*DISABLED') -and ($_.State -eq 'Off')} | start-vm
Start-Sleep -Seconds $wait
Write-Host "ACtiving VMs" -ForegroundColor DarkGreen
get-vm | where-object {($_.Name -notlike 'DC*') -and ($_.Name -notlike '*DISABLED') -and ($_.State -eq 'Off')} | start-vm
