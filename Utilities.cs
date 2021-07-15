using System;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace TodoApi {

    
    public class Utilities{

        private static CloudTable AuthTable()
        {
            try
            {
                string connectionString = "Nope :)";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference("ToDoTable");

                return table;
            }
            catch
            {
                return null;
            }
        }
        public static CloudTable getAuthTable(){
            return AuthTable();
        }

        public static async Task<TodoItem> RetriveEntry(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<TodoItem>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                TodoItem item = result.Result as TodoItem;
                return item;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
        public static async Task<TodoItem> CreateEntity(CloudTable table, TodoItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                TodoItem insertedItem = result.Result as TodoItem;

                return insertedItem;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
        public static async Task DeleteEntity(CloudTable table, TodoItem deleteEntity)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = await table.ExecuteAsync(deleteOperation);

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

    }
}