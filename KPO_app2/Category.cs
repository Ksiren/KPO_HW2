using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPO_app2
{
    public class Category
    {

        public Guid id { get; } // айди катег
        public bool type { get; } // доход/расход (1/0)
        public string name { get; } // название кат
        public Category(Guid _id, bool _type, string _name)
        {
            id = _id;
            type = _type;
            name = _name;
        }
    }
}
