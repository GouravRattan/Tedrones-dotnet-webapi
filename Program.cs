using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using COMMON_PROJECT_STRUCTURE_API.services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using MyCommonStructure.Services;

WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    s.AddSingleton<login>();
    s.AddSingleton<register>();
    s.AddSingleton<changePassword>();
    s.AddSingleton<resetPassword>();
    s.AddSingleton<editProfile>();
    s.AddSingleton<deleteProfile>();
    s.AddSingleton<drones>();
    s.AddSingleton<carts>();
    s.AddSingleton<contactUs>();

    s.AddCors();
    s.AddControllers();
    s.AddAuthorization();
    s.AddAuthentication("SourceJWT").AddScheme<SourceJwtAuthenticationSchemeOptions, SourceJwtAuthenticationHandler>("SourceJWT", options =>
        {
            options.SecretKey = appsettings["jwt_config:Key"].ToString();
            options.ValidIssuer = appsettings["jwt_config:Issuer"].ToString();
            options.ValidAudience = appsettings["jwt_config:Audience"].ToString();
            options.Subject = appsettings["jwt_config:Subject"].ToString();
        });
}).Configure(app =>
{
    app.UseAuthorization();
    app.UseAuthentication();

    app.UseCors(options =>
             options.WithOrigins("https://localhost:5001", "http://localhost:5002")
            // options.WithOrigins("*")
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseRouting();
    app.UseStaticFiles();

    app.UseEndpoints(e =>
    {
        var login = e.ServiceProvider.GetRequiredService<login>();

        e.MapPost("login",
            [AllowAnonymous] async (HttpContext http) =>
            {
                var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
                requestData rData = JsonSerializer.Deserialize<requestData>(body);
                if (rData.eventID == "1001") // update
                    await http.Response.WriteAsJsonAsync(await login.Login(rData));
            });

        var register = e.ServiceProvider.GetRequiredService<register>();
        e.MapPost("registration",
        [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);

            if (rData.eventID == "1002") // update
                await http.Response.WriteAsJsonAsync(await register.Registration(rData));

            if (rData.eventID == "1009") // get users details via email
                await http.Response.WriteAsJsonAsync(await register.GetUserRegistrationByEmail(rData));
        });

        var changePassword = e.ServiceProvider.GetRequiredService<changePassword>();
        e.MapPost("changePassword", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1003") // change Password
                await http.Response.WriteAsJsonAsync(await changePassword.ChangePassword(rData));
        });

        var resetPassword = e.ServiceProvider.GetRequiredService<resetPassword>();
        e.MapPost("resetPassword", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1003") // reset Password
                await http.Response.WriteAsJsonAsync(await resetPassword.ResetPassword(rData));
        });

        var editProfile = e.ServiceProvider.GetRequiredService<editProfile>();
        e.MapPost("editProfile", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await editProfile.EditProfile(rData));
        });

        var deleteProfile = e.ServiceProvider.GetRequiredService<deleteProfile>();
        e.MapPost("deleteProfile", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfile(rData));
        });

        var contactUs = e.ServiceProvider.GetRequiredService<contactUs>();
        e.MapPost("contactUs", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1006") await http.Response.WriteAsJsonAsync(await contactUs.ContactUs(rData));
        });


        var drones = e.ServiceProvider.GetRequiredService<drones>();
        e.MapPost("/drones", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await drones.AddDrone(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await drones.EditDrone(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await drones.DeleteDrone(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await drones.GetDrone(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await drones.GetAllDrones(rData));
        });

        var carts = e.ServiceProvider.GetRequiredService<carts>();
        e.MapPost("/carts", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await carts.AddToCart(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await carts.UpdateInCart(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await carts.RemoveFromCart(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await carts.GetACartItem(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await carts.GetAllCartItems(rData));
        });

        e.MapGet("/bing",
          async c => await c.Response.WriteAsJsonAsync("{'Name':'Gourav','Age':'22','Project':'TEDrones'}"));
    });
}).Build().Run();

public record requestData
{
    [Required]
    public string eventID { get; set; }
    [Required]
    public IDictionary<string, object> addInfo { get; set; }
}

public record responseData
{
    public responseData()
    {
        eventID = "";
        rStatus = 0;
        rData = new Dictionary<string, object>();
    }
    [Required]
    public int rStatus { get; set; } = 0;
    public string eventID { get; set; }
    public IDictionary<string, object> addInfo { get; set; }
    public IDictionary<string, object> rData { get; set; }
}
