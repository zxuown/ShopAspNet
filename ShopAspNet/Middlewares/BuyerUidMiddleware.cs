namespace ShopAspNet.Middlewares;

public class BuyerUidMiddleware
{
    private readonly RequestDelegate _next;
    public BuyerUidMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var uid = context.Request.Cookies["BuyerUid"];
        if (uid == null)
        {
            uid = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("BuyerUid", uid, new CookieOptions
            {
                Expires = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.FromHours(0))
            });
        }
        context.Items["BuyerUid"] = uid;

        await _next(context);
    }

}
public static class BuyerUidExtension
{
    public static IApplicationBuilder UseBuyerUid(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<BuyerUidMiddleware>();
    }
}
