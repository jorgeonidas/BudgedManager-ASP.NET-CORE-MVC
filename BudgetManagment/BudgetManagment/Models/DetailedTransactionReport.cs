
namespace BudgetManagment.Models
{
    public class DetailedTransactionReport
    {
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public IEnumerable<TransactionPerDate> GroupedTransactions { get; set; }
        public decimal DepositsBalance => GroupedTransactions.Sum(x => x.DepositsBalance);
        public decimal WithdrawalsBalance => GroupedTransactions.Sum(x => x.WithdrawalsBalance);
        public decimal Total => DepositsBalance - WithdrawalsBalance;
        public class TransactionPerDate
        {
            public DateTime TransactionDate { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }
            public decimal DepositsBalance => 
                Transactions.Where(x => x.OperationTypeId == OperationType.Income)
                .Sum(x => x.Amount);
            public decimal WithdrawalsBalance => 
                Transactions.Where(x => x.OperationTypeId == OperationType.Expense)
                .Sum(x => x.Amount);
        }
    }
}
