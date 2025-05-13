# Version Management and Release Process

This project uses [semantic-release](https://github.com/semantic-release/semantic-release) to automate version management and the release process. This ensures consistent versioning based on commit messages and streamlines the release workflow.

## Commit Message Format

We follow the [Conventional Commits](https://www.conventionalcommits.org/) specification for commit messages. This standardizes commit messages and makes them more readable and useful.

### Basic Format

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### Common Types

- `feat`: A new feature (corresponds to a MINOR version increment)
- `fix`: A bug fix (corresponds to a PATCH version increment)
- `docs`: Documentation only changes
- `style`: Changes that don't affect code logic (formatting, etc.)
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `perf`: Performance improvements
- `test`: Adding or correcting tests
- `build`: Changes to build system or dependencies
- `ci`: Changes to CI configuration
- `chore`: Other changes that don't modify src or test files

### Breaking Changes

Add `BREAKING CHANGE:` in the footer to indicate a breaking change (corresponds to a MAJOR version increment):

```
feat: allow provided config object to extend other configs

BREAKING CHANGE: `extends` key in config file is now used for extending other config files
```

## Automated Release Process

When you push commits to the `main` branch, semantic-release automatically:

1. Analyzes commit messages to determine the next version number
2. Generates release notes and updates CHANGELOG.md
3. Updates version in project files (YnabMcpServer.csproj and Directory.Build.props)
4. Creates a Git tag for the new version
5. Creates a GitHub release
6. Builds and publishes Docker images to Docker Hub

## Required GitHub Secrets

For the automated release process to work, the following secrets must be configured in your GitHub repository:

- `GH_TOKEN`: A GitHub Personal Access Token with repository access
- `DOCKER_USERNAME`: Your Docker Hub username
- `DOCKER_PAT`: Your Docker Hub Personal Access Token

## Manual Release

To manually trigger a release, you can use the GitHub Actions workflow dispatch feature from the Actions tab in your repository.
