#Requires -Version 5.1
<#
.SYNOPSIS
  Creates the Azure DevOps project "AutomInsuranceApp" (if missing) and pushes the repo.
.DESCRIPTION
  Run after: az devops login
  Ensures project exists, adds remote origin, pushes current branch.
#>
$ErrorActionPreference = "Stop"
$OrgUrl = "https://dev.azure.com/WPAzureDevOpsTrng"
$ProjectName = "AutomInsuranceApp"
$RepoName = "AutomInsuranceApp"
$RootDir = Split-Path -Parent $PSScriptRoot   # Repo root = parent of scripts folder

Set-Location $RootDir

# Ensure git repo and at least one commit
if (-not (Test-Path ".git")) {
    Write-Host "Run from repo root. Expected: $RootDir"
    exit 1
}
$branch = git rev-parse --abbrev-ref HEAD 2>$null
if (-not $branch) {
    Write-Host "No commits yet. Add files and commit first."
    exit 1
}

# Configure defaults
az devops configure --defaults organization=$OrgUrl project=$ProjectName 2>$null

# Check if project exists
$projects = az devops project list --organization $OrgUrl --output json | ConvertFrom-Json
$exists = $projects | Where-Object { $_.name -eq $ProjectName }
if (-not $exists) {
    Write-Host "Creating project: $ProjectName"
    az devops project create --name $ProjectName --description "Auto Insurance App - API and Client"
    Start-Sleep -Seconds 3
}

# Remote URL (prompt for PAT or use Git Credential Manager)
$RemoteUrl = "$OrgUrl/$ProjectName/_git/$RepoName"
$origin = git remote get-url origin 2>$null
if ($LASTEXITCODE -ne 0) { $origin = $null }
if (-not $origin) {
    git remote add origin $RemoteUrl
    Write-Host "Added remote: origin -> $RemoteUrl"
} else {
    Write-Host "Remote origin already set: $origin"
}

# Push (use main or master)
$branch = git rev-parse --abbrev-ref HEAD 2>$null
if (-not $branch) { $branch = "main" }
Write-Host "Pushing branch: $branch"
git push -u origin $branch

Write-Host "Done. Repo: $OrgUrl/$ProjectName/_git/$RepoName"
