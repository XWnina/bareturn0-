
# Better Git Workflow Guide

## Quick Walk-Through
### Pull: do so before you are going to code sth
```bash
# Update the local main branch
git checkout main
git pull origin main

# Rebase your working branch on the updated main
git checkout <working-branch>
git rebase main
```
### Push: do so after you coded sth
```bash
# Check files with modification
git status

# Add and commit files 
git add .
git commit -m "Your commit message"

# Conflict check:
# If possible, check if the remote main branch has any changes before push-> if having any, pull it down and merge into the working branch before pushing
# Might meet conflicts
git checkout main
git fetch
git pull 
git checkout <working-branch>
git rebase main

# Push
# Noticed that you don't need to push and merge for every single conflict
git push
```
#### Manual Step: Merge your push commit on github web.
The following are for keeping the updates to local branches, also contains the command to keep your working branch up-to-date with `main` on remote. 
```bash
# Update branches after merging into the main
git checkout main
git pull origin main
git checkout <working-branch>
git rebase main
git push --force
```
---
## Final Check: What to Expect if Everything Is Done Correctly
1. Check your local branches are up-to-date:
Running `git status` on both `main` and `<working-branch>` should return:
```bash
# On branch main
Your branch is up to date with 'origin/main'.
nothing to commit, working tree clean

# On your working branch
Your branch is up to date with 'origin/<working-branch>'.
nothing to commit, working tree clean
```
2. No commits behind/ahead:
Running `git status` and checking on GitHub, should not show messages like:
```bash
Your branch is behind 'origin/main' by X commits
Your branch is ahead of 'origin/main' by X commits
```
Other notes:
* Suggested using `git add <file>` to add files separately(for double checking on the not necessary files pushed).
* `git log --oneline --graph --all` would show a linear commit history without unnecessary merge commits.
* The remote repository should display your latest commits on both the `main` and your `<working-branch>`.
* If meeting conflicts, fix the conflicts first.
* Always use status to check the added/ committed files are correctly selected and at the correct areas
