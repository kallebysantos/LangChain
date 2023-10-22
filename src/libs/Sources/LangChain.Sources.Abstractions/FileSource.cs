namespace LangChain.Sources;

/// <summary>
///
/// </summary>
public class FileSource : ISource
{
    /// <summary>
    ///
    /// </summary>
    public required string Path { get; init; }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    public async Task<IReadOnlyCollection<Document>> LoadAsync(
        CancellationToken cancellationToken = default
    )
    {
        var pageContent = await File.ReadAllTextAsync(Path, cancellationToken)
            .ConfigureAwait(false);

        return (Document.Empty with { PageContent = pageContent, }).AsArray();
    }
#else
    public Task<IReadOnlyCollection<Document>> LoadAsync(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var pageContent = File.ReadAllText(Path);
            var documents = (Document.Empty with { PageContent = pageContent, }).AsArray();

            return Task.FromResult(documents);
        }
        catch (Exception exception)
        {
            return Task.FromException<IReadOnlyCollection<Document>>(exception);
        }
    }
#endif
}
