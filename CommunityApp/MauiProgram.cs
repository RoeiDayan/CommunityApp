using CommunityApp.Services;
using CommunityApp.ViewModels;
using CommunityApp.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace CommunityApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Kodchasan-SemiBold.ttf", "Font1");
                })
                .RegisterDataServices()
                .RegisterPages()
                .RegisterViewModels()
                .UseMauiCommunityToolkit()
                ;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        public static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<RegisterView>();
            builder.Services.AddTransient<SelectCommunityView>();
            builder.Services.AddTransient<HomePageView>();
            builder.Services.AddTransient<ProfilePageView>();
            builder.Services.AddTransient<StartCommunityView>();
            builder.Services.AddTransient<JoinCommunityView>();
            builder.Services.AddTransient<AccountPageView>();
            builder.Services.AddTransient<CreateReportView>();
            builder.Services.AddTransient<MemberListView>();
            builder.Services.AddTransient<CreateNoticeView>();

            return builder;
        }

        public static MauiAppBuilder RegisterDataServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<CommunityWebAPIProxy>();
            return builder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<SelectCommunityViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddTransient<StartCommunityViewModel>();
            builder.Services.AddTransient<JoinCommunityViewModel>();
            builder.Services.AddTransient<AccountPageViewModel>();
            builder.Services.AddTransient<CreateReportViewModel>();
            builder.Services.AddTransient<MemberListViewModel>();
            builder.Services.AddTransient<CreateNoticeViewModel>();



            return builder;
        }
    }
}
