# Version Management and Release Process

This project uses [semantic-release](https://github.com/semantic-release/semantic-release) to automate version management and the release process. This ensures consistent versioning based on commit messages and streamlines the release workflow.

## Commit Message Convention

We follow the [Conventional Commits](https://www.conventionalcommits.org/) specification for commit messages. This standardizes commit messages and makes them more readable and useful.

### Format

```
<type>([optional scope]): <description>

[optional body]

[optional footer(s)]
```

### Types

- `feat`: A new feature (corresponds to MINOR version increment)
- `fix`: A bug fix (corresponds to PATCH version increment)
- `docs`: Documentation only changes
- `style`: Changes that don't affect code logic (formatting, etc.)
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `perf`: Performance improvements
- `test`: Adding or correcting tests
- `build`: Changes to build system or dependencies
- `ci`: Changes to CI configuration
- `chore`: Other changes that don't modify src or test files

### Breaking Changes

Add `BREAKING CHANGE:` in the footer to indicate a breaking change (corresponds to MAJOR version increment):

```
feat: allow provided config object to extend other configs

BREAKING CHANGE: `extends` key in config file is now used for extending other config files
```

## Automatic Versioning

Based on these commit messages, semantic-release will:

1. Determine the next version number
2. Generate a CHANGELOG.md
3. Create a GitHub release
4. Tag the release in Git
5. Update version in the project files
6. Build and publish the Docker image with proper version tags using .NET 9's built-in container support

## Local Development

To ensure commit messages follow the convention:

1. Install Node.js dependencies: `npm install`
2. The pre-commit hook will validate your commit messages automatically

## Running a Manual Release

Releases are automatically triggered when changes are pushed to the main branch. However, you can manually trigger a release using GitHub Actions workflow dispatch.

## Version Information

The current version is maintained in:
- The project's .csproj file (Version property)
- Git tags
- Docker image tags on Docker Hub
