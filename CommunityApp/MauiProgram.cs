﻿using CommunityApp.Services;
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
            builder.Services.AddTransient<TenantRoomView>();
            builder.Services.AddTransient<ManageJoinRequestsView>();
            builder.Services.AddTransient<ManageMembersView>();
            builder.Services.AddTransient<RoomRequestView>();
            builder.Services.AddTransient<ManageRoomRequestsView>();
            builder.Services.AddTransient<CreateExpenseView>();
            builder.Services.AddTransient<CommunityExpensesView>();
            builder.Services.AddTransient<IssuePaymentsView>();
            builder.Services.AddTransient<PaymentHistoryView>();
            builder.Services.AddTransient<ManagePaymentsView>();
            builder.Services.AddTransient<ManageNoticesView>();
            builder.Services.AddTransient<ManageReportsView>();

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
            builder.Services.AddTransient<TenantRoomViewModel>();
            builder.Services.AddTransient<ManageJoinRequestsViewModel>();
            builder.Services.AddTransient<ManageMembersViewModel>();
            builder.Services.AddTransient<RoomRequestViewModel>();
            builder.Services.AddTransient<ManageRoomRequestsViewModel>();
            builder.Services.AddTransient<CreateExpenseViewModel>();
            builder.Services.AddTransient<CommunityExpensesViewModel>();
            builder.Services.AddTransient<IssuePaymentsViewModel>();
            builder.Services.AddTransient<PaymentHistoryViewModel>();
            builder.Services.AddTransient<ManagePaymentsViewModel>();
            builder.Services.AddTransient<ManageNoticesViewModel>();
            builder.Services.AddTransient<ManageReportsViewModel>();


            return builder;
        }
    }
}
