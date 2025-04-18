using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WritingServer.Models;

public class QuestionSet
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string QuestionSetId { get; set; } = string.Empty;
    public string QuestionSetName { get; set; } = string.Empty;
    public int QuestionSetOrder { get; set; } = 0;
    public bool Locked { get; set; } = true;
    public QuestionModels Questions { get; set; } = new();
}

public class QuestionSetList
{
    public List<QuestionSet> QuestionSets { get; set; } = [];
}

public class QuestionModels
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string ExpectedResultText { get; set; } = string.Empty;
    public int QuestionOrder { get; set; } = 0;
    public int MinWordLength { get; set; }
    public int MaxWordLength { get; set; }
    public ResponseList ResponseList { get; set; } = new();
}

public class QuestionModelList
{
    public List<QuestionModels> Questions { get; set; } = [];
}

public class ResponseModels
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string ResponseId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string ResponseText { get; set; } = string.Empty;
    public DateTime ResponseTime { get; set; }
}

public class ResponseList
{
    public List<ResponseModels> Response { get; set; } = [];
}