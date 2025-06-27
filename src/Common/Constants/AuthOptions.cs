using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProjectTemplate.Shared.Constants;
public class AuthOptions
{
    public const int ExpireMinutes = 120; //access token expire 120 min
    public const int ExpireMinutesRefresh = 360; //refresh token expire 240 min
    public const string Issuer = "AuthServer"; // издатель токена
    public const string Audience = "AuthClient"; // потребитель токена
    public const int MaxDeviceCount = 3;
    const string SecretKey = "F3uuiIIKJnkj78843Fh1KJH7DMMTyv12hdjsUY78N";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(SecretKey));
}
