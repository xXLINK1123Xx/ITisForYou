using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Learning_Platform.Data;

namespace Learning_Platform.Controllers
{
    [RoutePrefix("api/lessons")]
    public class LessonController : ApiController
    {
        private readonly LPDataContext context;
        private readonly IMapper mapper;

        public LessonController(LPDataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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

            var lesson = mapper.Map<Lesson, LessonDto>(result);
            return Ok(lesson);
        }

        [Route("{id}/tasks")]
        public async Task<IHttpActionResult> GetLessonTasks(int id)
        {
            var result = await context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            var tasks = result.Questions.ToList();
            var mappedTasks = mapper.Map<List<Question>, List<QuestionDto>>(tasks);
            return Ok(mappedTasks);
        }

        public class LessonDto
        {
            public int Id { get; set; }
            
            public string Title { get; set; }

            public string Description { get; set; }

            public List<QuestionDto> Questions { get; set; }
        }

        public class QuestionDto
        {
            public int Id { get; set; }

            public int LessonId { get; set; }

            public string Title { get; set; }

            public int? CorrectAnswerId { get; set; }

            public AnswerDto[] Answers { get; set; }
        }

        public class AnswerDto
        {

            public int Id { get; set; }

            public int QuestionId { get; set; }

            public string Text { get; set; }
        }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Lesson, LessonDto>()
                    .ForMember(r=> r.Questions, opt=> opt.MapFrom(s=> s.Questions.ToList()));

                CreateMap<Answer, AnswerDto>();

                CreateMap<Question, QuestionDto>()
                    .ForMember(r => r.Answers, opt => opt.MapFrom(s => s.Answers.ToArray()));
            }
        }

    }
}
