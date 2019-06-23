using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Learning_Platform.Data;
using Microsoft.AspNet.Identity;

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

            var course = mapper.Map<Course, CourseDto>(result);

            return Ok(course);
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

            var lessons = mapper.Map<List<LessonController.LessonDto>>(result.Lessons.ToList());

            return Ok(lessons);
        }

        [Authorize]
        [HttpGet]
        [Route("{id}/enroll")]
        public async Task<IHttpActionResult> EnrollUserToCourse(int id)
        {
            var result = await context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (result == null)
            {
                return InternalServerError(new Exception("no course with such id found!"));
            }

            var userCourse = new UserCourse
            {
                CourseId = id,
                UserId = User.Identity.GetUserId()
            };

            result.UserCourses.Add(userCourse);

            await context.SaveChangesAsync();

            return Ok();
        }

        public class CourseDto
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string Image { get; set; }
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
