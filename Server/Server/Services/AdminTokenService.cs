namespace WritingServer.Services;

public interface IAdminTokenService
{
    void StoreToken(string token);
    bool ValidateToken(string token);
    bool ClearToken(string token);
}

public class AdminTokenService : IAdminTokenService
{
    private readonly Dictionary<string, bool> _adminTokens = new();
    
    public void StoreToken(string token)
    {
        _adminTokens[token] = true;
    }
    
    public bool ValidateToken(string token)
    {
        return _adminTokens.ContainsKey(token) && _adminTokens[token];
    }

    public bool ClearToken(string token) => _adminTokens.Remove(token);
}