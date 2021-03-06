﻿using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyBlog.Core.Interfaces;

namespace TinyBlog.Core.SharedKernel
{

    //
    // Reference: https://tahirnaushad.com/2017/08/29/azure-table-storage-in-asp-net-core-2-0/
    public class AzureTableStorage<T> : IAzureTableStorage<T> where T : BaseEntity, new()
    {
        public AzureTableStorage(AzureTableSettings settings)
        {
            this.settings = settings;
        }

        public async Task<List<T>> GetList()
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>();

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;
                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }

        public async Task<List<T>> GetList(string partitionKey)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Query
            TableQuery<T> query = new TableQuery<T>()
                                        .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                                                QueryComparisons.Equal, partitionKey));

            List<T> results = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> queryResults =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);

            } while (continuationToken != null);

            return results;
        }

        public async Task<T> GetItem(string partitionKey, string rowKey)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            //Execute
            TableResult result = await table.ExecuteAsync(operation);

            return (T)(dynamic)result.Result;
        }

        public async Task Insert(T item)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Insert(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Update(T item)
        {
            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.InsertOrReplace(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        public async Task Delete(string partitionKey, string rowKey)
        {
            //Item
            T item = await GetItem(partitionKey, rowKey);

            //Table
            CloudTable table = await GetTableAsync();

            //Operation
            TableOperation operation = TableOperation.Delete(item);

            //Execute
            await table.ExecuteAsync(operation);
        }

        private readonly AzureTableSettings settings;

        public async Task<CloudTable> GetTableAsync()
        {
            //Account
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new StorageCredentials(this.settings.StorageAccount, this.settings.StorageKey), false);

            //Client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            //Table
            CloudTable table = tableClient.GetTableReference(this.settings.TableName);
            await table.CreateIfNotExistsAsync();

            return table;
        }
    }
}
