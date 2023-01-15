﻿using Microsoft.Extensions.DependencyInjection;

namespace ParkingManagement.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {            
            services.AddSingleton<Parking.Interfaces.IParkingService, Parking.ParkingService>();

            return services;
        }
    }
}