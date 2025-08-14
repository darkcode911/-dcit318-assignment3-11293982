using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing bank transfer for ${transaction.Amount} categorized as '{transaction.Category}'.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing mobile money payment for ${transaction.Amount} categorized as '{transaction.Category}'.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing crypto wallet transaction for ${transaction.Amount} categorized as '{transaction.Category}'.");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Transaction failed: Insufficient funds.");
            }
            else
            {
                base.ApplyTransaction(transaction);
                Console.WriteLine($"Transaction applied. New balance: ${Balance:F2}");
            }
        }
    }

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            var mySavings = new SavingsAccount("SA-001", 1000m);
            Console.WriteLine($"Account {mySavings.AccountNumber} created with initial balance: ${mySavings.Balance:F2}\n");

            var transactions = new List<Transaction>
            {
                new Transaction(1, DateTime.Now, 75.50m, "Groceries"),
                new Transaction(2, DateTime.Now, 120.00m, "Utilities"),
                new Transaction(3, DateTime.Now, 50.25m, "Entertainment")
            };

            var mobileProcessor = new MobileMoneyProcessor();
            var bankProcessor = new BankTransferProcessor();
            var cryptoProcessor = new CryptoWalletProcessor();

            Console.WriteLine("--- Transaction 1 ---");
            mobileProcessor.Process(transactions[0]);
            mySavings.ApplyTransaction(transactions[0]);
            Console.WriteLine();

            Console.WriteLine("--- Transaction 2 ---");
            bankProcessor.Process(transactions[1]);
            mySavings.ApplyTransaction(transactions[1]);
            Console.WriteLine();

            Console.WriteLine("--- Transaction 3 ---");
            cryptoProcessor.Process(transactions[2]);
            mySavings.ApplyTransaction(transactions[2]);
            Console.WriteLine();

            _transactions.AddRange(transactions);

            Console.WriteLine($"\nAll transactions processed. Final balance: ${mySavings.Balance:F2}");
            Console.WriteLine($"Total transactions recorded: {_transactions.Count}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}