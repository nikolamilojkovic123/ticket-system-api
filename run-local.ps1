# Pokreni infrastrukturu u Docker-u (SQL Server, Redis, RabbitMQ, Ollama)
Write-Host "Pokretanje infrastrukture..." -ForegroundColor Cyan
docker compose -f "$PSScriptRoot\docker-compose.infra.yml" --env-file "$PSScriptRoot\.env" up -d

# Sackaj da SQL Server bude spreman
Write-Host "Cekam da SQL Server bude spreman..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Pokreni API u novom prozoru
Write-Host "Pokretanje API-ja na http://localhost:5075 ..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\TicketSystem.API'; dotnet run --launch-profile http"

# Pokreni UI u novom prozoru
Write-Host "Pokretanje UI-ja na http://localhost:5057 ..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'C:\Ucenje\RSJP\TicketSystem.UI\TicketSystem.UI'; dotnet run --launch-profile http"

Write-Host ""
Write-Host "Lokalno okruzenje pokrenuto!" -ForegroundColor Green
Write-Host "  API:  http://localhost:5075/swagger" -ForegroundColor White
Write-Host "  UI:   http://localhost:5057" -ForegroundColor White
Write-Host ""
Write-Host "Za zaustavljanje infrastrukture:" -ForegroundColor Yellow
Write-Host "  docker compose -f docker-compose.infra.yml down" -ForegroundColor White
