using Microsoft.Extensions.DependencyInjection;

namespace PotionPanic.Services
{
    public static class ServiceHelper
    {
        public static IServiceProvider Services { get; private set; } = default!;

        public static void Configure(IServiceProvider provider) => Services = provider;

        public static T Get<T>() where T : notnull => Services.GetRequiredService<T>();
    }
}
