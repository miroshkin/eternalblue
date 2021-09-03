using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Ifs
{
    public class IFSHelper
    {
        public static string GetResourceName(Type type)
        {
            var resourceName = (ResourceNameAttribute)Attribute.GetCustomAttribute(type, typeof(ResourceNameAttribute));

            if (resourceName == null)
            {
                throw new ArgumentException("Resource name attribute has not been found");
            }
            
            return resourceName.Name;
        }
    }
}
