using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using TutorService.Application.Configuration;
using TutorService.Application.Interfaces;
using TutorService.Domain.Interfaces;
using TutorService.Infrastructure.Data;
using TutorService.Infrastructure.Repositories;

namespace TutorService.Web.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        
        var mongoConnectionString = configuration.GetConnectionString("MongoDB");
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase("TutorServiceDb");

        services.AddSingleton<IMongoDatabase>(mongoDatabase);
        services.AddScoped<IFileRepository, MongoFileRepository>();
        
        services.AddScoped<IUserRepository, UserRepository>()
            .AddScoped(typeof(IRepository<>), typeof(BaseRepository<>))
            .AddScoped(typeof(ICrudRepository<>), typeof(CrudRepository<>));

        services.AddScoped<DbInitializer>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<ITutorProfileRepository, TutorProfileRepository>()
            .AddScoped<ITutorPostRepository, TutorPostRepository>()
            .AddScoped<ISubjectRepository, SubjectRepository>()
            .AddScoped<ITagRepository, TagRepository>()
            .AddScoped<ILessonRepository, LessonRepository>()
            .AddScoped<IStudentTutorRelationRepository, StudentTutorRelationRepository>()
            .AddScoped<ISavedContentRepository, SavedContentRepository>();
        
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        
        
        return services;
    }
}