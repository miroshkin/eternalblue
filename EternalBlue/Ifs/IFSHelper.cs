using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EternalBlue.Ifs
{
    public class IFSHelper
    {
        const int FortranYearCreation = 1957;
        
        public static string GetResourceName(Type type)
        {
            var resourceName = (ResourceNameAttribute)Attribute.GetCustomAttribute(type, typeof(ResourceNameAttribute));

            if (resourceName == null)
            {
                throw new ArgumentException("Resource name attribute has not been found");
            }
            
            return resourceName.Name;
        }

        /// <summary>
        /// Gets the age of the oldest language that is still in use
        /// https://www.learnacademy.org/blog/first-programming-language-use-microsoft-apple/
        /// </summary>
        /// <returns>Fortran programming language age in years</returns>
        public static int GetFortranAge()
        {
            return DateTime.Now.Year - FortranYearCreation;
        }
    }
}
