using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KPO_app2
{
    public class BankAccount
    {
        public Guid id { get; } // айди счета
        public string name { get; private set; } // название счета
        public decimal balance { get; private set; } // текущий баланс
        public BankAccount(Guid _id, string _name, decimal _balance) 
        {
            id = _id;
            name = _name;
            balance = _balance;
        }

        public void ChangeBalance(decimal amount)
        {
            balance += amount;
        }
        public void ChangeName(string _name)
        {
            name = _name;
        }
    }
}
