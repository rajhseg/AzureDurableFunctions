using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BookFunction
{
    public static class ActivityFunction
    {
        private static CloudStorageAccount account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));

        [FunctionName("GetAllData")]
        public static async Task<List<Book>> GetAllData([ActivityTrigger]DurableActivityContext context)
        {
            // retrieves the organization name from the Orchestrator function
            var organizationName = context.GetInput<string>();

            return new List<Book> { new Book{ Id = 1, Name = "C#" }, new Book{ Id = 2, Name = "Java" } };
        }

        [FunctionName("SaveData")]
        public static async Task SaveData([ActivityTrigger]DurableActivityContext context)
        {
            // retrieves a list of books from the Orchestrator function
            var books = context.GetInput<List<Book>>();

            // create a table storage client
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Books");

            await table.CreateIfNotExistsAsync();

            TableBatchOperation tableBatchOperations = new TableBatchOperation();

            for(int i=0; i<books.Count; i++)
            {
                tableBatchOperations.Add(TableOperation.InsertOrMerge(
                    new BookRepository(books[i].Id)
                    {
                        Name = books[i].Name
                    }));
            }

            await table.ExecuteBatchAsync(tableBatchOperations);

        }
    }

    public class BookRepository: TableEntity
    {
        public BookRepository(int id)
        {
            PartitionKey = "TechnicalBooks";
            RowKey = id.ToString();
        }

        public string Name { set; get; }
    }

    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
