using System.Security.Cryptography;
using System.Text;

namespace FormSubmissions.Infrastructure.FileStorage;

public static class AttachmentPathBuilder
{
    public static string BuildRoot(string basePath, string storageKey)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(storageKey));
        var p1 = hash[0].ToString("x2");
        var p2 = hash[1].ToString("x2");
        return Path.Combine(basePath, p1, p2);
    }
}
