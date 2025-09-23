namespace PresidentCountyAPI.Services;
using Google.Cloud.Firestore;
using PresidentCountyAPI.Models;

public class PresidentCountyService
{
    private readonly FirestoreDb _db;
    private readonly string _collection = "PresidentCountyCandidates";

    public PresidentCountyService(string projectId, string credentialsPath)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
        _db = FirestoreDb.Create(projectId);
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
