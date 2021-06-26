﻿using Lab1_.NET.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1_.NET.Data
{
    public class SeedReservations
    {
        private static readonly Random random = new();

        public static void Seed(IServiceProvider serviceProvider, int count)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            var numberOfMovies = context.Movies.Count();
            var numberOfUsers = context.ApplicationUsers.Count();

            if (context.Reservations.Count() < 1200)
            {
                for (int i = 0; i < count; ++i)
                {
                    var movies = context.Movies.Skip(random.Next(1, numberOfMovies)).Take(2).ToList();
                    var user = context.ApplicationUsers.Skip(random.Next(1, numberOfUsers)).Take(1).First();

                    context.Reservations.Add(new Reservation
                    {
                        Movies = movies,
                        ReservationDateTime = GetRandomDate(),
                        ApplicationUser = user
                    });
                }

                context.SaveChanges();
            }
        }

        private static DateTime GetRandomDate()
        {
            int rangePastThreeYears = 3 * 365;
            DateTime randomDate = DateTime.Today.AddDays(-random.Next(rangePastThreeYears));

            return randomDate;
        }
    }
}
