using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Learning_Platform.Data;
using Microsoft.AspNet.Identity;
using static Learning_Platform.Controllers.CourseController;

namespace Learning_Platform.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly LPDataContext context;
        private readonly IMapper mapper;

        public UserController(LPDataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetUserProfile()
        {
            var userId = User.Identity.GetUserId();
            var dbprofile = await context.UserProfiles.FirstOrDefaultAsync(p=> p.UserId == userId);

            if (dbprofile == null)
            {
                dbprofile = new UserProfile {UserId = userId};
                context.UserProfiles.Add(dbprofile);
                await context.SaveChangesAsync();
            }

            var mappedProfile = mapper.Map<UserProfileDto>(dbprofile);
            return Ok(mappedProfile);
        }

        [HttpGet]
        [Route("courses")]
        public async Task<IHttpActionResult> GetUserCourses()
        {

            var userId = User.Identity.GetUserId();
            var dbCourses =
                (await context.AspNetUsers.FirstOrDefaultAsync(u => u.Id == userId))?.UserCourses
                .Select(c => c.Course).ToList();

            if (dbCourses == null)
            {
                return Ok(new List<CourseDto>());
            }

            var courses = mapper.Map<List<CourseDto>>(dbCourses);

            return Ok(courses);
        }

        [HttpGet]
        [Route("courses/{courseId}")]
        public async Task<IHttpActionResult> GetUserProgress(int courseId)
        {
            var userId = User.Identity.GetUserId();
            var progress =
                await context.UserProgresses.Where(u => u.UserId == userId && u.CourseId == courseId).Select(p => p.LessonId).ToListAsync();

            return Ok(progress);
        }


        [HttpPost]
        [Route("courses/{courseId}")]
        public async Task<IHttpActionResult> UpdateUserProgress(int courseId, int lessonId)
        {
            var progress = new UserProgress
            {
                CourseId = courseId,
                LessonId = lessonId,
                UserId = User.Identity.GetUserId()
            };

            context.UserProgresses.Add(progress);

            await context.SaveChangesAsync();

            return Ok(progress);
        }

        public class UserProfileDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int? Age { get; set; }
            //public EnGender Gender { get; set; }
        }

        public enum EnGender
        {
            Male = 0,
            Female = 1,
            NotAsigned = 3
        }
    }
}
