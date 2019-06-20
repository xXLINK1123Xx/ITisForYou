using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Learning_Platform.Data;

namespace Learning_Platform.Controllers
{

    [RoutePrefix("api/courses")]
    public class CourseController : ApiController
    {

        private readonly LPDataContext context;
        private readonly IMapper mapper;

        public CourseController(LPDataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            var result = await context.Courses.ToListAsync();

            return Ok(mapper.Map<List<CourseDto>>(result.ToList()));
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

        public class CourseDto
        {
            public int Id { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Course, CourseDto>();
            }
        }
    }
}
