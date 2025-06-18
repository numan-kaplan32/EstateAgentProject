using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.KervanDbModelFolder;
using Kervan.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc();

builder.Services.AddDbContext<KervanDbContext>(o =>
    {
    o.UseSqlServer("Server=NUMAN-KAPLAN\\SQLEXPRESS;Database=DbAgent;Integrated Security=True;TrustServerCertificate=True;");
});

builder.Services.AddScoped(typeof(ICoreService<>), typeof(Queries<>));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(c =>
   {
       c.LoginPath = "/Admin/Administrator/Login";
       c.LogoutPath = "/Admin/Administrator/Logout";
       c.AccessDeniedPath = "/Admin/Administrator/Login";
   }
    );
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "default",
    areaName: "Admin",
    pattern: "Admin/{Controller=Home}/{Action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{Controller=Home}/{Action=Index}/{id?}"
);

app.Run();
