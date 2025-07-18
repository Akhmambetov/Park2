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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Presentation.ConsoleUI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimulationServices(this IServiceCollection services)
        {
            services.AddSingleton<Park>();
            services.AddSingleton<IClockSimulationService, ClockSimulationService>();
            services.AddSingleton<IAttractionService, AttractionService>();
            services.AddSingleton<IVisitorService, VisitorService>();
            services.AddSingleton<IRandomAttractionService, RandomAttractionService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddTransient<AttractionProcessorService>();
            services.AddSingleton<SimulationEngine>();
            services.AddSingleton<ReportController>();
            return services;
        }
    }
}
