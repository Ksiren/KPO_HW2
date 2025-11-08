using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPO_app2
{
    public interface IOperationFacade
    {
        void ChangeOperDescription (Guid id, string description);
        void DeleteOperation (Guid id);
        Operation GetOperation (Guid id);
        IEnumerable<Operation> GetAccountOperations (Guid id);
        IEnumerable<Operation> GetCategoryOperations(Guid id);
        IEnumerable<Operation> GetDateOperations (DateTime date_s, DateTime date_f);
    }

    public class OperationFacade : IOperationFacade
    {
        private readonly IStorage<Operation> operationRepository;
        private readonly IStorage<BankAccount> accountRepository;
        private readonly IBankFactory factory;

        public OperationFacade (IStorage<Operation> _operationRepository, IStorage<BankAccount> _accountRepository, IBankFactory _factory)
        {
            operationRepository = _operationRepository;
            accountRepository = _accountRepository;
            factory = _factory;
        }
        public void ChangeOperDescription (Guid id, string description)
        {
            var operation = operationRepository.GetById(id);
            if (operation != null) operation.ChangeDescription(description = "-"); operationRepository.Update(operation);
        }
        public void DeleteOperation (Guid id) => operationRepository.Delete(id);
        public Operation GetOperation (Guid id) => operationRepository.GetById(id);
        public IEnumerable<Operation> GetAccountOperations (Guid id) =>  operationRepository.GetAll().Where(i => i.bank_account_id == id);
        public IEnumerable<Operation> GetCategoryOperations(Guid id) => operationRepository.GetAll().Where(i => i.category_id == id);
        public IEnumerable<Operation> GetDateOperations (DateTime date_s, DateTime date_f) => operationRepository.GetAll().Where(i => i.date >= date_s && i.date <= date_f);
    }
}
