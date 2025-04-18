using MongoDB.Driver;
using WritingServer.Models;
using WritingServer.Utils;

namespace WritingServer.Services;

public interface IUserManagementService
{
    Task<UserModel> AddUserAsync(string userName);
    Task<bool> DeleteUserAsync(string userName);
    Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName);
    Task<bool> ResetUserAsync(string userName);
    Task<UserModelList> GetAllUsersAsync();
    Task<UserModel?> GetUserByPinAsync(string pin);
    Task<UserModel?> GetUserByTokenAsync(string accessToken);
    Task<UserModel?> GetUserByNameAsync(string userName);
    string GenerateAccessToken(string username);
    void ResetLoginState(string userName);
}

public class UserManagementService : IUserManagementService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<UserModel> _usersCollection;
    private readonly IMongoCollection<ResponseModels> _responsesCollection;
    private readonly IUserTokenService _tokenService;

    public UserManagementService(IMongoDatabase database, IUserTokenService tokenService)
    {
        _database = database;
        _usersCollection = database.GetCollection<UserModel>("Users");
        _responsesCollection = database.GetCollection<ResponseModels>("Responses");
        _tokenService = tokenService;
    }

    public async Task<UserModel> AddUserAsync(string userName)
    {
        var existingUser = await _usersCollection.Find(u => u.Username == userName).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            return existingUser;
        }
        
        var pin = TokenGenerator.IdGenerator(4);
        
        var user = new UserModel
        {
            Username = userName,
            Pin = pin,
            ResetState = false
        };

        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    public async Task<bool> DeleteUserAsync(string userName)
    {
        var deleteResponseResult = await _responsesCollection.DeleteManyAsync(r => r.UserName == userName);
        
        var deleteUserResult = await _usersCollection.DeleteOneAsync(u => u.Username == userName);
        _tokenService.RemoveTokensForUser(userName);

        return deleteUserResult.DeletedCount > 0;
    }

    public async Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName)
    {
        var existingUser = await _usersCollection.Find(u => u.Username == newUserName).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            return false;
        }

        // Update username
        var updateResult = await _usersCollection.UpdateOneAsync(
            u => u.Username == currentUserName,
            Builders<UserModel>.Update.Set(u => u.Username, newUserName));

        // Update references in responses
        await _responsesCollection.UpdateManyAsync(
            r => r.UserName == currentUserName,
            Builders<ResponseModels>.Update.Set(r => r.UserName, newUserName));
        
        _tokenService.UpdateUserInTokens(currentUserName, newUserName);

        return updateResult.ModifiedCount > 0;
    }

    public async Task<bool> ResetUserAsync(string userName)
    {
        await _responsesCollection.DeleteManyAsync(r => r.UserName == userName);

        // Set reset flag
        var updateResult = await _usersCollection.UpdateOneAsync(
            u => u.Username == userName,
            Builders<UserModel>.Update.Set(u => u.ResetState, true));

        return updateResult.ModifiedCount > 0;
    }

    public async Task<UserModelList> GetAllUsersAsync()
    {
        var users = await _usersCollection.Find(_ => true).ToListAsync();
        return new UserModelList { Users = users };
    }

    public async Task<UserModel?> GetUserByPinAsync(string pin)
    {
        return await _usersCollection.Find(u => u.Pin == pin).FirstOrDefaultAsync();
    }

    public async Task<UserModel?> GetUserByNameAsync(string userName)
    {
        return await _usersCollection.Find(u => u.Username == userName).FirstOrDefaultAsync();
    }

    public async Task<UserModel?> GetUserByTokenAsync(string accessToken)
    {
        var username = _tokenService.GetUsernameFromToken(accessToken);
        return username != null ? await GetUserByNameAsync(username) : null;
    }

    // Helper methods for token management
    public string GenerateAccessToken(string userName)
    {
        var token = TokenGenerator.GenerateToken(32);
        _tokenService.StoreToken(token, userName);
        return token;
    }

    public void ResetLoginState(string userName)
    {
        _usersCollection.UpdateOne(
            u => u.Username == userName,
            Builders<UserModel>.Update.Set(u => u.ResetState, false));
    }
}