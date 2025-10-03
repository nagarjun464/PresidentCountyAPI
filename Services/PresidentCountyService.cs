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

    public async Task<List<PresidentCountyCandidate>> GetPagedAsync(int page, int pageSize, string? search = null)
    {
        var all = await GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            all = all.Where(c =>
                (!string.IsNullOrEmpty(c.CandidateName) && c.CandidateName.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(c.County) && c.County.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(c.State) && c.State.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(c.Party) && c.Party.ToLower().Contains(search))
            ).ToList();
        }
        Console.WriteLine($"Search term: {search}, Total records after filter: {all.Count}");
        return all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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

    public async Task<List<PresidentCountyCandidate>> GetAllFiltered(string? search)
    {
        var snapshot = await _db.Collection(_collection).GetSnapshotAsync();
        var all = snapshot.Documents.Select(d => d.ConvertTo<PresidentCountyCandidate>()).ToList();

        if (!string.IsNullOrEmpty(search))
        {
            all = all.Where(c =>
                (c.CandidateName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.State?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.County?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        return all;
    }
}
