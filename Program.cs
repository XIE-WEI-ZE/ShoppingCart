using Microsoft.EntityFrameworkCore;
using prjECommerceDemo.Models;

var builder = WebApplication.CreateBuilder(args);

//  加入 MVC 架構服務（放最前）
builder.Services.AddControllersWithViews();

//  資料庫連線
builder.Services.AddDbContext<DbShoppingCartExerciseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbConn")));

//  Google 驗證碼設定注入
builder.Services.Configure<GoogleReCaptchaSettings>(
    builder.Configuration.GetSection("GoogleReCaptcha"));

//  Session 設定
builder.Services.AddSession();

var app = builder.Build();

//  例外處理
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//  正確順序：Routing → Session → Authorization
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
