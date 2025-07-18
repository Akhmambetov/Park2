using Microsoft.Extensions.DependencyInjection;
using Park2.Application.Interfaces;
using Park2.Application.Interfaces.AttractionInterface;
using Park2.Application.Interfaces.ReportInterfaces;
using Park2.Application.Interfaces.VisitorInterface;
using Park2.Application.Services;
using Park2.Application.Services.AttrationService;
using Park2.Application.Services.ReportServices;
using Park2.Application.Services.VisitorService;
using Park2.Application.Simulation;
using Park2.Domain.Models;
using Park2.Presentation.ConsoleUI;
using Park2.Presentation.ConsoleUI.Menu;
using Park2.Presentation.ConsoleUI.Report;
using Park2.Presentation.ConsoleUI.VisitorPrinter;
using Park2.Presentation.ConsoleUI.AttractionPrinter;
using System;

namespace Park2.Presentation.ConsoleUI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimulationServices(this IServiceCollection services)
        {
            // Core simulation services
            services.AddSingleton<Park>();
            services.AddSingleton<IClockSimulationService, ClockSimulationService>();
            services.AddSingleton<IAttractionService, AttractionService>();
            services.AddSingleton<IVisitorService, VisitorService>();
            services.AddSingleton<IRandomAttractionService, RandomAttractionService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddTransient<AttractionProcessorService>();
            services.AddSingleton<SimulationEngine>();
            services.AddSingleton<IReportController, ReportController>();

            // Console UI services
            services.AddSingleton<ConsoleApp>();
            services.AddSingleton<IMenuPrinter, MenuPrinter>();
            services.AddSingleton<IReportMenuPrinter, ReportMenuPrinter>();
            services.AddSingleton<IVisitorStatsPrinter, VisitorStatsPrinter>();
            services.AddSingleton<IAttractionStatusPrinter, AttractionStatusPrinter>();
            services.AddSingleton<IConsoleChangeAttractionStatus, ConsoleChangeAttractionStatus>();
            services.AddSingleton<IVisitorCreator, ConsoleVisitorCreator>();
            services.AddSingleton<IInputReader, ConsoleInputReader>();
            services.AddSingleton<IOutputWriter, ConsoleOutputWriter>();
            services.AddSingleton<IReportPrinter, ReportPrinter>();

            return services;
        }
    }
}
