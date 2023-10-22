namespace LangChain.Sources;

/// <summary>
///
/// </summary>
public interface ISource
{
    /// <summary>
    /// Load data into Document objects.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyCollection<Document>> LoadAsync(CancellationToken cancellationToken = default);

#if NET6_0_OR_GREATER
    /// <summary>
    /// A lazy loader for Documents.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<Document> LazyLoad();
#endif

    /// <summary>
    /// Load Documents and split into chunks. Chunks are returned as Documents.
    /// </summary>
    ///
    /// <param name="textSplitter">
    /// TextSplitter instance to use for splitting documents.
    /// Defaults to RecursiveCharacterTextSplitter.
    /// </param>
    /// <returns></returns>
    // List<Document> LoadAndSplit(TextSplitter? textSplitter)
    // {
    //     textSplitter ??= new RecursiveCharacterTextSplitter();

    //     var documents = Load();

    //     return textSplitter.SplitDocuments(documents);
    // }
}
