# Automating YNAB MCP Server Releases with GitHub Actions and Semantic Versioning

_Date: May 14, 2025_

In my [previous article](building-ynab-mcp-server-blog.md), I walked through the development of an MCP server for YNAB using .NET 9 and AI-powered development. Now, let's dive into how I automated the release process using GitHub Actions, semantic versioning, and Docker Hub deployment to make the project sustainable and professional.

A robust CI/CD pipeline is critical for any modern software project. For the YNAB MCP Server, I wanted to ensure:

- Consistent versioning based on commit types
- Automatic changelog generation
- Easy release tagging
- Streamlined Docker image deployment

Let's walk through how I set this up, with practical examples from the project.

## The Power of Conventional Commits

The foundation of my automated release process is the [Conventional Commits](https://www.conventionalcommits.org/) specification. This structured commit message format provides a simple yet powerful way to communicate changes in your codebase.

### How Conventional Commits Work

Each commit message follows this format:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

The `type` categorizes the change:

- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation changes
- `style`: Formatting changes
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `perf`: Performance improvements
- `test`: Adding or correcting tests
- `chore`: Maintenance tasks

For example, a commit message might look like:

```
feat(api): add GetCategoryDetails endpoint

Added a new MCP tool that provides detailed information about a specific category.
This includes month-by-month budgeted and activity amounts.

Closes #123
```

This structured approach provides several benefits:

1. **Clear communication**: Team members can quickly understand the purpose of each change
2. **Automatic versioning**: Tools can analyze commits to determine the appropriate version bump
3. **Automatic changelog**: Well-structured commits can be automatically compiled into readable changelogs

### Project Setup with Development Dependencies

To implement the automated release pipeline, I first set up the necessary dependencies in my project. Here are the key packages that power the automation:

- **commitizen** and **cz-conventional-changelog**: Provide an interactive CLI for creating properly formatted commit messages
- **@commitlint/cli** and **@commitlint/config-conventional**: Enforce commit message formatting rules
- **semantic-release**: Automates version management based on commit history
- **@semantic-release/changelog**: Generates and updates the CHANGELOG.md file
- **@semantic-release/exec**: Allows running custom scripts during the release process
- **@semantic-release/git**: Commits updated files back to the repository
- **husky**: Sets up Git hooks for pre-commit validation

I also added useful npm scripts in package.json:

```json
"scripts": {
  "commit": "git-cz",
  "semantic-release": "semantic-release"
}
```

### Using Commitizen for Consistent Formatting

With Commitizen configured in the project, creating properly formatted commits becomes straightforward. Instead of using the standard `git commit` command, team members run:

```powershell
npm run commit
```

This executes the `git-cz` command defined in the package.json scripts section, launching an interactive prompt that guides developers through creating a conventional commit message:

1. First, select the type of change (feat, fix, docs, etc.)
2. Then enter the scope (optional)
3. Provide a short description
4. Add a longer description if needed
5. Indicate if it's a breaking change
6. Reference issues being closed

This interactive approach ensures all commits follow the conventional structure, eliminating inconsistencies and training new contributors on the correct format.

## Semantic Versioning Automation with semantic-release

With a foundation of conventional commits in place, I implemented [semantic-release](https://github.com/semantic-release/semantic-release) to automate version management. This tool analyzes commit messages to determine when and how to create a new release:

- `fix` commits trigger a PATCH release (1.0.0 -> 1.0.1)
- `feat` commits trigger a MINOR release (1.0.0 -> 1.1.0)
- `feat` commits with a `BREAKING CHANGE` footer trigger a MAJOR release (1.0.0 -> 2.0.0)

### Configuring semantic-release

The semantic-release configuration is defined in a `.releaserc.json` file at the root of the repository:

```json
// .releaserc.json
{
  "branches": ["main"],
  "plugins": [
    "@semantic-release/commit-analyzer",
    "@semantic-release/release-notes-generator",
    [
      "@semantic-release/changelog",
      {
        "changelogFile": "CHANGELOG.md"
      }
    ],
    [
      "@semantic-release/exec",
      {
        "prepareCmd": "pwsh -File ./scripts/update-version.ps1 ${nextRelease.version}",
        "publishCmd": "pwsh -File ./scripts/publish-docker.ps1 ${nextRelease.version}"
      }
    ],
    [
      "@semantic-release/git",
      {
        "assets": ["CHANGELOG.md", "package.json", "Directory.Build.props"],
        "message": "chore(release): ${nextRelease.version} [skip ci]\n\n${nextRelease.notes}"
      }
    ],
    "@semantic-release/github"
  ]
}
```

Let's break down what each part of this configuration does:

1. **branches**: Specifies that releases should only be created from the `main` branch.

2. **plugins**: The series of plugins that control the release process:
   - `@semantic-release/commit-analyzer`: Analyzes commits to determine the version bump type (patch, minor, major).
   - `@semantic-release/release-notes-generator`: Generates release notes based on commit messages.
   - `@semantic-release/changelog`: Updates the CHANGELOG.md file with the new release notes.
   - `@semantic-release/exec`: Executes custom commands at specific points in the release process:
     - `prepareCmd`: Runs the update-version.ps1 script with the new version number to update Directory.Build.props.
     - `publishCmd`: Runs the publish-docker.ps1 script to build and publish the Docker image with the appropriate tag.
   - `@semantic-release/git`: Commits the updated files back to the repository with a standardized release message.
   - `@semantic-release/github`: Creates a GitHub release with the generated release notes.

This configuration provides a completely automated workflow: when commits are pushed to the main branch, semantic-release analyzes them, determines if a new version is needed, generates the changelog, updates the version in .NET project files, commits these changes back to the repository, creates a GitHub release, and publishes a Docker image—all without manual intervention.

For commit linting, I use a separate configuration:

```javascript
// commitlint.config.js
module.exports = {
  extends: ["@commitlint/config-conventional"],
  rules: {
    "body-max-line-length": [2, "always", 100],
    "footer-max-line-length": [2, "always", 100],
    "header-max-length": [2, "always", 100],
    "scope-case": [2, "always", "lower-case"],
  },
};
```

This ensures all commits are properly linted and adhere to the conventional commits standard.

### Updating Version Information in .NET Projects

For .NET projects, we need to update the version in the `Directory.Build.props` file. I created a PowerShell script (update-version.ps1) that integrates with semantic-release to update this automatically:

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <Version>1.0.0</Version>
    <Authors>Chuck Bryan</Authors>
    <Product>YNAB MCP Server</Product>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <RepositoryUrl>https://github.com/ChuckBryan/YnabMcpServer</RepositoryUrl>
  </PropertyGroup>
</Project>
```

When semantic-release determines a new version, it calls my update-version.ps1 script to modify this file, ensuring all assemblies and packages reference the correct version.

## Automating Everything with GitHub Actions

With the foundational tools in place, I created a GitHub Actions workflow to automate the entire process. The workflow runs on push to the main branch and handles:

1. Building and testing the project
2. Analyzing commits for semantic-release
3. Creating a GitHub release with proper tags
4. Updating the CHANGELOG.md
5. Building and publishing a Docker image

### The GitHub Actions Workflow

Here's a simplified version of the workflow:

```yaml
name: Release

on:
  push:
    branches:
      - main

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v3
        with:
          fetch-depth: 0

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 9.0.x

      - name: Install dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Test
        run: dotnet test --no-restore

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: 18

      - name: Install semantic-release
        run: npm install

      - name: Run semantic-release
        env:
          GITHUB_TOKEN: ${{ secrets.GH_TOKEN }}
        run: npx semantic-release

      - name: Build and publish Docker image
        if: steps.release.outputs.new_release_published == 'true'
        env:
          DOCKER_HUB_TOKEN: ${{ secrets.DOCKER_HUB_TOKEN }}
          VERSION: ${{ steps.release.outputs.new_release_version }}
        run: ./scripts/publish-docker.ps1
```

This workflow ensures that every push to the main branch is evaluated for release potential based on the commits since the last release.

## Automating Docker Deployment

For the YNAB MCP Server, easy distribution via Docker Hub is essential. I created a PowerShell script that leverages .NET 9's SDK container support to build and publish the Docker image:

```powershell
# scripts/publish-docker.ps1

param (
    [Parameter(Mandatory=$false)]
    [string]$Version = "latest"
)

# Get the version from Directory.Build.props if not provided
if ($Version -eq "latest") {
    $xml = [xml](Get-Content .\Directory.Build.props)
    $Version = $xml.Project.PropertyGroup.Version
}

Write-Host "Publishing Docker image for version $Version"

# Login to Docker Hub
echo $env:DOCKER_HUB_TOKEN | docker login -u swampyfox --password-stdin

# Build and push for multiple platforms
dotnet publish ./src/YnabMcpServer/YnabMcpServer.csproj `
    -c Release `
    --os linux `
    --arch x64,arm64 `
    -p:PublishProfile=DefaultContainer `
    -p:ContainerRepository=swampyfox/ynabmcp `
    -p:ContainerImageTag=$Version `
    --push
```

This script:

1. Gets the current version from `Directory.Build.props` (which was updated by semantic-release)
2. Logs into Docker Hub using a secure token
3. Uses `dotnet publish` with .NET 9's container support to build multi-architecture images
4. Pushes the images to Docker Hub

The `EnableSdkContainerSupport` property in the csproj file eliminates the need for a custom Dockerfile:

```xml
<PropertyGroup>
  <EnableSdkContainerSupport>true</EnableSdkContainerSupport>
</PropertyGroup>
```

This streamlined approach means I don't need to maintain a separate Dockerfile or worry about containerization details—.NET 9 handles it all.

## Benefits of This Approach

Implementing this automated release system has delivered several key benefits:

### 1. Consistent Versioning

Every release follows semantic versioning principles, making dependencies clear and ensuring compatibility between components. Users always know what to expect from an update based on the version number.

### 2. Comprehensive Changelog

The CHANGELOG.md file is automatically generated and updated with each release, providing a detailed history of changes categorized by type:

```markdown
# Changelog

## [1.2.0] - 2025-04-30

### Features

- **api:** add GetCategoryDetails endpoint (#123)
- **analysis:** implement income vs expense summary tool (#125)

### Bug Fixes

- handle null budget response from YNAB API (#124)
- correct date format in transaction search (#126)

## [1.1.0] - 2025-04-15

...
```

This makes it easy for users to understand what has changed between versions without digging through commit history.

### 3. Reduced Manual Work

The entire release process runs automatically whenever necessary, freeing up developer time for more valuable tasks. There's no need to:

- Manually update version numbers
- Compile release notes
- Create GitHub releases
- Build and push Docker images

All of these happen automatically based on the commit history.

### 4. Improved Project Quality

With commit linting and structured messages, overall project quality has improved. Contributors are guided toward providing meaningful, informative commit messages that clearly document changes.

### 5. Easy Deployment

The combination of semantic-release and the Docker publishing script ensures that every release is immediately available as a Docker image, making it easy for users to adopt the latest version.

## Tips for Implementing Your Own Automated Release Pipeline

If you're considering setting up a similar system for your own projects, here are some tips:

1. **Start with conventional commits**: Even before implementing the full automation, get your team comfortable with structured commit messages.

2. **Store secrets securely**: Use GitHub secrets for sensitive tokens like Docker Hub credentials or GitHub PATs.

3. **Test the workflow locally**: Before pushing to GitHub, test the semantic-release process in dry-run mode to ensure it behaves as expected.

4. **Document the process**: Create clear guidelines for contributors on how to format commits and what to expect from the automated process.

5. **Monitor early releases**: Keep an eye on the first few automated releases to ensure everything is working correctly.

## Conclusion

Automating the release process for the YNAB MCP Server has transformed it from a personal project to a professional, sustainable open-source tool. The combination of conventional commits, semantic-release, and GitHub Actions creates a powerful workflow that ensures consistent versioning, comprehensive documentation, and easy distribution.

For .NET developers, the addition of SDK container support in .NET 9 makes Docker deployment particularly straightforward, eliminating the need for custom Dockerfiles and complex build processes.

If you're managing a similar project, I highly recommend investing the time to set up this kind of automation. The upfront cost is quickly repaid through time savings and improved project quality.

---

_The full source code for the YNAB MCP Server, including all automation scripts and workflows, is available on [GitHub](https://github.com/ChuckBryan/YnabMcpServer) under the MIT license._
