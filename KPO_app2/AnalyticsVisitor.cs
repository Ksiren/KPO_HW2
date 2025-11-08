using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPO_app2
{
    public interface IAnalyticsVisitor
    {
        void Visit(FinancialData data);
    }

    public class FinancialData
    {
        public IEnumerable<BankAccount> Accounts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Operation> Operations { get; set; }
        //public DateTime date_s { get; set; }
        //public DateTime date_e { get; set; }
        public FinancialData(IEnumerable<BankAccount>  accounts, IEnumerable<Category> categories, IEnumerable<Operation> operations)//, DateTime _date_s, DateTime _date_e) 
        { 
            Accounts = accounts;
            Categories = categories;
            Operations = operations;
            //date_s = _date_s;
            //date_e = _date_e;
        }
    }

    public class BalanceAnalytics : IAnalyticsVisitor
    {
        public decimal totalIncome { get; private set; }
        public decimal totalExpenses { get; private set; }
        public decimal balanceDifference { get; private set; }

        public void Visit(FinancialData data)
        {
            //var periodOperations = data.Operations.Where(o => o.date >= data.date_s && o.date <= data.date_e);
            totalIncome = data.Operations.Where(o => o.type == true).Sum(o => o.amount);
            totalExpenses = data.Operations.Where(o => o.type == false).Sum(o => o.amount);
            balanceDifference = totalIncome - totalExpenses;
        }
    }

    public class CategoryAnalytics : IAnalyticsVisitor
    {
        public Dictionary<string, decimal> incomeByCategory { get; private set; } = new();
        public Dictionary<string, decimal> expensesByCategory { get; private set; } = new();

        public void Visit(FinancialData data)
        {
            //var periodOperations = data.Operations.Where(o => o.date >= data.date_s && o.date <= data.date_e);
            incomeByCategory = data.Operations.Where(o => o.type == true).GroupBy(o => data.Categories.First(c => c.id == o.category_id).name)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.amount));
            expensesByCategory = data.Operations.Where(o => o.type == false).GroupBy(o => data.Categories.First(c => c.id == o.category_id).name)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.amount));
        }
    }
}
