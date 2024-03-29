﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using the template for generating Repositories and a Unit of Work for EF Core model.
// Code is generated on: 03/04/2022 21:00:08
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
using DevelSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevelSystem.Context
{
    public partial class EntityFrameworkRepository<T> : IRepository<T> where T : class
    {
        private DbContext context;
        protected DbSet<T> objectSet;

        public EntityFrameworkRepository(DbContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
            this.objectSet = context.Set<T>();
        }

        public virtual async Task<IQueryable<T>> All()
        {
            var list = await objectSet.ToListAsync();
            return list.AsQueryable();
        }

        public IQueryable<T> GetBy(Expression<Func<T, bool>> expression)
        {
            return objectSet.Where(expression).AsNoTracking();
        }

        public virtual async Task Add(T entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await objectSet.AddAsync(entity);
        }
        public virtual void Update(T entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            objectSet.Update(entity);
        }

        public virtual async Task AddAll(IList<T> entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await objectSet.AddRangeAsync(entity);
        }

        public virtual void Remove(T entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            objectSet.Remove(enity);
        }

        public DbContext Context
        {
            get
            {
                return context;
            }
        }
    }
}
