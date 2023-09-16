using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Configuring.Startup;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;

namespace Task_Manager_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Tasks_Controller : Controller
    {
        private readonly TaskManagerDbContext taskManagerDbContext;

        public Tasks_Controller(TaskManagerDbContext taskManagerDbContext)
        {
                this.taskManagerDbContext = taskManagerDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            // Get the tasks from the database
            return Ok(await taskManagerDbContext.task.ToListAsync());
        }

        [HttpGet]
        [Route("{id:int}")]
        [ActionName("GetTaskById")]
        public async Task<IActionResult> GetTaskById([FromRoute]int id)
        {
            var task = await taskManagerDbContext.task.FindAsync(id);
            //or
            // await taskManagerDbContext.task.FirstOrDefaultAsync(x => x.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(Tasks tasks)
        {
            await taskManagerDbContext.task.AddAsync(tasks);
            await taskManagerDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskById), new { task = tasks.TaskId }, tasks);

        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateNote([FromRoute] int id, [FromBody] Tasks updatedTask)
        {
            var existingTask = await taskManagerDbContext.task.FindAsync(id);

            if (UpdateNote == null)
            {
                return NotFound();
            }

            existingTask.TaskName = updatedTask.TaskName;
            existingTask.TaskDes = updatedTask.TaskDes;
            existingTask.DueDate = updatedTask.DueDate;
            existingTask.Status = updatedTask.Status;

            await taskManagerDbContext.SaveChangesAsync();

            return Ok(existingTask);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id)
        {
            var existingTask = await taskManagerDbContext.task.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            taskManagerDbContext.task.Remove(existingTask);
            await taskManagerDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
