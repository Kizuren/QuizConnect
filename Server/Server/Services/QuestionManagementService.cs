using MongoDB.Driver;
using WritingServer.Models;
using WritingServer.Utils;

namespace WritingServer.Services;

public interface IQuestionManagementService
{
    // Question Set operations
    Task<QuestionSet> CreateQuestionSetAsync(string name);
    Task<bool> UpdateQuestionSetOrderAsync(string questionSetId, int order);
    Task<bool> UpdateQuestionSetLockStatusAsync(string questionSetId, bool locked);
    Task<bool> UpdateQuestionSetNameAsync(string questionSetId, string newName);
    Task<bool> DeleteQuestionSetAsync(string questionSetId);
    Task<QuestionSetList> GetAllQuestionSetsAsync();
    Task<QuestionSet> GetQuestionSetByIdAsync(string questionSetId);
    
    // Question operations
    Task<QuestionModels> CreateQuestionAsync(string questionSetId, string questionText, string expectedResultText, 
        int questionOrder, int minWordLength, int maxWordLength);
    Task<QuestionModels> UpdateQuestionAsync(string questionId, string questionText, string expectedResultText, 
        int questionOrder, int minWordLength, int maxWordLength);
    Task<bool> DeleteQuestionAsync(string questionId);
    Task<QuestionModelList> GetQuestionsInSetAsync(string questionSetId);
    Task<QuestionModels> GetQuestionByIdAsync(string questionId);
    
    // Response operations
    Task<bool> SubmitResponseAsync(string questionId, string userName, string responseText);
    Task<ResponseList> GetResponsesForQuestionAsync(string questionId);
    Task<ResponseList> GetResponsesForUserAsync(string userName, string questionId);
}

public class QuestionManagementService : IQuestionManagementService
{
    private readonly IMongoCollection<QuestionSet> _questionSetsCollection;
    private readonly IMongoCollection<QuestionModels> _questionsCollection;
    private readonly IMongoCollection<ResponseModels> _responsesCollection;
    private readonly IDiscordNotificationService _discordNotificationService;

    public QuestionManagementService(IMongoDatabase database, IDiscordNotificationService discordNotificationService)
    {
        _questionSetsCollection = database.GetCollection<QuestionSet>("QuestionSets");
        _questionsCollection = database.GetCollection<QuestionModels>("Questions");
        _responsesCollection = database.GetCollection<ResponseModels>("Responses");
        _discordNotificationService = discordNotificationService;
    }

    #region Question Set operations
    public async Task<QuestionSet> CreateQuestionSetAsync(string name)
    {
        var questionSet = new QuestionSet
        {
            QuestionSetId = TokenGenerator.GenerateToken(16),
            QuestionSetName = name,
            QuestionSetOrder = await GetNextQuestionSetOrderAsync(),
            Locked = true
        };
        
        await _questionSetsCollection.InsertOneAsync(questionSet);
        return questionSet;
    }
    
    private async Task<int> GetNextQuestionSetOrderAsync()
    {
        var maxOrder = await _questionSetsCollection
            .Find(_ => true)
            .SortByDescending(qs => qs.QuestionSetOrder)
            .Limit(1)
            .Project(qs => qs.QuestionSetOrder)
            .FirstOrDefaultAsync();
        
        return maxOrder + 1;
    }

