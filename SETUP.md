# Setup Guide

Complete setup instructions for the GitHub Copilot Multi-Agent System.

## 📋 Prerequisites

### Required
- ✅ **GitHub Copilot** subscription (Individual, Business, or Enterprise)
- ✅ **VS Code** or **Visual Studio** with GitHub Copilot extension installed
- ✅ **Git** installed
- ✅ An **Azure subscription** (for deploying to Azure)

### Optional (for CI/CD)
- GitHub repository with Actions enabled
- Azure Service Principals for deployments
- Terraform Cloud account (or Azure Storage for state)

## 🚀 Installation

### Step 1: Copy Files to Your Project

```bash
# Option A: Clone directly into .github folder
cd your-project-root
git clone <repository-url> .github-agents-temp
cp -r .github-agents-temp/.github/* .github/
rm -rf .github-agents-temp

# Option B: Download and extract
# Download the release ZIP
unzip nvt-agents-final.zip
cp -r nvt-agents-final/.github/* your-project/.github/
```

### Step 2: Verify File Structure

Check that your `.github` folder contains:

```bash
ls -la .github/

# You should see:
# - agents/
# - clients/
# - config/
# - instructions/
# - knowledge/
# - prompts/
# - skills/
# - tools/
# - workflows/
```

### Step 3: Test Agent Recognition

1. Open VS Code
2. Open GitHub Copilot Chat (`Ctrl+Shift+I` or `Cmd+Shift+I`)
3. Type `@` and press space
4. You should see: `@ba`, `@archi`, `@dev`, `@reviewer`

✅ If you see them → Success! Skip to **Client Configuration**

❌ If not → Continue to **Troubleshooting**

## 🏢 Client Configuration

### Using Default Configuration

The system ships with a "default" client already active. This is suitable for:
- Internal projects
- Generic Azure data integration
- Learning and experimentation

**No action needed** if this fits your needs.

### Creating a Client-Specific Configuration

For real client projects:

```bash
# Navigate to your project
cd your-project-root

# Create new client
./.github/tools/client-manager.sh create contoso-pharma

# Edit configuration
nano .github/clients/contoso-pharma/CLIENT.md
# or
code .github/clients/contoso-pharma/CLIENT.md
```

### What to Configure in CLIENT.md

Edit these sections:

1. **Client Overview**
   - Client name
   - Industry
   - Project type
   - Timeline

2. **Technical Stack**
   - Azure subscription ID
   - Azure regions
   - Specific services used

3. **Naming Conventions**
   - Resource naming patterns
   - Tagging standards

4. **Security Requirements**
   - Compliance needs (GDPR, HIPAA, etc.)
   - Data classification
   - Access control requirements

5. **Data Pipeline Requirements**
   - Data sources
   - Data destinations
   - SLAs and performance requirements

### Activate Your Client

```bash
./.github/tools/client-manager.sh activate contoso-pharma
```

Verify:
```bash
./.github/tools/client-manager.sh show
```

## 🔧 CI/CD Configuration (Optional)

### GitHub Actions Setup

#### 1. Create Azure Service Principals

```bash
# For Development
az ad sp create-for-rbac --name "github-actions-dev" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{rg-name-dev} \
  --sdk-auth

# For Staging
az ad sp create-for-rbac --name "github-actions-staging" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{rg-name-staging} \
  --sdk-auth

# For Production
az ad sp create-for-rbac --name "github-actions-prod" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{rg-name-prod} \
  --sdk-auth
```

Save the JSON output for each.

#### 2. Configure GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions

Add these secrets:

**Azure Credentials**
```
AZURE_CREDENTIALS_DEV       (JSON from step 1 - dev)
AZURE_CREDENTIALS_STAGING   (JSON from step 1 - staging)
AZURE_CREDENTIALS_PROD      (JSON from step 1 - prod)
```

**Terraform State (if using Terraform)**
```
TF_STATE_RG_DEV            (Resource group for state)
TF_STATE_SA_DEV            (Storage account for state)
TF_STATE_RG_STAGING        
TF_STATE_SA_STAGING        
TF_STATE_RG_PROD           
TF_STATE_SA_PROD           
```

**Azure Data Factory (if using ADF)**
```
ADF_RG_DEV                 (ADF resource group - dev)
ADF_NAME_DEV               (ADF name - dev)
ADF_RG_STAGING             
ADF_NAME_STAGING           
ADF_RG_PROD                
ADF_NAME_PROD              
```

**Optional**
```
INFRACOST_API_KEY          (For cost estimation)
SONAR_TOKEN                (For SonarCloud)
```

#### 3. Configure Environments

Go to Settings → Environments

Create three environments:
- `development` - No approval required
- `staging` - No approval required
- `production` - **Required reviewers**: Add team members

### 4. Customize Workflows

Edit workflow files in `.github/workflows/` to match your project structure:

**`.github/workflows/dotnet-build-deploy.yml`**
```yaml
env:
  DOTNET_VERSION: '8.0.x'          # Your .NET version
  AZURE_WEBAPP_NAME: 'your-app'    # Your app name
  AZURE_WEBAPP_PACKAGE_PATH: './publish'
```

**`.github/workflows/terraform-deploy.yml`**
```yaml
env:
  TF_VERSION: '1.7.0'              # Your Terraform version
  WORKING_DIR: './terraform'        # Your Terraform directory
```

