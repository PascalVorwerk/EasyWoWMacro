# PowerShell script to add custom domain to Azure Web App
# Usage: .\add-custom-domain.ps1 -DomainName "yourdomain.com"

param(
    [Parameter(Mandatory=$true)]
    [string]$DomainName,
    
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "EasyWoWMacro-RG",
    
    [Parameter(Mandatory=$false)]
    [string]$WebAppName = "easywowmacro"
)

Write-Host "Adding custom domain: $DomainName" -ForegroundColor Green
Write-Host "Resource Group: $ResourceGroup" -ForegroundColor Yellow
Write-Host "Web App: $WebAppName" -ForegroundColor Yellow
Write-Host ""

# Check if Azure CLI is available
try {
    $azVersion = az --version
    Write-Host "✅ Azure CLI is available" -ForegroundColor Green
} catch {
    Write-Host "❌ Azure CLI not found. Please install Azure CLI first." -ForegroundColor Red
    exit 1
}

# Check if logged in to Azure
try {
    $account = az account show --query "name" --output tsv
    Write-Host "✅ Logged in to Azure account: $account" -ForegroundColor Green
} catch {
    Write-Host "❌ Not logged in to Azure. Please run 'az login' first." -ForegroundColor Red
    exit 1
}

# Add the custom domain
Write-Host "Adding custom domain to Azure Web App..." -ForegroundColor Yellow
try {
    az webapp config hostname add --webapp-name $WebAppName --resource-group $ResourceGroup --hostname $DomainName
    Write-Host "✅ Custom domain added successfully!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to add custom domain. Error: $_" -ForegroundColor Red
    exit 1
}

# List current hostnames
Write-Host ""
Write-Host "Current hostnames for $WebAppName:" -ForegroundColor Cyan
az webapp config hostname list --resource-group $ResourceGroup --webapp-name $WebAppName --query "[].name" --output table

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Add DNS records as described in DNS_SETUP.md" -ForegroundColor White
Write-Host "2. Wait for DNS propagation (15 minutes to 24 hours)" -ForegroundColor White
Write-Host "3. Verify domain ownership with Azure" -ForegroundColor White
Write-Host "4. Enable SSL certificate" -ForegroundColor White

Write-Host ""
Write-Host "Your app will be accessible at: https://$DomainName" -ForegroundColor Green 