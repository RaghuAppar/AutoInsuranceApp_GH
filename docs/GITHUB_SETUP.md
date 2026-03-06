# Push This Repo to GitHub

This repo is set up to push to both **Azure DevOps** (`origin`) and **GitHub** (remote `github`).

## One-time setup

1. **Create a repository on GitHub** (if you haven’t already):  
   https://github.com/new  
   Name it e.g. `AutoInsuranceApp` and leave it empty (no README, .gitignore, or license).

2. **Point the `github` remote at your repo** (replace with your org/user and repo name):

   ```bash
   git remote set-url github https://github.com/YOUR_ORG_OR_USER/AutoInsuranceApp.git
   ```

   Or with SSH:

   ```bash
   git remote set-url github git@github.com:YOUR_ORG_OR_USER/AutoInsuranceApp.git
   ```

3. **Push branches to GitHub**:

   ```bash
   # Push development (and create it on GitHub if needed)
   git push -u github development

   # Optionally push main and release
   git push github main
   git push github release
   ```

## Remotes

- **origin** – Azure DevOps: `https://dev.azure.com/WPAzureDevOpsTrng/AutoInsuranceApp/_git/AutoInsuranceApp`
- **github** – GitHub: set with the command above

## Later: push updates to both

```bash
git push origin development    # Azure DevOps
git push github development    # GitHub
```

Or push to both in one go:

```bash
git push origin development && git push github development
```
