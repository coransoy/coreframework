using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adriva.Common.Core;

namespace Adriva.Storage.Abstractions
{
    public interface ITableClient : IStorageClient
    {
        Task<TItem> GetAsync<TItem>(string partitionKey, string rowKey) where TItem : class, ITableItem, new();

        Task<SegmentedResult<TItem>> GetAllAsync<TItem>(string continuationToken = null, string partitionKey = null, string rowKey = null, int rowCount = 500) where TItem : class, ITableItem, new();

        Task<SegmentedResult<TItem>> SelectAsync<TItem>(Expression<Func<TItem, bool>> queryExpression, string continuationToken = null, int rowCount = 500) where TItem : class, ITableItem, new();

        Task UpsertAsync<TItem>(TItem item) where TItem : class, ITableItem;

        Task DeleteAsync(string partitionKey, string rowKey);
    }
}