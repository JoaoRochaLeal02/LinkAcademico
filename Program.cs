using LinkAcademico.Data;
using LinkAcademico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

//
// 🌍 LOCALIZAÇÃO (PORTUGUÊS)
//
var supportedCultures = new[]
{
    new CultureInfo("pt-BR")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture =
        new RequestCulture("pt-BR");

    options.SupportedCultures =
        supportedCultures;

    options.SupportedUICultures =
        supportedCultures;
});

//
// 🔗 BANCO DE DADOS
//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

//
// 🔐 IDENTITY + ROLES
//
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    //
    // 🔒 CONFIG SENHA
    //
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

//
// MVC
//
builder.Services.AddControllersWithViews();

//
// 🔐 RAZOR PAGES (IDENTITY)
//
builder.Services.AddRazorPages();

var app = builder.Build();

//
// 🌍 LOCALIZAÇÃO
//
app.UseRequestLocalization();

//
// PIPELINE
//
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

//
// 🔐 AUTH
//
app.UseAuthentication();

app.UseAuthorization();

//
// 🔥 ROTA PADRÃO
//
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//
// 🔐 IDENTITY
//
app.MapRazorPages();

//
// 🔥 CRIAR ROLES AUTOMATICAMENTE
//
using (var scope = app.Services.CreateScope())
{
    var services =
        scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles =
    {
        "Administrador",
        "Empresa",
        "Estudante"
    };

    foreach (var role in roles)
    {
        var exists =
            await roleManager.RoleExistsAsync(role);

        if (!exists)
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }
}

app.Run();