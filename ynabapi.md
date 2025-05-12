# YNAB API Endpoints Reference

| Method | Endpoint | Name | Parameters |
|--------|----------|------|------------|
| GET | `/user` | Get User Info | None - Requires authentication header |
| GET | `/budgets` | List Budgets | Query: `include_accounts` (boolean, optional) |
| GET | `/budgets/{budget_id}` | Get Budget | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/settings` | Get Budget Settings | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/accounts` | List Accounts | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/accounts/{account_id}` | Get Account | Path: `budget_id` (UUID, required), `account_id` (UUID, required) |
| POST | `/budgets/{budget_id}/accounts` | Create Account | Path: `budget_id` (UUID, required), Body: SaveAccount object |
| GET | `/budgets/{budget_id}/categories` | List Categories | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/categories/{category_id}` | Get Category | Path: `budget_id` (UUID, required), `category_id` (UUID, required) |
| PATCH | `/budgets/{budget_id}/categories/{category_id}` | Update Category | Path: `budget_id` (UUID, required), `category_id` (UUID, required), Body: SaveCategory object |
| GET | `/budgets/{budget_id}/months/{month}/categories/{category_id}` | Get Month Category | Path: `budget_id` (UUID, required), `month` (ISO date, required), `category_id` (UUID, required) |
| GET | `/budgets/{budget_id}/payees` | List Payees | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/payees/{payee_id}` | Get Payee | Path: `budget_id` (UUID, required), `payee_id` (UUID, required) |
| GET | `/budgets/{budget_id}/payee_locations` | List Payee Locations | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/payees/{payee_id}/payee_locations` | List Payee's Locations | Path: `budget_id` (UUID, required), `payee_id` (UUID, required) |
| GET | `/budgets/{budget_id}/payee_locations/{payee_location_id}` | Get Payee Location | Path: `budget_id` (UUID, required), `payee_location_id` (UUID, required) |
| GET | `/budgets/{budget_id}/transactions` | List Transactions | Path: `budget_id` (UUID, required), Query: `since_date` (ISO date, optional), `type` (string, optional), `last_knowledge_of_server` (integer, optional) |
| GET | `/budgets/{budget_id}/transactions/{transaction_id}` | Get Transaction | Path: `budget_id` (UUID, required), `transaction_id` (UUID, required) |
| GET | `/budgets/{budget_id}/accounts/{account_id}/transactions` | List Account Transactions | Path: `budget_id` (UUID, required), `account_id` (UUID, required) |
| GET | `/budgets/{budget_id}/categories/{category_id}/transactions` | List Category Transactions | Path: `budget_id` (UUID, required), `category_id` (UUID, required) |
| GET | `/budgets/{budget_id}/payees/{payee_id}/transactions` | List Payee Transactions | Path: `budget_id` (UUID, required), `payee_id` (UUID, required) |
| POST | `/budgets/{budget_id}/transactions` | Create Transaction | Path: `budget_id` (UUID, required), Body: SaveTransaction object |
| POST | `/budgets/{budget_id}/transactions/bulk` | Bulk Create Transactions | Path: `budget_id` (UUID, required), Body: BulkTransactions object |
| PATCH | `/budgets/{budget_id}/transactions/{transaction_id}` | Update Transaction | Path: `budget_id` (UUID, required), `transaction_id` (UUID, required), Body: SaveTransaction object |
| PATCH | `/budgets/{budget_id}/transactions` | Bulk Update Transactions | Path: `budget_id` (UUID, required), Body: BulkTransactions object |
| POST | `/budgets/{budget_id}/transactions/import` | Import Transactions | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/scheduled_transactions` | List Scheduled Transactions | Path: `budget_id` (UUID, required) |
| GET | `/budgets/{budget_id}/scheduled_transactions/{scheduled_transaction_id}` | Get Scheduled Transaction | Path: `budget_id` (UUID, required), `scheduled_transaction_id` (UUID, required) |
| GET | `/budgets/{budget_id}/months` | List Budget Months | Path: `budget_id` (UUID, required), Query: `last_knowledge_of_server` (integer, optional) |
| GET | `/budgets/{budget_id}/months/{month}` | Get Budget Month | Path: `budget_id` (UUID, required), `month` (ISO date, required) |
| PATCH | `/budgets/{budget_id}/months/{month}/categories/{category_id}` | Update Month Category | Path: `budget_id` (UUID, required), `month` (ISO date, required), `category_id` (UUID, required) |

## Notes

1. All endpoints require authentication via Bearer token in the Authorization header:
   ```
   Authorization: Bearer YOUR_ACCESS_TOKEN
   ```
2. All dates should be in ISO format (e.g., `2016-12-01`)
3. All amounts are in milliunits (e.g., 1000 = 1.00)
4. All UUIDs must be in valid UUID format
5. Base URL: `https://api.ynab.com/v1`
