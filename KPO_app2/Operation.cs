using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KPO_app2
{
    public class Operation
    {
        public Guid id {  get; }
        public bool type { get; }
        public Guid bank_account_id { get; }
        public decimal amount { get; }
        public DateTime date { get; }
        public string description { get; private set; }
        public Guid category_id { get; }

        public Operation(Guid _id, bool _type, Guid _bank_account_id, decimal _amount, DateTime _date, Guid _category_id, string _description)
        {
            id = _id;
            type = _type;
            bank_account_id = _bank_account_id;
            amount = _amount;
            date = _date;
            description = _description;
            category_id = _category_id;
        }
        public void ChangeDescription(string _description) => description = _description;
    }
}
