# Create Azure DevOps Project and Push Code

Your repo is ready: **AutoInsuranceApp** has a git repo with an initial commit (API + Client). To create the Azure DevOps project **AutomInsuranceApp** and push the code, use one of the options below.

---

## Option A: Create project in the portal, then push

### 1. Create the project

1. Open: **https://dev.azure.com/WPAzureDevOpsTrng**
2. Click **+ New project**.
3. **Project name:** `AutomInsuranceApp`
4. **Version control:** Git  
5. Click **Create**.  
   This creates a default repo named **AutomInsuranceApp**.

### 2. Push from your machine

In PowerShell, from the repo root:

```powershell
cd "c:\Cursor AI Demos\AutoInsuranceApp"

# Add Azure DevOps as remote (replace YOUR_PAT with a PAT if needed)
git remote add origin https://dev.azure.com/WPAzureDevOpsTrng/AutomInsuranceApp/_git/AutomInsuranceApp

# Push (branch is master)
git push -u origin master
```

When prompted for credentials, use your Azure DevOps username and a [Personal Access Token (PAT)](https://dev.azure.com/WPAzureDevOpsTrng/_usersSettings/tokens) (Code → Read & write).

---

## Option B: Use Azure DevOps CLI (create project + push)

### 1. Install and log in

```powershell
az extension add --name azure-devops
az devops login
```

Use a PAT when prompted (create one at: https://dev.azure.com/WPAzureDevOpsTrng/_usersSettings/tokens).

### 2. Create project and push

```powershell
cd "c:\Cursor AI Demos\AutoInsuranceApp"
.\scripts\create-ado-and-push.ps1
```

The script will create the project **AutomInsuranceApp** (if it doesn’t exist), add the remote, and push your current branch.

---

## Repo contents

- **AutoInsuranceApi/** – ASP.NET Core 8 Web API (SQLite, JWT, REST)
- **Client/** – AngularJS 1.x app (views, services, controllers)
- **docs/** – Setup notes
- **scripts/** – `create-ado-and-push.ps1` for Option B
