# [1.4.0](https://github.com/ChuckBryan/ynabmcpserver/compare/v1.3.0...v1.4.0) (2025-05-13)


### Features

* update Docker publishing configuration and enhance version handling in scripts ([9796e08](https://github.com/ChuckBryan/ynabmcpserver/commit/9796e08cb44b53761f2248d2b2bec8396f9a34ee))

# [1.3.0](https://github.com/ChuckBryan/ynabmcpserver/compare/v1.2.0...v1.3.0) (2025-05-13)


### Features

* enhance Docker publishing script with environment variable logging and verification steps ([78a5813](https://github.com/ChuckBryan/ynabmcpserver/commit/78a581356178b2d2b8c187748d78f8a07116dd64))

# [1.2.0](https://github.com/ChuckBryan/ynabmcpserver/compare/v1.1.0...v1.2.0) (2025-05-13)


### Features

* bump the feature ([45b8ae1](https://github.com/ChuckBryan/ynabmcpserver/commit/45b8ae1fc6bd744da234b4ba4ffc323bb0d043a4))

# [1.1.0](https://github.com/ChuckBryan/ynabmcpserver/compare/v1.0.0...v1.1.0) (2025-05-13)


### Bug Fixes

* update pre-commit hook to skip tests and add success message ([ced413c](https://github.com/ChuckBryan/ynabmcpserver/commit/ced413c2da23e17503cd312626aa51b42f83a4ba))


### Features

* update release workflow and commit-msg hook to skip commitlint for semantic-release ([456d458](https://github.com/ChuckBryan/ynabmcpserver/commit/456d4587cb795010265a14a888f3bdce1e430b41))

# 1.0.0 (2025-05-13)


### Bug Fixes

* bump to build ([a0f46cd](https://github.com/ChuckBryan/ynabmcpserver/commit/a0f46cd6b025bf48dadf729d9549460b3704bed0))
* refactor version update logic in update-version.ps1 for clarity ([cf4577a](https://github.com/ChuckBryan/ynabmcpserver/commit/cf4577aeea0f99f9fecd3391a9e67be237a1050e))
* Remove RuntimeIdentifiers property from project file ([30fde25](https://github.com/ChuckBryan/ynabmcpserver/commit/30fde250c15d8549a2a19e07153c18be27eff54c))
* remove version from csproj file to use Directory.Build.props instead ([ff471a8](https://github.com/ChuckBryan/ynabmcpserver/commit/ff471a88fc235cf559e06400436cf5f300ffb418))
* remove version from csproj to rely on Directory.Build.props for versioning ([7a8071d](https://github.com/ChuckBryan/ynabmcpserver/commit/7a8071d5bca13997ef4ff70a394d23d42a9f2d04))
* Specify exact .NET version in build workflow for consistency ([dfe227c](https://github.com/ChuckBryan/ynabmcpserver/commit/dfe227c5a2d691fd5f1d59083fda668f88775d7e))
* Update .NET version specification for build workflow and remove unused Dockerfile ([eabd2fd](https://github.com/ChuckBryan/ynabmcpserver/commit/eabd2fd13632ab47e8926d3894f2f078116d47f1))
* Update build badge link in README to point to the correct repository ([68f18a0](https://github.com/ChuckBryan/ynabmcpserver/commit/68f18a059bd4627bce71ad8d1e0f8353f5ec563a))


### Features

* Add container support for YNAB MCP Server; configure SDK container settings and runtime identifiers ([50c5e02](https://github.com/ChuckBryan/ynabmcpserver/commit/50c5e0280e5d840e9842729659186535131310d4))
* Add GitHub Actions build workflow and update README with build badge ([bd04807](https://github.com/ChuckBryan/ynabmcpserver/commit/bd04807a35d562046f4c06d687221103d93b95bc))
* Add initial project files for YNAB MCP Server including Program.cs and project configuration ([b69efa2](https://github.com/ChuckBryan/ynabmcpserver/commit/b69efa2c2b61cb12e5f73ef50878cea6138fe02e))
* Add steps to set up and verify NSwag CLI installation in GitHub Actions workflow ([f3d5147](https://github.com/ChuckBryan/ynabmcpserver/commit/f3d5147eee0ff7ac3c1d8063e2bef84177da3f8a))
* Add WireMock.NET implementation plan for YNAB MCP Server integration testing ([d3301b1](https://github.com/ChuckBryan/ynabmcpserver/commit/d3301b1ed2edf97f218850352ce36a6b2a50ff45))
* Add YNAB API environment setup and testing tools; include .env example, create script for .env file, and implement API connection tester ([a808241](https://github.com/ChuckBryan/ynabmcpserver/commit/a80824138cc35e30e510c69819944237bd1975f7))
* Enhance YNAB MCP Server configuration and API integration; add appsettings, refactor client registration, and implement user info retrieval ([99ddec4](https://github.com/ChuckBryan/ynabmcpserver/commit/99ddec4d340d3f2109f81af5f468c58686ed0a2e))
* Implement semantic-release for automated versioning and release process ([4a9257e](https://github.com/ChuckBryan/ynabmcpserver/commit/4a9257eed4da334ab0722f22dcba50cd060ee17e))
* Implement semantic-release for automated versioning and releases ([c416691](https://github.com/ChuckBryan/ynabmcpserver/commit/c41669177ce9b070c9928e5b1677c4c0bdae5d37))
* Implement YNAB API client and configuration; add user info retrieval, budget listing, and error handling ([e7ca228](https://github.com/ChuckBryan/ynabmcpserver/commit/e7ca2285ad23b97fcb18dda49cf494b355cbb944))
* Initialize YNAB MCP Server with core functionality ([ba10976](https://github.com/ChuckBryan/ynabmcpserver/commit/ba109760ad821dbb1e9be7878349abe8ac7e2ce7))
* Install NSwag CLI in GitHub Actions build workflow ([cc51deb](https://github.com/ChuckBryan/ynabmcpserver/commit/cc51deba70ed2b9833a08a177fed0de18040d840))
* Refactor YNAB MCP Server architecture; consolidate service registration, enhance transport handling, and remove obsolete classes ([fa96437](https://github.com/ChuckBryan/ynabmcpserver/commit/fa96437bfc8dbc2344b0791721b554d6eaa75015))
* Update VS Code configuration and README for Docker support; refactor server command and add usage instructions ([c73ad16](https://github.com/ChuckBryan/ynabmcpserver/commit/c73ad167a156e7cc7fc56fc3bebb596cfebcd683))
* Update YNAB API integration to use API token; refactor environment variables, update configuration files, and modify request scripts ([c51c46b](https://github.com/ChuckBryan/ynabmcpserver/commit/c51c46bff79e735680ea4bbae918028c6728c29b))

# Changelog

All notable changes to this project will be documented in this file. This file is automatically generated by semantic-release based on commit messages.

## [Unreleased]
