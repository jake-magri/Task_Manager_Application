using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;


namespace Configuring
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TaskManagerDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Add other services, authentication, etc.
        }

        public class TaskManagerDbContext : DbContext
        {
            public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options) 
            { 
            }

            public DbSet<Tasks> task { get; set; }
            public DbSet<Users> user_account { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder) // Fluent API in EF Core
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Tasks>()
                    .HasKey(t => t.TaskId); // Define the primary key for Tasks
                modelBuilder.Entity<Tasks>()
                    .Property(ti => ti.TaskName)
                    .HasMaxLength(100);
                modelBuilder.Entity<Tasks>()
                    .HasOne(us => us.User) // Defines the realationship to Users
                    .WithMany(ta => ta.Tasks) // Defines the inverse navigaton property in Users
                    .HasForeignKey(f => f.UserId); // Specifies the foreign key

                modelBuilder.Entity<Users>()
                    .HasKey(u => u.UserId); // Define the primary key for Users
                modelBuilder.Entity<Users>()
                    .Property(u => u.Username)
                    .HasMaxLength(50);
                modelBuilder.Entity<Users>()
                    .Property(e => e.Email)
                    .HasMaxLength(254);
                modelBuilder.Entity<Users>()
                    .Property(p => p.Password)
                    .HasMaxLength(60);
            }
        }

        public class Users
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string? Email { get; set; }
            public string Password { get; set; }

            // Navigation property for tasks associated with this user
            public ICollection<Tasks> Tasks { get; set; }
        }

        public class Tasks
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; }
            public string TaskDes { get; set; }
            public DateTime? DueDate { get; set; }
            public TaskStatus Status { get; set; }

            // Foreign key property
            public int UserId { get; set; }

            // Navigation property for the associated user
            public Users User { get; set; }
        }

        public enum TaskStatus
        {
            Open,
            In_Progress, // Match the PostgreSQL enum value 'In Progress' using underscores
            Closed
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add production error handling here.
            }

            app.UseRouting();

            // Configure middleware for authentication, CORS, etc.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
