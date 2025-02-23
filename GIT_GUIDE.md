# Git Workflow Guide

## Pull
Use this before you start new changes to ensure your local code is up-to-date.

```bash
git checkout main
git fetch origin
git pull origin main
git checkout <working_branch>
git rebase main
```

### Logic of Pull
1. **Fetch and pull** changes from the remote `main` branch to your local `main` branch.
2. **Apply pulled changes** into your local working branch using `rebase`.

---

## Push
Use this after completing your current changes (assumes you're on your own local working branch).

```bash
git checkout main
git fetch origin
git pull origin main

git checkout <working_branch>
git rebase main

git status
git add . // suggested using 'git add <file>' to add files separately.
git commit -m "Your message"

git push origin <working_branch>
```

#### <After Merging on GitHub (via the Website)>

```bash
git checkout main
git pull origin main
git checkout <working_branch>
git rebase main
```
### Logic of Push
1. **Synchronize** your local `main` branch with the latest changes from the remote.
2. **Rebase** your working branch to incorporate the latest changes from `main`.
3. **Stage, commit, and push** your changes to the remote repository.
4. **Merge** your working branch into the remote `main` branch via GitHub.
5. **Update local branches** after merging by pulling the latest changes into `main` and rebasing your working branch.

