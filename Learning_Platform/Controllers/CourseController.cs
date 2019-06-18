using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Learning_Platform.Data;

namespace Learning_Platform.Controllers
{

    [RoutePrefix("api/courses")]
    public class CourseController : ApiController
    {

        private readonly LPDataContext context;

        public CourseController(LPDataContext context)
        {
            this.context = context;
        }

        public async Task<IHttpActionResult> GetAll()
        {
            var result = await context.Courses.ToListAsync();

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var result = await context.Courses.FirstOrDefaultAsync(c=> c.Id == id);

            if (result == null)
            {
                return InternalServerError(new Exception("no course with such id found!"));
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}/lessons")]
        public async Task<IHttpActionResult> GetCourseLessons(int id)
        {
            var result = await context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (result == null)
            {
                return InternalServerError(new Exception("no course with such id found!"));
            }

            return Ok(result.Lessons.ToList());
        }
    }
}
