namespace PresidentCountyAPI.Models;

using Google.Cloud.Firestore;
[FirestoreData]
public class PresidentCountyCandidate
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("state")]
    public string? State { get; set; }

    [FirestoreProperty("county")]
    public string? County { get; set; }

    [FirestoreProperty("candidate")]
    public string? CandidateName { get; set; }

    [FirestoreProperty("party")]
    public string? Party { get; set; }

    [FirestoreProperty("total_votes")]
    public string? TotalVotes { get; set; }

    [FirestoreProperty("won")]
    public string? Won { get; set; }
}

