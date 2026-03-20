using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Data;

namespace WebLibrary
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Books}/{action=Index}/{id?}");
            app.MapRazorPages()
               .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Staff", "Member" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var staffEmail = "admin@library.com";
                var staffUser = await userManager.FindByEmailAsync(staffEmail);
                if (staffUser == null)
                {
                    staffUser = new IdentityUser { UserName = staffEmail, Email = staffEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(staffUser, "Admin@1234");
                    await userManager.AddToRoleAsync(staffUser, "Staff");

                    var adminMember = new WebLibrary.Models.Member
                    {
                        FirstName = "Hristo",
                        LastName = "Mitev",
                        Email = staffEmail,
                        Phone = "",
                        MembershipType = "Staff",
                        BooksRead = 0,
                        JoinDate = DateTime.Now
                    };
                    dbContext.Members.Add(adminMember);
                    await dbContext.SaveChangesAsync();
                }
            }

            app.Run();
        }
    }
}
