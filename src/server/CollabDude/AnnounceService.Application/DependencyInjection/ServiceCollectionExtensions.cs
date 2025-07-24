using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AnnounceService.Application.Interfaces;
using AnnounceService.Application.Services;
using AnnounceService.Application.Mappings;
using AnnounceService.Application.Validators;

namespace AnnounceService.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // AutoMapper
        services.AddAutoMapper(
            typeof(AnnounceMappingProfile),
            typeof(ApplicationMappingProfile),
            typeof(CommentMappingProfile),
            typeof(CategoryMappingProfile),
            typeof(TagMappingProfile),
            typeof(NotificationMappingProfile)
        );

        // Application Services
        services.AddScoped<IAnnounceService, Services.AnnounceService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<INotificationService, NotificationService>();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateAnnounceRequestValidator>();

        return services;
    }
}