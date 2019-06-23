using System.Linq;
using AutoMapper;
using Learning_Platform.Controllers;
using Learning_Platform.Data;

namespace Learning_Platform.Mapping
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {

            CreateMap<Lesson, LessonController.LessonDto>()
                .ForMember(r => r.Questions, opt => opt.MapFrom(s => s.Questions.ToList()));

            CreateMap<Answer, LessonController.AnswerDto>();

            CreateMap<Question, LessonController.QuestionDto>()
                .ForMember(r => r.Answers, opt => opt.MapFrom(s => s.Answers.ToArray()));


            CreateMap<Course, CourseController.CourseDto>();
        }
    }
}