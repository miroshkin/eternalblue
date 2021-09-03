using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EternalBlue.Data;

namespace EternalBlue.Ifs
{
    public interface IIfsDataProvider
    {
        public ICollection<T> GetItems<T>(string resource, CancellationToken ct);
    }
}
