using NSwag;
using NSwag.CodeGeneration.CSharp;
using System.IO;
using System.Threading.Tasks;

namespace YnabMcpServer.Generated;

/// <summary>
/// Helper class to generate the YNAB API client from the OpenAPI specification.
/// </summary>
public static class YnabApiClientGenerator
{
    /// <summary>
    /// Generates the YNAB API client from the OpenAPI specification.
    /// </summary>
    /// <param name="specFilePath">The path to the OpenAPI specification file.</param>
    /// <param name="outputFilePath">The path to the output file.</param>
    /// <param name="className">The name of the generated client class.</param>
    /// <param name="namespace">The namespace for the generated code.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task GenerateClientAsync(
        string specFilePath,
        string outputFilePath,
        string className = "YnabClient",
        string @namespace = "YnabMcpServer.Generated.YnabApi")
    {
        // Load the OpenAPI document
        var document = await OpenApiDocument.FromFileAsync(specFilePath);

        // Configure the C# client generator
        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = className,
            CSharpGeneratorSettings =
            {
                Namespace = @namespace,
                GenerateDataAnnotations = true,
                GenerateNullableReferenceTypes = true,
                GenerateOptionalParameters = true,
                DateType = "System.DateTimeOffset",
            },
            UseBaseUrl = false,
            GenerateClientInterfaces = true,
            GenerateDtoTypes = true,
            GenerateClientClasses = true,
            ExposeJsonSerializerSettings = true,
            ClientClassAccessModifier = "public",
            AdditionalNamespaceUsages = new[] { "System.Net.Http", "System.Threading.Tasks" },
            UseHttpClientCreationMethod = true,
        };

        // Generate the client
        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();

        // Write the client code to a file
        await File.WriteAllTextAsync(outputFilePath, code);
    }
}
