using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WritingServer.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public bool ResetState { get; set; } = false;
}

public class UserModelList
{
    public List<UserModel> Users { get; set; } = [];
}