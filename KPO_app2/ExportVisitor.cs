using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPO_app2
{
    public interface IExportVisitor
    {
        void Visit(BankAccount account);
        void Visit(Category category);
        void Visit(Operation operation);
        string GetResult();
    }

    public class CsvExportVisitor : IExportVisitor
    {
        private readonly StringBuilder _csv = new();
        private bool _headersWritten = false;

        public void Visit(BankAccount account)
        {
            if (!_headersWritten)
            {
                _csv.AppendLine("type,id,name,balance");
                _headersWritten = true;
            }
            _csv.AppendLine($"Account,{account.id},{account.name},{account.balance}");
        }
        public void Visit(Category category)
        {
            if (!_headersWritten)
            {
                _csv.AppendLine("type,id,name,category type");
                _headersWritten = true;
            }
            _csv.AppendLine($"Category,{category.id},{category.name},{category.type}");
        }

        public void Visit(Operation operation)
        {
            if (!_headersWritten)
            {
                _csv.AppendLine("Type,Id,OperationType,Amount,Date,Description,AccountId,CategoryId");
                _headersWritten = true;
            }
            _csv.AppendLine($"Operation,{operation.id},{operation.type},{operation.amount},{operation.date:yyyy-MM-dd},{operation.description},{operation.bank_account_id},{operation.category_id}");
        }

        public string GetResult() => _csv.ToString();
    }


    public class JsonExportVisitor : IExportVisitor
    {
        private readonly List<object> _data = new();

        public void Visit(BankAccount account)
        {
            _data.Add(new { Type = "Account", account.id, account.name, account.balance });
        }

        public void Visit(Category category)
        {
            _data.Add(new { Type = "Category", category.id, category.name, CategoryType = category.type });
        }

        public void Visit(Operation operation)
        {
            _data.Add(new {
                Type = "Operation",
                operation.id,
                OperationType = operation.type,
                operation.amount,
                operation.date,
                operation.description,
                operation.bank_account_id,
                operation.category_id
            });
        }

        public string GetResult() => System.Text.Json.JsonSerializer.Serialize(_data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }
}

