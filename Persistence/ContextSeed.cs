using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains;
using Domains.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Persistence
{
    public class ContextSeed
    {
        public static async Task SeedAsync(DataContext context, ILoggerFactory loggerFactory, UserManager<AppUser> userManager)
        {
            try
            {
                if (!userManager.Users.Any() || !context.Activities.Any())
                {
                    var users = new List<AppUser>
                    {
                        new AppUser()
                        {
                            DisplayName = "Kholfa",
                            UserName = "khalifa",
                            Email = "khalifa@test.com"
                        },
                        new AppUser()
                        {
                            DisplayName = "Osos",
                            UserName = "osama",
                            Email = "osos@test.com"
                        },
                        new AppUser
                    {
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Jane",
                        UserName = "jane",
                        Email = "jane@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Tom",
                        UserName = "tom",
                        Email = "tom@test.com"
                    },
                    };


                    foreach (var user in users)
                    {
                        await userManager.CreateAsync(user, "Pa$$w0rd");
                    }

                    var activities = new List<Activity>
                    {
                      new Activity
                    {
                        Title = "Past Activity 1",
                        Date = DateTime.Now.AddMonths(-2),
                        Description = "Activity 2 months ago",
                        Category = "drinks",
                        City = "London",
                        Venue = "Pub",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[0],
                                IsHost = true
                            }
                        }
                    },
                    new Activity
                    {
                        Title = "Past Activity 2",
                        Date = DateTime.Now.AddMonths(-1),
                        Description = "Activity 1 month ago",
                        Category = "culture",
                        City = "Paris",
                        Venue = "The Louvre",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[0],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 1",
                        Date = DateTime.Now.AddMonths(1),
                        Description = "Activity 1 month in future",
                        Category = "music",
                        City = "London",
                        Venue = "Wembly Stadium",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[2],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 2",
                        Date = DateTime.Now.AddMonths(2),
                        Description = "Activity 2 months in future",
                        Category = "food",
                        City = "London",
                        Venue = "Jamies Italian",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[0],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[2],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 3",
                        Date = DateTime.Now.AddMonths(3),
                        Description = "Activity 3 months in future",
                        Category = "drinks",
                        City = "London",
                        Venue = "Pub",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[0],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 4",
                        Date = DateTime.Now.AddMonths(4),
                        Description = "Activity 4 months in future",
                        Category = "culture",
                        City = "London",
                        Venue = "British Museum",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = true
                            }
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 5",
                        Date = DateTime.Now.AddMonths(5),
                        Description = "Activity 5 months in future",
                        Category = "drinks",
                        City = "London",
                        Venue = "Punch and Judy",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[0],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 6",
                        Date = DateTime.Now.AddMonths(6),
                        Description = "Activity 6 months in future",
                        Category = "music",
                        City = "London",
                        Venue = "O2 Arena",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[2],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 7",
                        Date = DateTime.Now.AddMonths(7),
                        Description = "Activity 7 months in future",
                        Category = "travel",
                        City = "Berlin",
                        Venue = "All",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[0],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[2],
                                IsHost = false
                            },
                        }
                    },
                    new Activity
                    {
                        Title = "Future Activity 8",
                        Date = DateTime.Now.AddMonths(8),
                        Description = "Activity 8 months in future",
                        Category = "drinks",
                        City = "London",
                        Venue = "Pub",
                        UserActivities = new List<UserActivity>
                        {
                            new UserActivity
                            {
                                AppUser = users[2],
                                IsHost = true
                            },
                            new UserActivity
                            {
                                AppUser = users[1],
                                IsHost = false
                            },
                        }
                    }
                    };
                    context.Activities.AddRange(activities);
                    context.SaveChanges();
                }



            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ContextSeed>();
                logger.LogError(ex.Message);
            }
        }

    }
}
