using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Ifs
{
    public class ResourceNameAttribute : Attribute
    {
        public ResourceNameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get;}
    }
}
