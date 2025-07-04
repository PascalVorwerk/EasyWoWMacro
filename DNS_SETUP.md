# DNS Setup Guide for Custom Domain

## 🌐 Your Azure Web App Details

- **App Name**: `easywowmacro`
- **Default URL**: https://easywowmacro.azurewebsites.net
- **Resource Group**: `EasyWoWMacro-RG`
- **Region**: West Europe

## 📋 DNS Configuration Options

You have several options for setting up your custom domain. Choose the one that best fits your needs:

### Option 1: CNAME Record (Recommended for Subdomains)

**Best for**: `app.yourdomain.com`, `www.yourdomain.com`, `easywowmacro.yourdomain.com`

**DNS Records to Add**:
```
Type: CNAME
Name: your-subdomain (e.g., "app" or "www")
Value: easywowmacro.azurewebsites.net
TTL: 3600 (or default)
```

**Example**:
- If your domain is `example.com` and you want `app.example.com`
- Add CNAME: `app` → `easywowmacro.azurewebsites.net`

### Option 2: A Record (For Root Domain)

**Best for**: `yourdomain.com` (root domain)

**DNS Records to Add**:
```
Type: A
Name: @ (or leave empty for root domain)
Value: 20.126.176.160
TTL: 3600 (or default)
```

**Note**: You'll also need to add a TXT record for domain verification:
```
Type: TXT
Name: @ (or leave empty for root domain)
Value: asuid.yourdomain.com
TTL: 3600 (or default)
```

### Option 3: Multiple Records (For Both Root and Subdomain)

**Best for**: Having both `yourdomain.com` and `www.yourdomain.com`

**DNS Records**:
```
Type: A
Name: @
Value: 20.126.176.160
TTL: 3600

Type: CNAME
Name: www
Value: easywowmacro.azurewebsites.net
TTL: 3600

Type: TXT
Name: @
Value: asuid.yourdomain.com
TTL: 3600
```

## 🔧 Step-by-Step Setup Process

### Step 1: Choose Your Domain Strategy

1. **Decide on your domain name** (e.g., `easywowmacro.com`, `app.yourdomain.com`)
2. **Choose the DNS record type** based on your needs (see options above)

### Step 2: Add DNS Records

Go to your domain registrar's DNS management panel and add the appropriate records:

**Common Registrars**:
- **GoDaddy**: DNS Management → Manage Zones
- **Namecheap**: Domain List → Manage → Advanced DNS
- **Google Domains**: DNS → Manage Custom Records
- **Cloudflare**: DNS → Records
- **Route53**: Hosted Zones → Create Record

### Step 3: Verify DNS Propagation

After adding DNS records, verify they're working:

```bash
# For CNAME records
nslookup your-subdomain.yourdomain.com

# For A records
nslookup yourdomain.com
```

### Step 4: Add Custom Domain to Azure

Once DNS is configured, add the domain to Azure:

```bash
# For CNAME (subdomain)
az webapp config hostname add --webapp-name easywowmacro --resource-group EasyWoWMacro-RG --hostname your-subdomain.yourdomain.com

# For A record (root domain)
az webapp config hostname add --webapp-name easywowmacro --resource-group EasyWoWMacro-RG --hostname yourdomain.com
```

### Step 5: Verify Domain Ownership

Azure will provide a verification token. Add it as a TXT record:

```
Type: TXT
Name: asuid
Value: [verification-token-from-azure]
TTL: 3600
```

### Step 6: Enable SSL Certificate

Once verified, enable HTTPS:

```bash
# Enable HTTPS only
az webapp update --resource-group EasyWoWMacro-RG --name easywowmacro --https-only true

# Add free SSL certificate
az webapp config ssl bind --certificate-thumbprint [thumbprint] --ssl-type SNI --name easywowmacro --resource-group EasyWoWMacro-RG
```

## 🎯 Recommended Setup Examples

### Example 1: Subdomain Setup
**Domain**: `app.easywowmacro.com`

**DNS Records**:
```
CNAME: app → easywowmacro.azurewebsites.net
```

### Example 2: Root Domain Setup
**Domain**: `easywowmacro.com`

**DNS Records**:
```
A: @ → 20.126.176.160
TXT: asuid → [verification-token]
```

### Example 3: WWW + Root Setup
**Domain**: `easywowmacro.com` and `www.easywowmacro.com`

**DNS Records**:
```
A: @ → 20.126.176.160
CNAME: www → easywowmacro.azurewebsites.net
TXT: asuid → [verification-token]
```

## ⏱️ DNS Propagation Time

- **CNAME records**: Usually 15 minutes to 2 hours
- **A records**: Usually 15 minutes to 24 hours
- **TXT records**: Usually 15 minutes to 2 hours

## 🔍 Troubleshooting

### Common Issues

1. **DNS Not Propagated**
   - Wait 24-48 hours for full propagation
   - Use tools like `whatsmydns.net` to check global propagation

2. **Domain Verification Fails**
   - Ensure TXT record is exactly as provided by Azure
   - Check for typos in the verification token

3. **SSL Certificate Issues**
   - Ensure domain is properly verified before requesting SSL
   - Check that DNS records are pointing to the correct Azure app

### Verification Commands

```bash
# Check current hostnames
az webapp config hostname list --resource-group EasyWoWMacro-RG --webapp-name easywowmacro

# Check SSL status
az webapp config ssl list --resource-group EasyWoWMacro-RG

# Test DNS resolution
nslookup yourdomain.com
```

## 🚀 Next Steps After DNS Setup

1. **Update GitHub Actions**: If you change the domain, update the workflow
2. **Configure Redirects**: Set up www to non-www redirects (or vice versa)
3. **Monitor Performance**: Use Azure Application Insights to monitor your app
4. **Set up CDN**: Consider Azure CDN for better global performance

## 📞 Support

If you encounter issues:
- Check Azure App Service logs in the Azure Portal
- Verify DNS records using online tools
- Contact your domain registrar for DNS-related issues

---

**Your app will be accessible at your custom domain once DNS is configured and propagated!** 