using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootArt.Infrastructure.Entities
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }= default!;

        public string Surname { get; set; } = default!;

        public int Age { get; set; }

        public string Gender { get; set; } = default!;

        public DateTime Optime { get; set; } = DateTime.Now;
    }
}
