using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeeves.Models
{
    public class Connection
    {
        public Connection(string name, string pipe)
        {
            Name = name;
            Pipe = pipe;
        }

        public string Name { get; set; }
        public string Pipe { get; set; }
    }
}
