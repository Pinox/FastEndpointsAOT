namespace Shared.Contracts.Uploads;

/// <summary>
/// Base request for multipart file upload.
/// Web endpoint adds IFormFile property for actual file binding.
/// </summary>
public class MultipartUploadRequestBase
{
    /// <summary>
    /// Title for the uploaded file
    /// </summary>
    public virtual string? Title { get; set; }
    
    /// <summary>
    /// Description for the uploaded file
    /// </summary>
    public virtual string? Description { get; set; }
}

/// <summary>
/// Base request to save an image with dimensions.
/// Web endpoint may add IFormFile properties for typed file uploads.
/// </summary>
public class ImageSaveRequestBase
{
    public virtual string ID { get; set; } = "";
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
}

/// <summary>
/// Base request for typed image upload with multiple files.
/// Web endpoint adds IFormFile properties.
/// </summary>
public class ImageSaveTypedRequestBase : ImageSaveRequestBase
{
    public virtual Guid GuidId { get; set; }
}
