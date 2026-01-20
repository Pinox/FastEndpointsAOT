using System.Linq.Expressions;

#pragma warning disable RCS1074
namespace FastEndpoints;

/// <summary>
/// a dto representing search parameters for pending job storage record retrieval.
/// Note: This is a class (not struct) for Native AOT compatibility.
/// </summary>
/// <typeparam name="TStorageRecord">the type of storage record</typeparam>
public sealed class PendingJobSearchParams<TStorageRecord> where TStorageRecord : IJobStorageRecord
{
    /// <summary>
    /// the ID of the job queue for fetching the next batch of records for.
    /// </summary>
    public string QueueID { get; internal set; }

    /// <summary>
    /// a boolean lambda expression to match the next batch of records
    /// <code>
    /// 	r => r.QueueID == "xxx" &amp;&amp;
    /// 	     !r.IsComplete &amp;&amp;
    /// 	     DateTime.UtcNow &gt;= r.ExecuteAfter &amp;&amp;
    /// 	     DateTime.UtcNow &lt;= r.ExpireOn
    /// </code>
    /// </summary>
    public Expression<Func<TStorageRecord, bool>> Match { get; internal set; }

    /// <summary>
    /// the number of pending records to fetch
    /// </summary>
    public int Limit { get; internal set; }

    /// <summary>
    /// cancellation token
    /// </summary>
    public CancellationToken CancellationToken { get; internal set; }
}