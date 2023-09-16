using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;


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
            services.AddControllers();
            services.AddSwaggerGen();
        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task_Manager API V1");
                c.RoutePrefix = string.Empty;
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseRouting();
            app.UseHttpsRedirection();

            // Configure middleware for authentication, CORS, etc.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
                    .Property(t => t.TaskName)
                    .HasMaxLength(100);
                modelBuilder.Entity<Tasks>()
                    .HasOne(t => t.User) // Defines the realationship to Users
                    .WithMany(t => t.Tasks) // Defines the inverse navigaton property in Users
                    .HasForeignKey(u => u.UserId); // Specifies the foreign key

                modelBuilder.Entity<Users>()
                    .HasKey(u => u.UserId); // Define the primary key for Users
                modelBuilder.Entity<Users>()
                    .Property(u => u.Username)
                    .HasMaxLength(50);
                modelBuilder.Entity<Users>()
                    .Property(u => u.Email)
                    .HasMaxLength(254);
                modelBuilder.Entity<Users>()
                    .Property(u => u.Password)
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
            [EnumMember(Value = "Open")]
            Open,
            [EnumMember(Value = "In Progress")]
            In_Progress, // Match the PostgreSQL enum value 'In Progress' using underscores
            [EnumMember(Value = "Closed")]
            Closed
        }
    }
}
