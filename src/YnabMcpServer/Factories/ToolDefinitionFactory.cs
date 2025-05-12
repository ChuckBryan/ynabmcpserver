using ModelContextProtocol.Contracts;
using System.Text.Json;

namespace YnabMcpServer.Factories;

/// <summary>
/// Factory class to create standardized MCP tool definitions
/// </summary>
public static class ToolDefinitionFactory
{
    /// <summary>
    /// Creates a tool definition for a budget-related tool
    /// </summary>
    public static ToolDefinition CreateBudgetTool(string description, bool requiresBudgetId = true, object? additionalProperties = null)
    {
        var properties = new Dictionary<string, object>();

        if (requiresBudgetId)
        {
            properties["budgetId"] = new
            {
                type = "string",
                description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
            };
        }

        if (additionalProperties != null)
        {
            foreach (var property in additionalProperties.GetType().GetProperties())
            {
                properties[property.Name] = property.GetValue(additionalProperties)!;
            }
        }

        var schema = new
        {
            type = "object",
            properties = properties,
            required = requiresBudgetId ? new[] { "budgetId" } : Array.Empty<string>()
        };

        return new ToolDefinition
        {
            Description = description,
            InputSchema = JsonSerializer.SerializeToDocument(schema)
        };
    }

    /// <summary>
    /// Creates a tool definition for an entity-specific tool (like account or category)
    /// </summary>
    public static ToolDefinition CreateEntityTool(string description, string entityName, string entityIdDescription)
    {
        var idField = $"{entityName.ToLowerInvariant()}Id";

        return new ToolDefinition
        {
            Description = description,
            InputSchema = JsonSerializer.SerializeToDocument(new
            {
                type = "object",
                properties = new Dictionary<string, object>
                {
                    ["budgetId"] = new
                    {
                        type = "string",
                        description = "The ID of the budget. Use 'last-used' for the last used budget or 'default' for the default budget."
                    },
                    [idField] = new
                    {
                        type = "string",
                        description = entityIdDescription
                    }
                },
                required = new[] { "budgetId", idField }
            })
        };
    }
}
