using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPO_app2
{
    public interface IBankFactory
    {
        BankAccount MakeBankAccount (string name, decimal balance);
        Category MakeCategory (bool type, string name);
        Operation MakeOperation(bool type, Guid bank_account_id, decimal amount, DateTime date, Guid category_id, string description, IStorage<BankAccount> bankFacade);
    }
    public class BankFactory : IBankFactory
    {
        public BankAccount MakeBankAccount(string name, decimal balance = 0)
        {
            if (string.IsNullOrEmpty(name)) return null; // throw new ArgumentNullException("you can't have empty name");
            if (balance < 0) return null;//throw new ArgumentOutOfRangeException("balance can't be negative");
            return new BankAccount(Guid.NewGuid(), name, balance);
        }
        public Category MakeCategory(bool type, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("you can't have empty name");
            return new Category(Guid.NewGuid(), type, name);
        }
        public Operation MakeOperation(bool type, Guid bank_account_id, decimal amount, DateTime date, Guid category_id, string description, IStorage<BankAccount> bankFacade)
        {
            if (DateTime.Now < date) return null; // throw new ArgumentException("wrong date (places in the future)");
            bankFacade.GetById(bank_account_id).ChangeBalance(type? amount: -amount);
            return new Operation(Guid.NewGuid(), type, bank_account_id, amount, date, category_id, description);
        }
    }
}
