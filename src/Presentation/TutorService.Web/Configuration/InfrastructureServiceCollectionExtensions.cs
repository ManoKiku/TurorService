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
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(ICrudRepository<>), typeof(CrudRepository<>));
        
        services.AddScoped<DbInitializer>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITutorProfileRepository, TutorProfileRepository>();
        services.AddScoped<ITutorPostRepository, TutorPostRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IStudentTutorRelationRepository, StudentTutorRelationRepository>();
        
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        
        
        return services;
    }
}