namespace YnabMcpServer.Models.MCP;

/// <summary>
/// Base class for all tool request models.
/// </summary>
public abstract class ToolRequestBase
{
    /// <summary>
    /// The budget ID to use for the request. Can be "last-used" or "default" or a specific budget ID.
    /// </summary>
    public string BudgetId { get; set; } = "last-used";
}

/// <summary>
/// Budget management tool requests
/// </summary>
public class GetBudgetsRequest : ToolRequestBase
{
    /// <summary>
    /// Whether to include the list of accounts in the response.
    /// </summary>
    public bool IncludeAccounts { get; set; } = false;
}

public class GetBudgetDetailsRequest : ToolRequestBase
{
    // BudgetId inherited from ToolRequestBase
}

public class GetBudgetSettingsRequest : ToolRequestBase
{
    // BudgetId inherited from ToolRequestBase
}

/// <summary>
/// Account management tool requests
/// </summary>
public class GetAccountsRequest : ToolRequestBase
{
    // BudgetId inherited from ToolRequestBase
}

public class GetAccountDetailsRequest : ToolRequestBase
{
    /// <summary>
    /// The account ID to get details for.
    /// </summary>
    public string AccountId { get; set; } = string.Empty;
}

/// <summary>
/// Transaction management tool requests
/// </summary>
public class GetTransactionsRequest : ToolRequestBase
{
    /// <summary>
    /// If specified, only transactions on or after this date will be included.
    /// </summary>
    public DateTimeOffset? SinceDate { get; set; }

    /// <summary>
    /// The transaction type filter. Can be "uncategorized" or "unapproved".
    /// </summary>
    public string? Type { get; set; }
}

public class CreateTransactionRequest : ToolRequestBase
{
    /// <summary>
    /// The transaction data to create.
    /// </summary>
    public Dictionary<string, object> Transaction { get; set; } = new();
}

public class UpdateTransactionRequest : ToolRequestBase
{
    /// <summary>
    /// The transaction ID to update.
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// The transaction data to update.
    /// </summary>
    public Dictionary<string, object> Transaction { get; set; } = new();
}

/// <summary>
/// Category management tool requests
/// </summary>
public class GetCategoriesRequest : ToolRequestBase
{
    // BudgetId inherited from ToolRequestBase
}

public class GetCategoryDetailsRequest : ToolRequestBase
{
    /// <summary>
    /// The category ID to get details for.
    /// </summary>
    public string CategoryId { get; set; } = string.Empty;
}
