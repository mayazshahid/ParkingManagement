using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicaiton(this IServiceCollection services)
        {
            services.AddSingleton<DataStore>();
            services.AddSingleton<Parking.Interfaces.IParkingRepository, Parking.ParkingRepository>();

            return services;
        }
    }
}
