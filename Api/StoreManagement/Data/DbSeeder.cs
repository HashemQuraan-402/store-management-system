using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Models.Entities;

namespace StoreManagement.Data
{
    public static class DbSeeder
    {
        public static void Seed(StoreDbContext dbContext, string seedUsersPassword)
        {
            dbContext.Database.Migrate();

            if (dbContext.Users.Any())
            {
                return;
            }

            var hasher = new PasswordHasher<User>();

            var users = new List<User>
            {
                new()
                {
                    UserFullName = "Ahmad Al-Manager",
                    UserName = "manager1",
                    UserType = UserType.Manager
                },
                new()
                {
                    UserFullName = "sara Al-Manager",
                    UserName = "manager2",
                    UserType = UserType.Manager
                },
                new()
                {
                    UserFullName = "Omar Employee",
                    UserName = "employee1",
                    UserType = UserType.Employee
                },
                new()
                {
                    UserFullName = "Lina Employee",
                    UserName = "employee2",
                    UserType = UserType.Employee
                },
                new()
                {
                    UserFullName = "Yousef Employee",
                    UserName = "employee3",
                    UserType = UserType.Employee
                }
            };

            foreach (var user in users)
            {
                user.Password = hasher.HashPassword(user, seedUsersPassword);
            }
            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }
    }
}
