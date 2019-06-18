using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using Learning_Platform.Data;

namespace Learning_Platform.Controllers
{
    [RoutePrefix("api/lessons")]
    public class LessonController : ApiController
    {
        private readonly LPDataContext context;

        public LessonController(LPDataContext context)
        {
            this.context = context;
        }

        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> GetLesson(int id)
        {
            var result = await context.Lessons.FirstOrDefaultAsync(l=> l.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route("{id}/tasks")]
        public async Task<IHttpActionResult> GetLessonTasks(int id)
        {
            var result = await context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}
