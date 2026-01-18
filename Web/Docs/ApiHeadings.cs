namespace Web.Docs;

public static class ApiHeadings
{
    public const string API = "API";
    public const string MinimalAPI = "Minimal API";
    
    public const string Admin = "Admin";
    public const string Basics = "Basics";
    public const string Binding = "Binding";
    public const string Domain = "Domain";
    public const string Pipeline = "Pipeline";
    public const string Security = "Security";
    public const string Uploads = "Uploads";
    public const string Versioning = "Versioning";
    public const string Hostings = "Hostings";
    public const string TestCases = "TestCases";
    
    
    public static readonly IReadOnlyList<string> AllNames = new[]
    {
        API,
        Admin,
        Basics,
        Binding,
        Domain,
        Pipeline,
        Security,
        Uploads,
        Versioning,
        Hostings,
        TestCases
    };
}
