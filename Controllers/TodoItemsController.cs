using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi;
using Microsoft.Azure.Cosmos.Table;

namespace TodoApi.Controllers
{
    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {

        public TodoItemsController()
        {

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            TableQuery<TodoItem> query = new TableQuery<TodoItem>();
            var segment = await Utilities.getAuthTable().ExecuteQuerySegmentedAsync(query, null);
            return segment;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            TodoItem todoItem = await Utilities.RetriveEntry(Utilities.getAuthTable(), "Test", id.ToString());

            return ItemToDTO(todoItem);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            Random random = new Random();
            int newId = random.Next(10000);
            while(true){
                try {
                    await GetTodoItem(newId);
                    newId++;
                } catch (Exception e) {
                    break;
                }
            }
            
            var todoItem = new TodoItem{
                PartitionKey = "Test",
                RowKey = Convert.ToString(newId),
                Name = todoItemDTO.Name,
                IdNum = newId
                };

            await Utilities.CreateEntity(Utilities.getAuthTable(), todoItem);

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = newId },
                todoItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var deleteOperation = TableOperation.Delete(new TableEntity() { PartitionKey = "Test", RowKey = Convert.ToString(id), ETag = "*" });
            try {
            var deleteResult = await Utilities.getAuthTable().ExecuteAsync(deleteOperation);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
            {
                return new NotFoundResult();
            }
            return NoContent();
        }
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = Int32.Parse(todoItem.RowKey),
                Name = todoItem.Name,
            };       
    }
}
