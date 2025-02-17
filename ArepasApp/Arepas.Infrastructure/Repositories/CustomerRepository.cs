﻿
using Arepas.Domain.Interfaces.Repositories;
using Arepas.Infrastructure.Common;
using Arepas.Infrastructure.Context;
using Arepas.Domain.Entities.Models;
using Arepas.Domain.Entities.Dto;
using Microsoft.EntityFrameworkCore;

namespace Arepas.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customers>, ICustomerRepository
    {
        private readonly AppDbContext _appDbContext;

        public CustomerRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<PaginationResult<Customers>> GetByPageAsync(PaginationParams @params)
        {
            var customers = _appDbContext.Customers.OrderBy(x => x.Id);

            var xTotalCount = customers.Count();
            
            var items = await customers.OrderBy(p => p.Id)
                .Skip((@params.Page - 1) * @params.Limit)
                .Take(@params.Limit)
                .ToListAsync<Customers>();

            return new PaginationResult<Customers>()
            {
                XTotalCount = xTotalCount,
                Links = @"<links>",
                Item = items
            };
        }

        public async Task<IEnumerable<Customers>> SearchAsync(string queryValue)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var customers = await _appDbContext.Customers
                .Where(x => x.FirstName.Contains(queryValue)
                || x.LastName.Contains(queryValue)
                || x.Email.Contains(queryValue)
                || x.Address.Contains(queryValue)
                || x.PhoneNumber.Contains(queryValue))
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return customers;
        }
    }
}
