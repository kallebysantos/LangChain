namespace LangChain.Sources;

/// <summary>
/// Object for storing document
/// </summary>
/// /// <remarks>
/// - no BaseModel implementation from pydantic
/// - ported from langchain/docstore/document.py
/// </remarks>
/// <param name="PageContent"></param>
/// <param name="Metadata"></param>
public record Document(string PageContent, IReadOnlyDictionary<string, string> Metadata)
{
    /// <summary>
    /// Returns an Empty Document
    /// </summary>
    public static Document Empty { get; } =
        new(PageContent: string.Empty, Metadata: new Dictionary<string, string>());

    int LookupIndex;
    string? LookupStr;

    /// <summary>
    /// Paragraphs of the page.
    /// </summary>
    public IReadOnlyCollection<string> Paragraphs() =>
        PageContent.Split(new[] { "\n\n" }, StringSplitOptions.None);

    /// <summary>
    /// Summary of the page (the first paragraph)
    /// </summary>
    public string Summary() => Paragraphs().First();

    /// <summary>
    /// Lookup a term in the page, imitating cmd-F functionality.
    /// </summary>
    public string Lookup(string searchString)
    {
        if (searchString is null)
        {
            throw new ArgumentNullException(nameof(searchString));
        }

        // if there is a new search string, reset the index
        var shouldResetIndex = searchString.ToUpperInvariant() != LookupStr;

        if (shouldResetIndex)
        {
            LookupStr = searchString.ToUpperInvariant();
            LookupIndex = 0;
        }
        else
        {
            LookupIndex++;
        }

        // get all the paragraphs that contain the search string
        var lookups = Paragraphs().Where(p => p.ToUpperInvariant().Contains(LookupStr)).ToList();

        if (!lookups.Any())
        {
            return "No Results";
        }

        var lookup = lookups.ElementAtOrDefault(LookupIndex);

        if (lookup is null)
        {
            return "No More Results";
        }

        string resultPrefix = $"(Result {LookupIndex + 1}/{lookups.Count})";

        return $"{resultPrefix} {lookup}";
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var serializedMetadata = string.Join(", ", Metadata.Select(x => $"{{{x.Key}:{x.Value}}}"));
        return $"(PageContent='{PageContent}', LookupStr='{LookupStr}', Metadata={serializedMetadata}), LookupIndex={LookupIndex}";
    }
}
