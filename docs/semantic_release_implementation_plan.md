# Semantic Release Implementation Plan

This document provides a step-by-step checklist for implementing semantic-release in the YnabMcpServer project. The implementation automates version management, changelog generation, and the release process.

## Setup Checklist

### 1. ✅ Basic Requirements

- [x] Create package.json with semantic-release and plugins
- [x] Configure .releaserc.json with semantic-release plugins
- [x] Add scripts for version management
- [x] Create initial versioning in project file

### 2. ✅ Version Management Scripts

- [x] Create PowerShell script to update .csproj version (update-version.ps1)
- [x] Create PowerShell script for Docker image versioning using .NET 9's built-in container support (publish-docker.ps1)
- [x] Configure semantic-release to use these scripts

### 3. ✅ Commit Message Convention

- [x] Set up Conventional Commits standard
- [x] Add commitlint configuration
- [x] Configure husky for commit message validation
- [x] Create documentation on commit message format

### 4. ✅ CI/CD Configuration

- [x] Create GitHub Actions workflow for semantic-release
- [x] Set up Docker Hub integration
- [x] Configure environment variables and secrets

### 5. ✅ Documentation

- [x] Create VERSIONING.md with guidelines
- [x] Update README.md with reference to versioning
- [x] Add initial CHANGELOG.md

## Required GitHub Secrets

The following secrets must be set up in your GitHub repository:

- `GH_TOKEN`: A GitHub Personal Access Token with repo scope
- `DOCKER_USERNAME`: Your Docker Hub username
- `DOCKER_PAT`: Your Docker Hub Personal Access Token

## First-Time Setup

1. Make sure all files are committed to the repository
2. Install local dependencies:
   ```powershell
   npm install
   ```
3. Make sure husky hooks are executable:
   ```powershell
   chmod +x .husky/*
   ```
4. Configure the required GitHub repository secrets

## Normal Development Workflow

1. Make code changes
2. Commit with conventional commit format:
   ```
   type(scope): description
   ```
3. Push to the main branch
4. Semantic Release automatically:
   - Determines the next version
   - Generates changelog
   - Tags the release
   - Updates version in project files
   - Builds and pushes Docker image

## Manual Release Trigger

If you need to manually trigger a release:

1. Go to GitHub Actions tab
2. Select the "Semantic Release" workflow
3. Click "Run workflow" button
4. Choose the branch (usually main)
5. Confirm

## Troubleshooting

If a release fails:
1. Check the GitHub Actions logs
2. Verify all required secrets are set correctly
3. Ensure commit messages follow the conventional commit format
4. Check if the update-version.ps1 script can access the .csproj file

## Additional Resources

- [Semantic Release Documentation](https://github.com/semantic-release/semantic-release)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Husky](https://typicode.github.io/husky/)
- [Commitlint](https://commitlint.js.org/)
