<h1 align="center">CONTRIBUTING</h1>

## Ember Developer Guide
Welcome to Ember Contributing Guidelines. Follow these when you are thinking about contributing to Ember project. We welcome your contributions to Ember!

## Quick Reference
Here are the basic steps needed to get set up and contribute a patch.
1. Install and set up [Git](https://git-scm.com/).
2. **Fork** the Ember repository to your GitHub account and get the source code using:

```bash
git clone --recursive https://github.com/<your_username>/Ember.git
cd Ember
```
3. Windows users can run the `GenerateProjects.bat` to generate project files. 
    
   If you are getting errors, make sure you cloned the repo using `--recurse` option. If you are still getting errors (might because you are using an old git version), run `git submodule init` and `git submodule update` in the ember directory, and see if that resolves your issue, otherwise you can open an issue providing your git logs.
4. Open the generated `Ember.sln` in Visual Studio.
5. Create a new branch where your work for the issue will go, format it `fix-issue-#`, e.g.:
    ```
    git checkout -b fix-issue-12345 master
    ```
    If an issue does not already exist, please [create](https://github.com/billyeatcookies/Ember/issues) it. Trivial issues (e.g. typo fixes) do not require any issue to be created.
6. Once you fixed the issue, try building the solution, and check no errors are occuring, if everything is ok, commit.
7. Push the branch on your fork on GitHub and create a pull request. Include the issue number with format `Mentioned-Issue: ##` in the pull request description. For example:
    ```
    Mentioned-Issue: #12345: Fix x bug in y module
    ```
8. And you are done!
