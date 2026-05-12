using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TutorService.Application.Interfaces;
using TutorService.Application.Mappers;
using TutorService.Application.Services;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Repositories;

namespace TutorService.Web.Configuration;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>()
            .AddScoped<IJwtService, JwtService>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ITutorProfileService, TutorProfileService>()
            .AddScoped<ITutorPostService, TutorPostService>()
            .AddScoped<ISubjectService, SubjectService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<ISubCategoryService, SubCategoryService>()
            .AddScoped<ITagService, TagService>()
            .AddScoped<ICityService, CityService>()
            .AddScoped<ILessonRepository, LessonRepository>()
            .AddScoped<ILessonService, LessonService>()
            .AddScoped<IStudentTutorRelationService, StudentTutorRelationService>()
            .AddScoped<IFileRepository, MongoFileRepository>()
            .AddScoped<IAssignmentRepository, AssignmentRepository>()
            .AddScoped<IAssignmentService, AssignmentService>()
            .AddScoped<IChatRepository, ChatRepository>()
            .AddScoped<IChatService, ChatService>()
            .AddScoped<IMessageRepository, MessageRepository>()
            .AddScoped<IMessageService, MessageService>()
            .AddScoped<IMessageService, MessageService>()
            .AddScoped<ISavedContentService, SavedContentService>()
            .AddScoped<IReviewService, ReviewService>()
            .AddScoped<ILessonTaskService, LessonTaskService>()
            .AddScoped<ILessonCommentService, LessonCommentService>();
        
        services.AddSignalR().AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        
        
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<TutorMappingProfile>();
            cfg.AddProfile<SubjectMappingProfile>();
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<SubcategoryMappingProfile>();
            cfg.AddProfile<TagMappingProfile>();
            cfg.AddProfile<CityMappingProfile>();
            cfg.AddProfile<LessonMappingProfile>();
            cfg.AddProfile<StudentTutorRelationMappingProfile>();
            cfg.AddProfile<AssignmentMappingProfile>();
            cfg.AddProfile<ChatMappingProfile>();
            cfg.AddProfile<SavedContentMappingProfile>();
            cfg.AddProfile<ReviewMappingProfile>();
            cfg.AddProfile<LessonTaskMappingProfile>();
            cfg.AddProfile<LessonCommentMappingProfile>();
            cfg.AddProfile<SavedContentMappingProfile>();
        });

        return services;
    }
}