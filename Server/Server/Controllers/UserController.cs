using Microsoft.AspNetCore.Mvc;
using WritingServer.Models;
using WritingServer.Services;

namespace WritingServer.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IQuestionManagementService _questionManagementService;

    public UserController(IUserManagementService userManagementService, IQuestionManagementService questionManagementService)
    {
        _userManagementService = userManagementService;
        _questionManagementService = questionManagementService;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponse>> Login([FromBody] UserLoginRequest request)
    {
        var user = await _userManagementService.GetUserByPinAsync(request.Pin);
        
        if (user == null)
        {
            return Ok(new UserLoginResponse
            { 
                Success = false, 
                ErrorMessage = "Invalid PIN" 
            });
        }

        var accessToken = _userManagementService.GenerateAccessToken(user.Username);
        if (user.ResetState)
        {
            _userManagementService.ResetLoginState(user.Username);
        }

        return Ok(new UserLoginResponse 
        { 
            Success = true, 
            AccessToken = accessToken 
        });
    }
    
    [HttpDelete("logout")]
    public ActionResult<UserLogoutResponse> Logout([FromBody] UserLogoutRequest request)
    {
        if (request.AccessToken == string.Empty || !_userManagementService.ClearToken(request.AccessToken))
        {
            return Ok(new UserLogoutResponse 
            { 
                Success = false,
                ErrorMessage = "Invalid access token"
            });
        }

        return Ok(new UserLogoutResponse 
        { 
            Success = true
        });
    }

    [HttpGet("username")]
    public async Task<ActionResult<UserGetUsernameResponse>> GetUsername()
    {
        string? token = GetBearerToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new UserGetUsernameResponse
            {
                Success = false,
                ErrorMessage = "Missing or invalid authorization token"
            });
        }
        
        var user = await _userManagementService.GetUserByTokenAsync(token);
        
        if (user == null)
        {
            return Unauthorized(new UserGetUsernameResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        return Ok(new UserGetUsernameResponse 
        { 
            Success = true, 
            Username = user.Username 
        });
    }

    [HttpGet("state")]
    public async Task<ActionResult<UserGetStateResponse>> GetState()
    {
        string? token = GetBearerToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new UserGetStateResponse
            {
                Success = false,
                ErrorMessage = "Missing or invalid authorization token"
            });
        }

        var user = await _userManagementService.GetUserByTokenAsync(token);
        
        if (user == null)
        {
            return Unauthorized(new UserGetStateResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        return Ok(new UserGetStateResponse 
        { 
            Success = true, 
            ResetState = user.ResetState 
        });
    }
    
    [HttpGet("questionsets")]
    public async Task<ActionResult<UserGetQuestionSetsResponse>> GetQuestionSets()
    {
        string? token = GetBearerToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new UserGetStateResponse
            {
                Success = false,
                ErrorMessage = "Missing or invalid authorization token"
            });
        }

        var user = await _userManagementService.GetUserByTokenAsync(token);

        if (user == null)
        {
            return Unauthorized(new UserGetQuestionSetsResponse
            {
                Success = false,
                ErrorMessage = "Invalid access token"
            });
        }

        var questionSets = await _questionManagementService.GetAllQuestionSetsAsync();
        
        // Only return unlocked question sets to users
        questionSets.QuestionSets = questionSets.QuestionSets.Where(qs => !qs.Locked).ToList();

        return Ok(new UserGetQuestionSetsResponse
        {
            Success = true,
            QuestionSets = questionSets
        });
    }

    [HttpGet("questions")]
    public async Task<ActionResult<UserGetQuestionsResponse>> GetQuestions([FromQuery] string questionSetId)
    {
        string? token = GetBearerToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new UserGetStateResponse
            {
                Success = false,
                ErrorMessage = "Missing or invalid authorization token"
            });
        }

        var user = await _userManagementService.GetUserByTokenAsync(token);

        if (user == null)
        {
            return Unauthorized(new UserGetQuestionsResponse
            {
                Success = false,
                ErrorMessage = "Invalid access token"
            });
        }
        
        var questionSet = await _questionManagementService.GetQuestionSetByIdAsync(questionSetId);
        if (questionSet.Locked)
        {
            return BadRequest(new UserGetQuestionsResponse
            {
                Success = false,
                ErrorMessage = "This question set is locked"
            });
        }

        var questions = await _questionManagementService.GetQuestionsInSetAsync(questionSetId);
        return Ok(new UserGetQuestionsResponse
        {
            Success = true,
            Questions = questions
        });
    }

    [HttpPost("responses")]
    public async Task<ActionResult<UserSubmitResponseResponse>> SubmitResponse([FromBody] UserSubmitResponseRequest request)
    {
        string? token = GetBearerToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new UserGetStateResponse
            {
                Success = false,
                ErrorMessage = "Missing or invalid authorization token"
            });
        }

        var user = await _userManagementService.GetUserByTokenAsync(token);

        if (user == null)
        {
            return Unauthorized(new UserSubmitResponseResponse
            {
                Success = false,
                ErrorMessage = "Invalid access token"
            });
        }

        if (string.IsNullOrWhiteSpace(request.ResponseText))
        {
            return BadRequest(new UserSubmitResponseResponse
            {
                Success = false,
                ErrorMessage = "Response text cannot be empty"
            });
        }
        
        var question = await _questionManagementService.GetQuestionByIdAsync(request.QuestionId);
        
        var wordCount = request.ResponseText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        if (wordCount < question.MinWordLength || wordCount > question.MaxWordLength)
        {
            return BadRequest(new UserSubmitResponseResponse
            {
                Success = false,
                ErrorMessage = $"Response must be between {question.MinWordLength} and {question.MaxWordLength} words"
            });
        }

        await _questionManagementService.SubmitResponseAsync(request.QuestionId, user.Username, request.ResponseText);
        return Ok(new UserSubmitResponseResponse { Success = true });
    }

    [HttpGet("responses")]
    public async Task<ActionResult<UserGetResponsesResponse>> GetResponses([FromQuery] string questionId)
    {
        string? token = GetBearerToken();
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new UserGetStateResponse
            {
                Success = false,
                ErrorMessage = "Missing or invalid authorization token"
            });
        }

        var user = await _userManagementService.GetUserByTokenAsync(token);

        if (user == null)
        {
            return Unauthorized(new UserGetResponsesResponse
            {
                Success = false,
                ErrorMessage = "Invalid access token"
            });
        }

        var responses = await _questionManagementService.GetResponsesForUserAsync(user.Username, questionId);
        return Ok(new UserGetResponsesResponse
        {
            Success = true,
            Responses = responses
        });
    }
    
    private string? GetBearerToken()
    {
        if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return null;
        }
    
        string auth = authHeader.ToString();
        if (!auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        
        return auth.Substring(7).Trim();
    }
}