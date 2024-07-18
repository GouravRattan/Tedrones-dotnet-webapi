using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using MyCommonStructure.Services;

WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    s.AddSingleton<register>();
    s.AddSingleton<login>();
    s.AddSingleton<changePassword>();
    s.AddSingleton<resetPassword>();
    s.AddSingleton<editProfile>();
    s.AddSingleton<deleteProfile>();
    s.AddSingleton<contactUs>();
    s.AddSingleton<drones>();
    s.AddSingleton<carts>();
    s.AddSingleton<users>();

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
    app.UseCors(options =>
             options.WithOrigins("https://localhost:5001", "http://localhost:5002")
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseRouting();
    app.UseStaticFiles();

    app.UseAuthorization();
    app.UseAuthentication();

    app.UseEndpoints(e =>
    {
        var register = e.ServiceProvider.GetRequiredService<register>();
        e.MapPost("/registration",
        [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await register.Registration(rData)); // register
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await register.GetUserByEmail(rData)); // get users details via email
        });

        var login = e.ServiceProvider.GetRequiredService<login>();
        e.MapPost("/login",
        [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await login.Login(rData)); // login
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await login.Logout(rData)); // logout
        });

        var changePassword = e.ServiceProvider.GetRequiredService<changePassword>();
        e.MapPost("/changePassword", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await changePassword.ChangePassword(rData)); // change Password
        });

        var resetPassword = e.ServiceProvider.GetRequiredService<resetPassword>();
        e.MapPost("/resetPassword", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await resetPassword.ResetPassword(rData)); // reset Password
        });

        var editProfile = e.ServiceProvider.GetRequiredService<editProfile>();
        e.MapPost("/editProfile", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await editProfile.EditProfile(rData)); // edit profile
        });

        var deleteProfile = e.ServiceProvider.GetRequiredService<deleteProfile>();
        e.MapPost("/deleteProfile", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfileByUser(rData)); // delete profile by user
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfileByAdmin(rData)); // delete profile by admin
        });

        var contactUs = e.ServiceProvider.GetRequiredService<contactUs>(); // for contact details
        e.MapPost("/contactUs", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await contactUs.ContactUs(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await contactUs.DeleteContactById(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await contactUs.GetContactById(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await contactUs.GetAllContacts(rData));
        });


        var drones = e.ServiceProvider.GetRequiredService<drones>();
        e.MapPost("/drones", [AllowAnonymous] async (HttpContext http) => // for drone details
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
        e.MapPost("/carts", [AllowAnonymous] async (HttpContext http) => // for cart details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await carts.AddToCart(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await carts.UpdateInCart(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await carts.RemoveFromCart(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await carts.GetACartItem(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await carts.GetAllCartItems(rData));
        });

        var users = e.ServiceProvider.GetRequiredService<users>();
        e.MapPost("/users", [AllowAnonymous] async (HttpContext http) => // for user details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await users.GetAllUsers(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await users.GetUserById(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await users.DeleteUserById(rData));
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
