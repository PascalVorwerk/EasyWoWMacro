# Azure Setup Guide for Blazor Server Migration

## ✅ What We've Already Set Up

Your Azure infrastructure has been created successfully:

- **Resource Group**: `EasyWoWMacro-RG` (West Europe)
- **App Service Plan**: `EasyWoWMacro-Plan` (B1 Basic tier)
- **Web App**: `easywowmacro` (https://easywowmacro.azurewebsites.net)
- **Runtime**: .NET 9.0 (Linux)

## 🔧 Next Steps to Complete the Migration

### 1. Update Your Project Target Framework (Optional)

Since Azure App Service doesn't support .NET 10.0 yet, you have two options:

**Option A: Keep .NET 10.0** (Recommended)
- Your .NET 10.0 project will run fine on .NET 9.0 runtime
- No changes needed to your project

**Option B: Downgrade to .NET 9.0**
- Update `EasyWoWMacro.Web.csproj` to use `net9.0`
- Update `EasyWoWMacro.Core.csproj` to use `net9.0`

### 2. Configure GitHub Secrets

You need to add the Azure publish profile to your GitHub repository secrets:

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Copy the entire XML content from the publish profile we generated

**Publish Profile Content:**
```xml
<publishData><publishProfile profileName="easywowmacro - Web Deploy" publishMethod="MSDeploy" publishUrl="easywowmacro.scm.azurewebsites.net:443" msdeploySite="easywowmacro" userName="$easywowmacro" userPWD="GiYoeKSg1iE0c0mhxokPi3qYAkwRQtzr3sowqRTbNetwqK0vbcdl9djof3Hq" destinationAppUrl="http://easywowmacro.azurewebsites.net" SQLServerDBConnectionString="" mySQLDBConnectionString="" hostingProviderForumLink="" controlPanelLink="https://portal.azure.com" webSystem="WebSites"><databases /></publishProfile><publishProfile profileName="easywowmacro - FTP" publishMethod="FTP" publishUrl="ftps://waws-prod-am2-577.ftp.azurewebsites.windows.net/site/wwwroot" ftpPassiveMode="True" userName="easywowmacro\$easywowmacro" userPWD="GiYoeKSg1iE0c0mhxokPi3qYAkwRQtzr3sowqRTbNetwqK0vbcdl9djof3Hq" destinationAppUrl="http://easywowmacro.azurewebsites.net" SQLServerDBConnectionString="" mySQLDBConnectionString="" hostingProviderForumLink="" controlPanelLink="https://portal.azure.com" webSystem="WebSites"><databases /></publishProfile><publishProfile profileName="easywowmacro - Zip Deploy" publishMethod="ZipDeploy" publishUrl="easywowmacro.scm.azurewebsites.net:443" userName="$easywowmacro" userPWD="GiYoeKSg1iE0c0mhxokPi3qYAkwRQtzr3sowqRTbNetwqK0vbcdl9djof3Hq" destinationAppUrl="http://easywowmacro.azurewebsites.net" SQLServerDBConnectionString="" mySQLDBConnectionString="" hostingProviderForumLink="" controlPanelLink="https://portal.azure.com" webSystem="WebSites"><databases /></publishProfile></publishData>
```

### 3. Update GitHub Actions Workflow

The workflow file `.github/workflows/deploy-server.yml` has been created and configured. You can now:

1. **Disable the old workflow**: Go to `.github/workflows/deploy.yml` and disable it
2. **Enable the new workflow**: The `deploy-server.yml` is ready to use

### 4. Test Your Migration

1. **Local Testing**: Run your app locally to ensure it works:
   ```bash
   cd EasyWoWMacro.Web
   dotnet run
   ```

2. **Deploy to Azure**: Push to your master branch to trigger the new deployment

3. **Verify SEO**: Check that your pages are properly prerendered by:
   - Viewing page source (should show HTML content)
   - Using browser dev tools to check for server-side rendering

### 5. Custom Domain Setup (Optional)

If you want to use a custom domain instead of `easywowmacro.azurewebsites.net`:

1. Go to Azure Portal → App Service → easywowmacro
2. Navigate to **Custom domains**
3. Add your domain and configure DNS records

### 6. SSL Certificate (Recommended)

Enable HTTPS for better SEO:

1. Go to Azure Portal → App Service → easywowmacro
2. Navigate to **TLS/SSL settings**
3. Enable **HTTPS Only**
4. Add a free SSL certificate

## 🚀 Benefits of This Migration

### SEO Improvements
- **Server-side rendering**: Search engines can crawl your content
- **Faster initial page load**: HTML is rendered on the server
- **Better Core Web Vitals**: Improved performance metrics

### Performance Benefits
- **Reduced client-side JavaScript**: Less code sent to browsers
- **Better caching**: Static assets are cached more effectively
- **Improved accessibility**: Better for screen readers and assistive technologies

### Development Benefits
- **Simpler debugging**: Server-side errors are easier to trace
- **Better error handling**: Server-side error pages
- **Easier deployment**: Standard ASP.NET Core deployment

## 📊 Cost Comparison

**Current Setup (B1 Basic Plan)**:
- **Cost**: ~$13/month
- **Features**: 1 CPU, 1.75 GB RAM, 10 GB storage
- **Suitable for**: Development and small production workloads

**Scaling Options**:
- **B2**: ~$26/month (2 CPU, 3.5 GB RAM)
- **S1**: ~$73/month (1 CPU, 1.75 GB RAM, auto-scaling)
- **P1V2**: ~$146/month (1 CPU, 3.5 GB RAM, premium features)

## 🔍 Monitoring and Maintenance

1. **Azure Application Insights**: Enable for monitoring
2. **Log Analytics**: Set up for centralized logging
3. **Health Checks**: Configure application health monitoring

## 🆘 Troubleshooting

### Common Issues

1. **Build Failures**: Check that all dependencies are compatible
2. **Runtime Errors**: Verify .NET version compatibility
3. **Deployment Issues**: Check GitHub secrets and permissions

### Support Resources

- [Azure App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Blazor Server Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/server)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

---

**Your app will be available at**: https://easywowmacro.azurewebsites.net

**Next step**: Add the GitHub secret and push to master to deploy! 