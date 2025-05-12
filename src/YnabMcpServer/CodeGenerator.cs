using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using YnabMcpServer.Generated;

namespace YnabMcpServer;

/// <summary>
/// Helper class to handle build-time code generation.
/// </summary>
public class CodeGenerator
{
    private readonly ILogger<CodeGenerator> _logger;

    public CodeGenerator(ILogger<CodeGenerator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates the YNAB API client from the OpenAPI specification.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task GenerateYnabClientAsync()
    {
        try
        {
            _logger.LogInformation("Generating YNAB API client...");

            // Get the root directory of the project
            var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            while (!File.Exists(Path.Combine(projectDirectory, "YnabMcpServer.csproj")) &&
                   Directory.GetParent(projectDirectory) != null)
            {
                projectDirectory = Directory.GetParent(projectDirectory).FullName;
            }

            // Define paths
            var rootDirectory = Directory.GetParent(projectDirectory).Parent.FullName;
            var specFilePath = Path.Combine(rootDirectory, "docs", "open_api_spec.yml");
            var outputDirectory = Path.Combine(projectDirectory, "Generated", "YnabApi");
            var outputFilePath = Path.Combine(outputDirectory, "YnabClient.cs");

            // Create the output directory if it doesn't exist
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                _logger.LogInformation("Created output directory: {OutputDirectory}", outputDirectory);
            }

            // Generate the client
            await YnabApiClientGenerator.GenerateClientAsync(
                specFilePath,
                outputFilePath,
                "YnabClient",
                "YnabMcpServer.Generated.YnabApi"
            );

            _logger.LogInformation("YNAB API client generated successfully at {OutputFilePath}", outputFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating the YNAB API client");
            throw;
        }
    }
}
