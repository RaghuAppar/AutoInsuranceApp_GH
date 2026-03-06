# Push This Repo to GitHub

**origin** is set to the GitHub repository **AutoInsuranceApp_GH**. **azure** is the Azure DevOps repo.

## One-time setup

1. **Create a repository on GitHub** named **AutoInsuranceApp_GH** (if you haven’t already):  
   https://github.com/new  
   Leave it empty (no README, .gitignore, or license).

2. **Set the origin URL** (replace `RaghuAppar` with your GitHub username or organization):

   ```bash
   git remote set-url origin https://github.com/RaghuAppar/AutoInsuranceApp_GH.git
   ```

   Or with SSH:

   ```bash
   git remote set-url origin git@github.com:RaghuAppar/AutoInsuranceApp_GH.git
   ```

3. **Push the development branch to GitHub**:

   ```bash
   git push -u origin development
   ```

   Optionally push other branches:

   ```bash
   git push origin main
   git push origin release
   ```

## Remotes

| Remote  | Repository |
|---------|------------|
| **origin** | GitHub **AutoInsuranceApp_GH**: `https://github.com/RaghuAppar/AutoInsuranceApp_GH.git` (replace `RaghuAppar`) |
| **azure**  | Azure DevOps: `https://dev.azure.com/WPAzureDevOpsTrng/AutoInsuranceApp/_git/AutoInsuranceApp` |

## Push to both

```bash
git push origin development    # GitHub
git push azure development     # Azure DevOps
```

Or in one go:

```bash
git push origin development && git push azure development
```
