using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPO_app2
{
    public interface IBankAccountFacade
    {
        void ChangeAccountName(Guid id, string name);
        void DeleteAccount(Guid id);
        BankAccount GetAccount(Guid id);
        IEnumerable<BankAccount> GetAllAccounts();
        public void ChangeBalance(Guid id, decimal new_balance);
    }

    public class BankAccountFacade : IBankAccountFacade
    {
        private readonly IStorage<BankAccount> accountRepository;
        private readonly IBankFactory factory;

        public BankAccountFacade(IStorage<BankAccount> _accountRepository, IBankFactory _factory)
        {
            accountRepository = _accountRepository;
            factory = _factory;
        }
        public void ChangeAccountName(Guid id, string name)
        {
            var acc = accountRepository.GetById(id);
            if (acc != null) acc.ChangeName(name); accountRepository.Update(acc);
        }
        public void DeleteAccount(Guid id) => accountRepository.Delete(id);
        public BankAccount GetAccount(Guid id) => accountRepository.GetById(id);
        public IEnumerable<BankAccount> GetAllAccounts() => accountRepository.GetAll();

        public void ChangeBalance(Guid id, decimal new_balance)
        {
            var acc = accountRepository.GetById(id);
            decimal change_num = acc.balance - new_balance;
            if (acc != null) acc.ChangeBalance(change_num); accountRepository.Update(acc);
        }
    }
}
