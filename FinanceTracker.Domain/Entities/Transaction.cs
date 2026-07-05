using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceTracker.Domain.Entities.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceTracker.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public DateTimeOffset CreatedDate { get; private set; }
        public string? Description { get; private set; }

        public Transaction(decimal amount, TransactionType transactionType, string description, Guid accountId)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            TransactionType = transactionType;
            Description = description;
            AccountId = accountId;
            CreatedDate = DateTimeOffset.UtcNow;
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription;
        }

        private Transaction() { }

        public Account Account { get; private set; } = null!;
    }
}
