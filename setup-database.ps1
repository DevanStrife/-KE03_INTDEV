# Matrix Inc - Database Setup Script
# Automatische database configuratie voor nieuwe installaties

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Matrix Inc - Database Setup Script  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running from correct directory
$rootPath = $PSScriptRoot
if (-not $rootPath) {
    $rootPath = Get-Location
}

Write-Host "Working directory: $rootPath" -ForegroundColor Gray
Write-Host ""

# Step 1: Check Prerequisites
Write-Host "[1/6] Checking prerequisites..." -ForegroundColor Yellow

# Check .NET SDK
try {
    $dotnetVersion = dotnet --version
    Write-Host "  ✓ .NET SDK found: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "  ✗ .NET SDK not found!" -ForegroundColor Red
    Write-Host "    Please install .NET 10 SDK from https://dotnet.microsoft.com/" -ForegroundColor Red
    exit 1
}

# Check EF Core tools
try {
    $efVersion = dotnet ef --version 2>&1 | Select-Object -First 1
    if ($efVersion -match "Entity Framework Core") {
        Write-Host "  ✓ EF Core tools found: $efVersion" -ForegroundColor Green
    } else {
        throw "EF Core tools not installed"
    }
} catch {
    Write-Host "  ✗ EF Core tools not found!" -ForegroundColor Red
    Write-Host "    Installing EF Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ EF Core tools installed successfully" -ForegroundColor Green
    } else {
        Write-Host "  ✗ Failed to install EF Core tools" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""

# Step 2: Check project structure
Write-Host "[2/6] Verifying project structure..." -ForegroundColor Yellow

$dataAccessPath = Join-Path $rootPath "MatrixInc.DataAccess"
$webPath = Join-Path $rootPath "MatrixInc.Web"

if (-not (Test-Path $dataAccessPath)) {
    Write-Host "  ✗ MatrixInc.DataAccess project not found!" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $webPath)) {
    Write-Host "  ✗ MatrixInc.Web project not found!" -ForegroundColor Red
    exit 1
}

Write-Host "  ✓ Project structure verified" -ForegroundColor Green
Write-Host ""

# Step 3: Build solution
Write-Host "[3/6] Building solution..." -ForegroundColor Yellow

Set-Location $rootPath
dotnet build --configuration Release --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Build failed!" -ForegroundColor Red
    Write-Host "    Please fix compilation errors before running database setup." -ForegroundColor Red
    exit 1
}

Write-Host "  ✓ Solution built successfully" -ForegroundColor Green
Write-Host ""

# Step 4: Navigate to DataAccess project
Write-Host "[4/6] Preparing database migrations..." -ForegroundColor Yellow
Set-Location $dataAccessPath

# Check if user wants to drop existing database
Write-Host ""
Write-Host "  Do you want to drop the existing database (if it exists)?" -ForegroundColor Cyan
Write-Host "  This will DELETE all data!" -ForegroundColor Red
Write-Host "  [Y] Yes  [N] No (default)" -ForegroundColor Cyan
$dropDb = Read-Host "  Choice"

if ($dropDb -eq "Y" -or $dropDb -eq "y") {
    Write-Host "  Dropping existing database..." -ForegroundColor Yellow
    dotnet ef database drop --startup-project ..\MatrixInc.Web\MatrixInc.Web.csproj --force 2>&1 | Out-Null
    Write-Host "  ✓ Database dropped" -ForegroundColor Green
}

Write-Host ""

# Step 5: Apply migrations
Write-Host "[5/6] Creating database and applying migrations..." -ForegroundColor Yellow
Write-Host "  This may take a moment..." -ForegroundColor Gray

$migrateOutput = dotnet ef database update --startup-project ..\MatrixInc.Web\MatrixInc.Web.csproj 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ Database created successfully" -ForegroundColor Green
    Write-Host "  ✓ All migrations applied" -ForegroundColor Green
} else {
    Write-Host "  ✗ Database migration failed!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error details:" -ForegroundColor Red
    Write-Host $migrateOutput -ForegroundColor Red
    Write-Host ""
    Write-Host "Common solutions:" -ForegroundColor Yellow
    Write-Host "  1. Check if SQL Server LocalDB is installed and running" -ForegroundColor Gray
    Write-Host "  2. Verify connection string in appsettings.json" -ForegroundColor Gray
    Write-Host "  3. Run 'sqllocaldb start mssqllocaldb' to start LocalDB" -ForegroundColor Gray
    exit 1
}

Write-Host ""

# Step 6: Verify database and seed data
Write-Host "[6/6] Verifying database setup..." -ForegroundColor Yellow

$migrationsList = dotnet ef migrations list --startup-project ..\MatrixInc.Web\MatrixInc.Web.csproj --no-build 2>&1

if ($migrationsList -match "Applied") {
    Write-Host "  ✓ Migrations verified" -ForegroundColor Green

    # Show applied migrations
    Write-Host ""
    Write-Host "  Applied migrations:" -ForegroundColor Cyan
    $migrationsList | Where-Object { $_ -match "Applied" } | ForEach-Object {
        Write-Host "    - $_" -ForegroundColor Gray
    }
} else {
    Write-Host "  ⚠ Could not verify migrations" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Database Setup Complete! ✓           " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

Write-Host "Seed data added:" -ForegroundColor Cyan
Write-Host "  • 8 Products (Tandwielen, Lagers, etc.)" -ForegroundColor Gray
Write-Host "  • 3 Customers (Jan, Marie, Piet)" -ForegroundColor Gray
Write-Host "  • 3 Addresses (Amsterdam, Rotterdam, Utrecht)" -ForegroundColor Gray
Write-Host ""

Write-Host "Database connection:" -ForegroundColor Cyan
Write-Host "  Server: (localdb)\mssqllocaldb" -ForegroundColor Gray
Write-Host "  Database: MatrixIncDb" -ForegroundColor Gray
Write-Host ""

Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Start MatrixInc.Web:     cd MatrixInc.Web && dotnet run" -ForegroundColor Gray
Write-Host "  2. Start MatrixInc.Admin:   cd MatrixInc.Admin && dotnet run" -ForegroundColor Gray
Write-Host "  3. Start MatrixInc.Api:     cd MatrixInc.Api && dotnet run" -ForegroundColor Gray
Write-Host "  4. Start MatrixInc.Courier: Open in Visual Studio and Run (F5)" -ForegroundColor Gray
Write-Host ""

Write-Host "For more information, see:" -ForegroundColor Cyan
Write-Host "  docs/Database-Setup-Guide.md" -ForegroundColor Gray
Write-Host "  README.md" -ForegroundColor Gray
Write-Host ""

# Return to root directory
Set-Location $rootPath

Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
