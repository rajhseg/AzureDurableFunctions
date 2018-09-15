using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFunction
{
    public class OrchestratorFunction
    {
        [FunctionName("Orchestrator")]
        public static async Task<string> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var name = context.GetInput<string>();
            // retrieves the list of data by invoking a separate Activity Function.
            var books = await context.CallActivityAsync<List<Book>>("GetAllData", name);
            if (books.Count > 0)
            {
                //Saving the retrieved data to table
                await context.CallActivityAsync("SaveData", books);
            }
            return context.InstanceId;
        }
    }
}
