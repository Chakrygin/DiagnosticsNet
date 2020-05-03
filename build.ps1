Write-Host "Restoring..."

dotnet restore `
    --packages "packages" `
    --configfile "nuget.config" `
    --no-cache

Write-Host

# ========================================

Write-Host "Building..."

dotnet build `
    --configuration "Release" `
    --no-restore `
    --nologo

Write-Host

# ========================================

foreach ($project in Get-ChildItem -File -Path "tests\**\*.Tests.csproj" | Resolve-Path -Relative)
{
    Write-Host "Testing ""$project"""

    dotnet test "$project" `
        --configuration "Release" `
        --results-directory "TestResults" `
        --no-build `
        --nologo

    Write-Host
}

# ========================================

foreach ($project in Get-ChildItem -File -Path "src\**\*.csproj" | Resolve-Path -Relative)
{
    Write-Host "Packing ""$project"""

    dotnet pack "$project" `
        --configuration "Release" `
        --output "artifacts" `
        --no-build `
        --nologo

    Write-Host
}

# ========================================
