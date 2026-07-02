# Database Reset Script voor Matrix Inc
# Dit script reset de database naar de nieuwe structuur met Address support

Write-Host "=== Matrix Inc Database Reset ===" -ForegroundColor Cyan
Write-Host ""

# Check of de database bestaat
$dbName = "MatrixIncDb"
$connectionString = "(localdb)\mssqllocaldb"

Write-Host "Stap 1: Stoppen van alle apps..." -ForegroundColor Yellow
# Stop eventuele draaiende processen
Get-Process | Where-Object { $_.ProcessName -like "*MatrixInc*" } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

Write-Host "Stap 2: Database verwijderen..." -ForegroundColor Yellow
# Verwijder de database
$deleteQuery = @"
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'$dbName')
BEGIN
    ALTER DATABASE [$dbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$dbName];
    PRINT 'Database $dbName is verwijderd.';
END
ELSE
BEGIN
    PRINT 'Database $dbName bestaat niet.';
END
"@

try {
    sqlcmd -S $connectionString -Q $deleteQuery -ErrorAction Stop
    Write-Host "✓ Database verwijderd" -ForegroundColor Green
}
catch {
    Write-Host "⚠ Database verwijderen mislukt (mogelijk niet aanwezig)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Stap 3: Database opnieuw aanmaken..." -ForegroundColor Yellow
Write-Host "De database wordt automatisch aangemaakt wanneer je een van de apps start." -ForegroundColor Cyan
Write-Host ""

Write-Host "=== Voltooid ===" -ForegroundColor Green
Write-Host ""
Write-Host "Volgende stappen:" -ForegroundColor Cyan
Write-Host "1. Start MatrixInc.Web of MatrixInc.Admin" -ForegroundColor White
Write-Host "2. De database wordt automatisch aangemaakt met de nieuwe structuur" -ForegroundColor White
Write-Host "3. Seed data wordt automatisch toegevoegd" -ForegroundColor White
Write-Host ""
