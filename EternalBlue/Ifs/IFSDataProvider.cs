using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using EternalBlue.Data;
using EternalBlue.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EternalBlue.Ifs
{
    public class IfsDataProvider : IIfsDataProvider 
    {
        private readonly IFSContext _context;
        private readonly HttpClient _httpClient;

        public IfsDataProvider(IFSContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<ICollection<T>> GetItems<T>(string resource, CancellationToken ct)
        {
            return await _httpClient.GetFromJsonAsync<ICollection<T>>(resource, ct);
        }
    }
}
