# Jedna ulazna tacka za pokretanje/gasenje TicketSystem-a.
#
# Upotreba:
#   .\start.ps1                  -> interaktivni meni (pita Docker ili Lokalno)
#   .\start.ps1 -Mode docker     -> ceo stack u Docker-u (SQL, Redis, RabbitMQ, Ollama, API, UI)
#   .\start.ps1 -Mode local      -> infra u Docker-u, API/UI lokalno (dotnet run) - via run-local.ps1
#   .\start.ps1 -Mode docker -Stop   -> gasi Docker stack
#   .\start.ps1 -Mode local -Stop    -> gasi infra kontejnere (dotnet run prozore zatvori rucno)

param(
    [ValidateSet("docker", "local")]
    [string]$Mode,

    [switch]$Stop
)

if (-not $Mode) {
    Write-Host ""
    Write-Host "Kako zelis da pokrenes TicketSystem?" -ForegroundColor Cyan
    Write-Host "  [1] Docker  - sve u kontejnerima (SQL, Redis, RabbitMQ, Ollama, API, UI)" -ForegroundColor White
    Write-Host "  [2] Lokalno - infra u Dockeru, API/UI lokalno (dotnet run, brzi edit/debug)" -ForegroundColor White
    Write-Host ""
    $choice = Read-Host "Izbor (1/2)"

    $Mode = switch ($choice) {
        "1" { "docker" }
        "2" { "local" }
        default {
            Write-Host "Nepoznat izbor." -ForegroundColor Red
            exit 1
        }
    }
}

switch ($Mode) {

    "docker" {
        if ($Stop) {
            Write-Host "Gasenje Docker stack-a..." -ForegroundColor Yellow
            docker compose -f "$PSScriptRoot\docker-compose.yml" --env-file "$PSScriptRoot\.env" down
            break
        }

        Write-Host "Pokretanje celog stack-a u Docker-u..." -ForegroundColor Cyan
        docker compose -f "$PSScriptRoot\docker-compose.yml" --env-file "$PSScriptRoot\.env" up --build -d

        Write-Host ""
        Write-Host "Pokrenuto!" -ForegroundColor Green
        Write-Host "  UI:            http://localhost:4200" -ForegroundColor White
        Write-Host "  API / Swagger: http://localhost:5000/swagger" -ForegroundColor White
        Write-Host "  RabbitMQ:      http://localhost:15672" -ForegroundColor White
        Write-Host ""
        Write-Host "Logovi:  docker compose -f docker-compose.yml logs -f" -ForegroundColor Yellow
        Write-Host "Gasenje: .\start.ps1 -Mode docker -Stop" -ForegroundColor Yellow
    }

    "local" {
        if ($Stop) {
            Write-Host "Gasenje infra kontejnera..." -ForegroundColor Yellow
            docker compose -f "$PSScriptRoot\docker-compose.infra.yml" --env-file "$PSScriptRoot\.env" down
            Write-Host "Napomena: API/UI prozore (dotnet run) zatvori rucno." -ForegroundColor Yellow
            break
        }

        & "$PSScriptRoot\run-local.ps1"
    }
}
