using Microsoft.EntityFrameworkCore;
using AccountingApi.Models;

namespace AccountingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 註冊服務：注意括號內的 AppDbContext 必須與你建立的類別名稱完全一致
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVue",
                    policy => policy.WithOrigins("http://localhost:5173") // Vue 預設網址
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowVue");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
