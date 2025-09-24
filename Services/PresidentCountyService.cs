namespace PresidentCountyAPI.Services;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using PresidentCountyAPI.Models;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

public class PresidentCountyService
{
    private readonly FirestoreDb _db;
    private readonly string _collection = "PresidentCountyCandidates";

    public PresidentCountyService(string projectId, string credentialsPath)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        _db = FirestoreDb.Create(projectId);
    }

    public async Task<List<PresidentCountyCandidate>> GetPagedAsync(int page, int pageSize)
    {
        var collection = _db.Collection("PresidentCountyCandidates")
            .OrderByDescending("total_votes");

        var snapshot = await collection
            .Offset((page - 1) * pageSize)
            .Limit(pageSize)
            .GetSnapshotAsync();

        return snapshot.Documents
            .Select(d => d.ConvertTo<PresidentCountyCandidate>())
            .ToList();
    }


    public async Task<List<PresidentCountyCandidate>> GetAllAsync()
    {
        var snapshot = await _db.Collection(_collection).GetSnapshotAsync();
        return snapshot.Documents.Select(d => d.ConvertTo<PresidentCountyCandidate>()).ToList();
    }

    public async Task<PresidentCountyCandidate> GetAsync(string id)
    {
        var doc = await _db.Collection(_collection).Document(id).GetSnapshotAsync();
        return doc.Exists ? doc.ConvertTo<PresidentCountyCandidate>() : null;
    }

    public async Task<string> CreateAsync(PresidentCountyCandidate candidate)
    {
        var docRef = await _db.Collection(_collection).AddAsync(candidate);
        return docRef.Id;
    }

    public async Task UpdateAsync(string id, PresidentCountyCandidate candidate)
    {
        await _db.Collection(_collection).Document(id).SetAsync(candidate, SetOptions.Overwrite);
    }

    public async Task DeleteAsync(string id)
    {
        await _db.Collection(_collection).Document(id).DeleteAsync();
    }
}
