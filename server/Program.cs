using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;  
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;     


//dotnet watch run
    namespace TodoApi{
        public class Program {   

        public static void Main(string[] args)
        {
             // הגדרת מפתח סודי (במציאות שומרים ב-appsettings.json)
            var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharsLong!");
            
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("TodoDB");
                                          
            builder.Services.AddDbContext<ToDoDbContext>(options =>       
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
           
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen();
       
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
               });
            });
       
            builder.Services.AddControllers();//

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            builder.Services.AddAuthorization();//

            var app = builder.Build();
       
       
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                    options.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }
       
            app.UseCors("AllowAllOrigins");//cors

            app.UseAuthentication(); // 1. בדיקה מי המשתמש
            app.UseAuthorization();  // 2. בדיקה למה מותר לו לגשת

            app.MapControllers();

            // הרשמה
            app.MapPost("/register", async (ToDoDbContext db, User user) => {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // התחברות והנפקת טוקן
            app.MapPost("/login", async (ToDoDbContext db, User loginUser) => {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);
                if (user == null) return Results.Unauthorized();

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), new Claim(ClaimTypes.Name, user.Username) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Results.Ok(new { token = tokenHandler.WriteToken(token) });
            });    


            app.MapGet("/tasks", async ([FromServices]ToDoDbContext db) => await db.Items.ToListAsync()).RequireAuthorization();

           app.MapPost("/tasks", async ([FromServices]ToDoDbContext db, [FromBody]Item item) =>
            {
                 db.Items.Add(item);
                await db.SaveChangesAsync();
                return Results.Created($"/tasks/{item.Id}", item);//201 (Created).
            }).RequireAuthorization();
            

             app.MapPut("/tasks/{id}", async ([FromServices]ToDoDbContext db, int id,[FromBody] Item updateItem) =>
            {
            
                var item = await db.Items.FindAsync(id);
                if (item is null) return Results.NotFound();

                item.Name = updateItem.Name;
                item.IsComplete = updateItem.IsComplete;

                await db.SaveChangesAsync();
                return Results.NoContent();// 204 (No Content) כדי להצביע על כך שהבקשה הושלמה בהצלחה, אך אין תוכן להחזיר
            }).RequireAuthorization();

            app.MapDelete("/tasks/{id}", async ([FromServices]ToDoDbContext db, int id) =>
            {
                var item = await db.Items.FindAsync(id);
                if (item is null) return Results.NotFound();

                db.Items.Remove(item);
                await db.SaveChangesAsync();
                return Results.NoContent();// 204 (No Content) כדי להצביע על כך שהבקשה הושלמה בהצלחה, אך אין תוכן להחזיר
            }).RequireAuthorization();



            app.Run();
        }}
    }
// }