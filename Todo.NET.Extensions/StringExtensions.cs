using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Todo.NET.Extensions;

public static class StringExtensions
{
    public static Guid ToGuid(this string input) => Guid.TryParse(input, out var result) ? result : Guid.Empty;

    public static int ToInt(this string input) => int.TryParse(input, out var result) ? result : 0;
    
    public static long ToLong(this string input) => long.TryParse(input, out var result) ? result : 0;
    
    public static double ToDouble(this string input) => double.TryParse(input, out var result) ? result : 0;

    public static bool ToBool(this char value) => value switch
    {
        '1' => true,
        '0' => false,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public static string JoinString(this IEnumerable<string> input, char separator) => string.Join(separator, input);

    public static string JoinString(this IEnumerable<string> input, string separator) => string.Join(separator, input);
    
    public static string ToQueryString(this object input)
    {
        var query = input.GetType()
            .GetProperties()
            .Where(x => x.GetValue(input) is not null)
            .Select(x => x.Name + "=" + WebUtility.UrlEncode(x.GetValue(input)?.ToString()))
            .JoinString("&");
        return query;
    }

    public static string ToBase64Encode(this string text) => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

    public static string ToBase64Decode(this string base64) => Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    
    public static string ToSha256(this string input)
    {
        if (string.IsNullOrEmpty(input)) return default!;
        var hashed = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var stringBuilder = new StringBuilder();
        foreach (var byteCode in hashed)
            stringBuilder.Append(byteCode.ToString("X2"));
        return stringBuilder.ToString();
    }

    public static string ToHmacSha512(this string input, string secret)
    {
        if (string.IsNullOrEmpty(input)) return default!;
        var hashed = HMACSHA512.HashData(
            Encoding.UTF8.GetBytes(secret), 
            Encoding.UTF8.GetBytes(input));
        var stringBuilder = new StringBuilder();
        foreach (var byteCode in hashed) 
            stringBuilder.Append(byteCode.ToString("X2"));
        return stringBuilder.ToString();
    }
    
    public static string ToMd5(this string input)
    {
        if (string.IsNullOrEmpty(input)) return default!;
        var hashBytes = MD5.HashData(Encoding.ASCII.GetBytes(input));
        var stringBuilder = new StringBuilder();
        foreach (var t in hashBytes)
            stringBuilder.Append(t.ToString("X2"));
        return stringBuilder.ToString();
    }
    
    public static bool IsPrimitiveType(object? obj)
    {
        if (obj == null) return false;

        switch (obj.GetType().Name)
        {
            case "Boolean":
            case "Byte":
            case "SByte":
            case "Int16":
            case "Int32":
            case "Int64":
            case "UInt16":
            case "UInt32":
            case "UInt64":
            case "Char":
            case "Double":
            case "Single":
                return true;

            default:
                return false;
        }
    }
    
    public static int GenerateRandomNumber(this int length)
    {
        var stringBuilder = new StringBuilder();
        var random = new Random();

        while (stringBuilder.Length < length)
        {
            var start = 0;
            const int end = 9;
            if (stringBuilder.Length == 0) start = 1;
            var num = random.Next(start, end);
            stringBuilder.Append(num + "");
        }

        return stringBuilder.ToString().ToInt();
    }

    public static string GenerateRandomString(this int length)
    {
        const string chars = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        
        return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
    }
}