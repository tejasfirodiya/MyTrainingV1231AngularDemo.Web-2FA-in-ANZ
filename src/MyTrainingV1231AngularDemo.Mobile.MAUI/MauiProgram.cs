﻿using Microsoft.Extensions.Configuration;
using System.Reflection;
using MyTrainingV1231AngularDemo.Core;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI
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
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            ApplicationBootstrapper.InitializeIfNeeds<MyTrainingV1231AngularDemoMobileMAUIModule>();

            var app = builder.Build();
            return app;
        }
    }
}