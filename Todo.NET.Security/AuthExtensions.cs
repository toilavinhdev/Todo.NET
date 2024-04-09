using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Todo.NET.Security;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtBearerAuth(this IServiceCollection services, 
                                                      string tokenSingingKey, 
                                                      Action<JwtBearerOptions>? jwtOptions = null)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    o =>
                    {
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSingingKey));
                        
                        // defaults
                        o.TokenValidationParameters.IssuerSigningKey = key;
                        o.TokenValidationParameters.ValidateIssuerSigningKey = true;
                        o.TokenValidationParameters.ValidateLifetime = true;
                        o.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(60);
                        o.TokenValidationParameters.ValidAudience = null;
                        o.TokenValidationParameters.ValidateAudience = false;
                        o.TokenValidationParameters.ValidIssuer = null;
                        o.TokenValidationParameters.ValidateIssuer = false;
                        
                        // override
                        jwtOptions?.Invoke(o);

                        // correct mistake
                        o.TokenValidationParameters.ValidateAudience = o.TokenValidationParameters.ValidAudience is not null;
                        o.TokenValidationParameters.ValidateIssuer = o.TokenValidationParameters.ValidIssuer is not null;
                    });
        
        return services;
    }
    
    public static IServiceCollection AddCookieAuth(this IServiceCollection services,
                                                   TimeSpan validFor,
                                                   Action<CookieAuthenticationOptions>? options = null)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    o =>
                    {
                        o.ExpireTimeSpan = validFor;
                        o.Cookie.MaxAge = validFor;
                        o.Cookie.HttpOnly = true;
                        o.Cookie.SameSite = SameSiteMode.Lax;
                        options?.Invoke(o);
                    });

        return services;
    }
}