using BudgetManagment.Models;
using Dapper;
using Microsoft.Data.SqlClient;
namespace BudgetManagment.Services
{
    public class RepositoryTransactions : IRepositoryTransactions
    {
        private readonly string _connectionString;
        public RepositoryTransactions(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>("Transactions_Insert",
                new
                {
                    transaction.UserId,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.Note,
                    transaction.AccountId
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaction.Id = id;
        }

        public async Task<IEnumerable<Transaction>> GetByAccounId(GetTransactionsByAccount model)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Transaction>(
                                                @"SELECT t.Id, t.Amount, t.TransactionDate, c.Name as Category,
                                                accounts.Name as Account, c.OperationTypeId
                                                FROM Transactions t
                                                INNER JOIN Categories c
                                                ON c.Id = t.CategoryId
                                                INNER JOIN Accounts accounts
                                                ON accounts.Id = t.AccountId
                                                WHERE t.AccountId = @AccountId AND t.UserId = @UserId
                                                AND TransactionDate BETWEEN @StartDate and @FinishDate",
                                               model);
        }

        public async Task<IEnumerable<Transaction>> GetByUserId(TransasctionsPerUserParameters model)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Transaction>(
                                                @"SELECT t.Id, t.Amount, t.TransactionDate, c.Name as Category,
                                                accounts.Name as Account, c.OperationTypeId, Note
                                                FROM Transactions t
                                                INNER JOIN Categories c
                                                ON c.Id = t.CategoryId
                                                INNER JOIN Accounts accounts
                                                ON accounts.Id = t.AccountId
                                                WHERE t.UserId = @UserId
                                                AND TransactionDate BETWEEN @StartDate and @FinishDate
                                                ORDER BY t.TransactionDate DESC",
                                               model);
        }

        public async Task Update(Transaction transaction, decimal previousAmount, int previousAccountId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("Transactions_Update",
                new
                {
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.Note,
                    transaction.AccountId,
                    previousAmount,
                    previousAccountId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaction> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(
                @"SELECT Transactions.*, cat.OperationTypeId
                FROM Transactions
                INNER JOIN Categories cat
                ON cat.Id = Transactions.CategoryId
                WHERE Transactions.Id = @Id AND Transactions.UserId = @UserId",
                new { id, userId }
            );
        }

        public async Task<IEnumerable<ResultByWeek>> GetResultsByWeek(TransasctionsPerUserParameters model)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ResultByWeek>(@"SELECT DATEDIFF(d,@startDate,TransactionDate) / 7 + 1 as WeekDays,
                                    SUM(Amount) as Amount, cat.OperationTypeId
                                    FROM Transactions
                                    INNER JOIN Categories cat
                                    ON cat.Id = Transactions.CategoryId
                                    WHERE Transactions.UserId = @userId AND
                                    TransactionDate BETWEEN @startDate AND @finishDate
                                    GROUP BY DATEDIFF(d,@startDate,TransactionDate) / 7, cat.OperationTypeId", model);
        }

        public async Task<IEnumerable<GetByMonthResult>> GetByMonth(int userId, int year)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection .QueryAsync<GetByMonthResult>(@"SELECT MONTH(TransactionDate) as Month,
                                SUM(Amount) as Amount, cat.OperationTypeId
                                FROM Transactions
                                INNER JOIN Categories cat
                                ON	cat.Id = Transactions.CategoryId
                                WHERE Transactions.UserId = @userId AND YEAR(TransactionDate) = @year
                                GROUP BY MONTH(TransactionDate), cat.OperationTypeId", new { userId, year });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("Transactions_Delete", 
                                            new { id }, 
                                            commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