    public async Task<bool> UpdateQuestionSetOrderAsync(string questionSetId, int order)
    {
        var result = await _questionSetsCollection.UpdateOneAsync(
            qs => qs.QuestionSetId == questionSetId,
            Builders<QuestionSet>.Update.Set(qs => qs.QuestionSetOrder, order));
        
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateQuestionSetLockStatusAsync(string questionSetId, bool locked)
    {
        var result = await _questionSetsCollection.UpdateOneAsync(
            qs => qs.QuestionSetId == questionSetId,
            Builders<QuestionSet>.Update.Set(qs => qs.Locked, locked));
        
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateQuestionSetNameAsync(string questionSetId, string newName)
    {
        var result = await _questionSetsCollection.UpdateOneAsync(
            qs => qs.QuestionSetId == questionSetId,
            Builders<QuestionSet>.Update.Set(qs => qs.QuestionSetName, newName));
        
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteQuestionSetAsync(string questionSetId)
    {
        // First, find all questions in this set
        var questions = await _questionsCollection
            .Find(q => q.QuestionId.StartsWith(questionSetId + "_"))
            .ToListAsync();
        
        // Delete all responses for questions in this set
        foreach (var question in questions)
        {
            await _responsesCollection.DeleteManyAsync(r => r.QuestionId == question.QuestionId);
        }
        
        // Delete all questions in this set
        await _questionsCollection.DeleteManyAsync(q => q.QuestionId.StartsWith(questionSetId + "_"));
        
        // Delete the question set
        var result = await _questionSetsCollection.DeleteOneAsync(qs => qs.QuestionSetId == questionSetId);
        return result.DeletedCount > 0;
    }

    public async Task<QuestionSetList> GetAllQuestionSetsAsync()
    {
        var questionSets = await _questionSetsCollection
            .Find(_ => true)
            .SortBy(qs => qs.QuestionSetOrder)
            .ToListAsync();
        
        return new QuestionSetList { QuestionSets = questionSets };
    }

    public async Task<QuestionSet> GetQuestionSetByIdAsync(string questionSetId)
    {
        return await _questionSetsCollection
            .Find(qs => qs.QuestionSetId == questionSetId)
            .FirstOrDefaultAsync();
    }
    #endregion
    
    #region Question operations
    public async Task<QuestionModels> CreateQuestionAsync(string questionSetId, string questionText, string expectedResultText, 
        int questionOrder, int minWordLength, int maxWordLength)
    {
        var question = new QuestionModels
        {
            QuestionId = $"{questionSetId}_{TokenGenerator.GenerateToken(8)}",
            QuestionText = questionText,
            ExpectedResultText = expectedResultText,
            QuestionOrder = questionOrder,
            MinWordLength = minWordLength,
            MaxWordLength = maxWordLength
        };
        
        await _questionsCollection.InsertOneAsync(question);
        return question;
    }

    public async Task<QuestionModels> UpdateQuestionAsync(string questionId, string questionText, string expectedResultText, 
        int questionOrder, int minWordLength, int maxWordLength)
    {
        var update = Builders<QuestionModels>.Update
            .Set(q => q.QuestionText, questionText)
            .Set(q => q.ExpectedResultText, expectedResultText)
            .Set(q => q.QuestionOrder, questionOrder)
            .Set(q => q.MinWordLength, minWordLength)
            .Set(q => q.MaxWordLength, maxWordLength);
        
        await _questionsCollection.UpdateOneAsync(q => q.QuestionId == questionId, update);
        return await GetQuestionByIdAsync(questionId);
    }

    public async Task<bool> DeleteQuestionAsync(string questionId)
    {
        await _responsesCollection.DeleteManyAsync(r => r.QuestionId == questionId);
        
        var result = await _questionsCollection.DeleteOneAsync(q => q.QuestionId == questionId);
        return result.DeletedCount > 0;
    }

    public async Task<QuestionModelList> GetQuestionsInSetAsync(string questionSetId)
    {
        var questions = await _questionsCollection
            .Find(q => q.QuestionId.StartsWith(questionSetId + "_"))
            .SortBy(q => q.QuestionOrder)
            .ToListAsync();
        
        return new QuestionModelList { Questions = questions };
    }

    public async Task<QuestionModels> GetQuestionByIdAsync(string questionId)
    {
        return await _questionsCollection
            .Find(q => q.QuestionId == questionId)
            .FirstOrDefaultAsync();
    }
    #endregion
    
    #region Response operations
    public async Task<bool> SubmitResponseAsync(string questionId, string userName, string responseText)
    {
        var response = new ResponseModels
        {
            ResponseId = TokenGenerator.GenerateToken(16),
            UserName = userName,
            QuestionId = questionId,
            ResponseText = responseText,
            ResponseTime = DateTime.UtcNow
        };
        
        await _responsesCollection.InsertOneAsync(response);
        
        // Discord webhook
        var question = await GetQuestionByIdAsync(questionId);
        string questionSetId = questionId.Split('_')[0];
        var questionSet = await GetQuestionSetByIdAsync(questionSetId);
        await _discordNotificationService.SendResponseNotification(
            userName, 
            questionSet?.QuestionSetName ?? "Unknown Set", 
            question?.QuestionText ?? "Unknown Question", 
            responseText);
        
        return true;
    }

    public async Task<ResponseList> GetResponsesForQuestionAsync(string questionId)
    {
        var responses = await _responsesCollection
            .Find(r => r.QuestionId == questionId)
            .SortByDescending(r => r.ResponseTime)
            .ToListAsync();
        
        return new ResponseList { Response = responses };
    }

    public async Task<ResponseList> GetResponsesForUserAsync(string userName, string questionId)
    {
        var responses = await _responsesCollection
            .Find(r => r.QuestionId == questionId && r.UserName == userName)
            .SortByDescending(r => r.ResponseTime)
            .ToListAsync();
        
        return new ResponseList { Response = responses };
    }
    #endregion
}