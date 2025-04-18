namespace WritingServer.Services;

public interface IUserTokenService
{
    void StoreToken(string token, string username);
    string? GetUsernameFromToken(string token);
    bool ValidateToken(string token);
    void ClearToken(string token);
    void RemoveTokensForUser(string username);
    void UpdateUserInTokens(string oldUsername, string newUsername);
}

public class UserTokenService : IUserTokenService
{
    private readonly Dictionary<string, string> _userTokens = new(); // token -> username

    public void StoreToken(string token, string username)
    {
        _userTokens[token] = username;
    }

    public string? GetUsernameFromToken(string token)
    {
        return _userTokens.TryGetValue(token, out var username) ? username : null;
    }

    public bool ValidateToken(string token)
    {
        return _userTokens.ContainsKey(token);
    }

    public void ClearToken(string token)
    {
        _userTokens.Remove(token);
    }
    
    public void RemoveTokensForUser(string username)
    {
        var tokensToRemove = _userTokens
            .Where(kvp => kvp.Value == username)
            .Select(kvp => kvp.Key)
            .ToList();
            
        foreach (var token in tokensToRemove)
        {
            _userTokens.Remove(token);
        }
    }
    
    public void UpdateUserInTokens(string oldUsername, string newUsername)
    {
        foreach (var key in _userTokens.Keys.ToList())
        {
            if (_userTokens[key] == oldUsername)
            {
                _userTokens[key] = newUsername;
            }
        }
    }
}