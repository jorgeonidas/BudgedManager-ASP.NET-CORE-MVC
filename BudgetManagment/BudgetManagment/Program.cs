using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositoryAccountTypes, RepositoryAccountTypes>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IRepositoryAccounts, RepositoryAccounts>();
builder.Services.AddTransient<IRepositoryCategories, RepositoryCategories>();
builder.Services.AddTransient<IRepositoryTransactions, RepositoryTransactions>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IReportsService, ReportsService>();
builder.Services.AddTransient<IRepositoryUsers, RepositoryUsers>();
builder.Services.AddTransient<IUserStore<User>, UserStore>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transactions}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
