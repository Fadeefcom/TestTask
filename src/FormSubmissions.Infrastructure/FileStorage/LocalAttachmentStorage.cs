namespace FormSubmissions.Infrastructure.FileStorage;

public sealed class LocalAttachmentStorage : IAttachmentStorage
{
    private readonly string _basePath;

    public LocalAttachmentStorage()
    {
        _basePath = Path.Combine(AppContext.BaseDirectory, "attachments");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct)
    {
        var key = Guid.NewGuid().ToString("N");
        var dir = AttachmentPathBuilder.BuildRoot(_basePath, key);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        await using var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
        await content.CopyToAsync(fs, ct);

        return Path.GetRelativePath(_basePath, path).Replace('\\', '/');
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct)
    {
        var path = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        Stream s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
        return Task.FromResult(s);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        var path = Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }
}