**`.github/workflows/adf-deploy.yml`**
```yaml
env:
  ADF_DIR: './data-factory'         # Your ADF directory
```

### 5. Test Your Pipelines

```bash
# Push to trigger workflows
git add .github/workflows/
git commit -m "Configure CI/CD pipelines"
git push origin develop  # Triggers dev deployment
```

## 🧪 Testing Your Setup

### Test 1: Agent Recognition

```
# In Copilot Chat
@ba Hello, can you hear me?
```

Expected: Business Analyst should respond.

### Test 2: Instructions Auto-Application

Create a C# file:
```bash
mkdir -p src
cat > src/Test.cs << 'EOF'
public class Test {
    public string GetMessage() {
        return "hello";
    }
}
EOF
```

Open in VS Code and ask:
```
@dev Review this C# code for best practices
```

Expected: Developer should mention C# conventions (PascalCase methods, nullable reference types, etc.)

### Test 3: Knowledge Base Access

```
@archi What are the best practices for Azure Data Factory according to our knowledge base?
```

Expected: Should reference content from `.github/knowledge/azure/best-practices.md`

### Test 4: Skills Usage

```
@archi Use the solution-design skill to design a data platform architecture
```

Expected: Should follow the methodology from `.github/skills/solution-design/SKILL.md`

### Test 5: Client Context

```
@ba What's our current active client and their context?
```

Expected: Should reference content from `.github/clients/[active-client]/CLIENT.md`

## 🐛 Troubleshooting

### Agents Don't Appear

**Problem**: `@ba`, `@archi`, etc. don't autocomplete

**Solutions**:
1. Check file locations:
   ```bash
   ls .github/agents/
   ls .github/config/copilot-config.json
   ```

2. Restart VS Code

3. Verify GitHub Copilot is enabled:
   - VS Code → Extensions → GitHub Copilot → Ensure enabled

4. Check copilot-config.json syntax:
   ```bash
   cat .github/config/copilot-config.json | jq .
   ```

### Instructions Not Applied

**Problem**: Coding standards aren't being followed automatically

**Solutions**:
1. Check frontmatter in instruction files:
   ```bash
   head -n 5 .github/instructions/terraform.md
   # Should show:
   # ---
   # applyTo: "**/*.tf"
   # ---
   ```

2. Verify file extension: `.instructions.md` not just `.md`

3. Check file path matches `applyTo` pattern

### Client Manager Script Not Working

**Problem**: `client-manager.sh` fails or not found

**Solutions**:
1. Make executable:
   ```bash
   chmod +x .github/tools/client-manager.sh
   ```

2. Run from project root:
   ```bash
   cd your-project-root
   ./.github/tools/client-manager.sh list
   ```

3. Check script exists:
   ```bash
   ls -la .github/tools/client-manager.sh
   ```

### CI/CD Pipeline Failures

**Problem**: GitHub Actions workflows fail

**Solutions**:
1. Check secrets are configured:
   - GitHub repository → Settings → Secrets

2. Verify Azure credentials:
   ```bash
   # Test the service principal
   az login --service-principal \
     -u <appId> \
     -p <password> \
     --tenant <tenant>
   ```

3. Check workflow syntax:
   ```bash
   # Install act (local workflow runner)
   # https://github.com/nektos/act
   act -l  # List workflows
   ```

4. Review workflow logs in GitHub Actions tab

### Knowledge Base Empty

**Problem**: Agents say they can't access knowledge

**Solutions**:
1. Verify files exist:
   ```bash
   ls -la .github/knowledge/azure/
   # Should show: best-practices.md, services.md
   ```

2. Check file content:
   ```bash
   head .github/knowledge/azure/best-practices.md
   ```

3. Files should have substantial content (not empty)

## 📚 Next Steps

After successful setup:

1. **Read Documentation**
   - [START-HERE.md](START-HERE.md) - Quick start
   - [README.md](README.md) - Complete documentation
   - [INDEX.md](INDEX.md) - System overview

2. **Try Example Workflows**
   - See [START-HERE.md](START-HERE.md) for example interactions

3. **Customize for Your Team**
   - Add team-specific skills
   - Extend knowledge base
   - Create additional prompts
   - Configure client-specific settings

4. **Set Up CI/CD**
   - Configure GitHub Actions
   - Set up Azure environments
   - Test deployment pipelines

## 🎓 Training

Recommended learning path:

**Week 1: Basics**
- Day 1-2: Understand agent roles and capabilities
- Day 3-4: Practice basic workflows
- Day 5: Set up first client configuration

**Week 2: Advanced**
- Day 1-2: Master skills and knowledge base
- Day 3-4: Configure CI/CD pipelines
- Day 5: Customize for team needs

**Week 3: Mastery**
- Day 1-3: Train team members
- Day 4-5: Implement on real projects

## 🆘 Support

Still stuck?

1. Check [INDEX.md](INDEX.md) for system overview
2. Review agent documentation in `.github/agents/`
3. Ask the agents directly:
   ```
   @archi Explain how you work and when I should use you
   ```
4. Contact the architecture team

---

**Setup Version**: 1.0.0  
**Last Updated**: 2026-02-05  
**For**: Azure Data Integration Projects
