using Microsoft.AspNetCore.Mvc;
using WritingServer.Models;
using WritingServer.Services;
using WritingServer.Utils;

namespace WritingServer.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserManagementService _userManagementService;
    private readonly IAdminTokenService _tokenService;
    private readonly IQuestionManagementService _questionManagementService;

    public AdminController(IConfiguration configuration, IUserManagementService userManagementService,
        IAdminTokenService tokenService, IQuestionManagementService questionManagementService)
    {
        _configuration = configuration;
        _userManagementService = userManagementService;
        _tokenService = tokenService;
        _questionManagementService = questionManagementService;
    }

    [HttpPost("login")]
    public ActionResult<AdminLoginResponse> Login([FromBody] AdminLoginRequest request)
    {
        var adminLoginId = _configuration["ADMIN_LOGIN_ID"];

        if (string.IsNullOrEmpty(adminLoginId) || request.LoginId != adminLoginId)
        {
            return Ok(new AdminLoginResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid login credentials" 
            });
        }

        var token = TokenGenerator.GenerateToken(32);
        _tokenService.StoreToken(token);

        return Ok(new AdminLoginResponse 
        { 
            Success = true, 
            AccessToken = token 
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

    private bool ValidateAdminToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        
        return _tokenService.ValidateToken(token);
    }

    #region User Management
    [HttpGet("users")]
    public async Task<ActionResult<AdminUserListResponse>> GetUsers()
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminUserListResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var users = await _userManagementService.GetAllUsersAsync();
        return Ok(new AdminUserListResponse { Success = true, Users = users });
    }

    [HttpPost("users")]
    public async Task<ActionResult<AdminUserResponse>> AddUser([FromBody] AdminAddUserRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "Username cannot be empty" 
            });
        }

        await _userManagementService.AddUserAsync(request.UserName);
        return Ok(new AdminUserResponse { Success = true });
    }

    [HttpPut("users")]
    public async Task<ActionResult<AdminUserResponse>> UpdateUser([FromBody] AdminEditUserRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.NewUserName))
        {
            return BadRequest(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "Username cannot be empty" 
            });
        }

        var success = await _userManagementService.UpdateUserNameAsync(request.UserName, request.NewUserName);
        
        return Ok(new AdminUserResponse 
        { 
            Success = success,
            ErrorMessage = success ? null : "Failed to update user - username may already exist"
        });
    }

    [HttpPut("users/reset")]
    public async Task<ActionResult<AdminUserResponse>> ResetUser([FromBody] AdminUserResetRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var user = await _userManagementService.GetUserByNameAsync(request.UserName);
        if (user == null)
        {
            return NotFound(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "User not found" 
            });
        }

        var success = await _userManagementService.ResetUserAsync(request.UserName);
        return Ok(new AdminUserResponse { Success = success });
    }

    [HttpDelete("users")]
    public async Task<ActionResult<AdminUserResponse>> DeleteUser([FromBody] AdminDeleteUserRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminUserResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var success = await _userManagementService.DeleteUserAsync(request.UserName);
        
        return Ok(new AdminUserResponse 
        { 
            Success = success,
            ErrorMessage = success ? null : "User not found"
        });
    }
    #endregion
    
    #region Question Set Management
    [HttpGet("questionsets")]
    public async Task<ActionResult<AdminListQuestionSetsResponse>> GetQuestionSets()
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminListQuestionSetsResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var questionSets = await _questionManagementService.GetAllQuestionSetsAsync();
        return Ok(new AdminListQuestionSetsResponse { Success = true, QuestionSets = questionSets });
    }

    [HttpPost("questionsets")]
    public async Task<ActionResult<AdminQuestionSetResponse>> CreateQuestionSet([FromBody] AdminCreateQuestionSetRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.QuestionSetName))
        {
            return BadRequest(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Question set name cannot be empty" 
            });
        }

        var questionSet = await _questionManagementService.CreateQuestionSetAsync(request.QuestionSetName);
        return Ok(new AdminQuestionSetResponse { Success = true, QuestionSet = questionSet });
    }

    [HttpPut("questionsets/order")]
    public async Task<ActionResult<AdminQuestionSetResponse>> UpdateQuestionSetOrder([FromBody] AdminUpdateOrderQuestionSetRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var success = await _questionManagementService.UpdateQuestionSetOrderAsync(request.QuestionSetId, request.QuestionSetOrder);
        if (!success)
        {
            return NotFound(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Question set not found" 
            });
        }

        var questionSet = await _questionManagementService.GetQuestionSetByIdAsync(request.QuestionSetId);
        return Ok(new AdminQuestionSetResponse { Success = true, QuestionSet = questionSet });
    }

    [HttpPut("questionsets/lock")]
    public async Task<ActionResult<AdminQuestionSetResponse>> UpdateQuestionSetLock([FromBody] AdminUpdateLockQuestionSetRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var success = await _questionManagementService.UpdateQuestionSetLockStatusAsync(request.QuestionSetId, request.Locked);
        if (!success)
        {
            return NotFound(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Question set not found" 
            });
        }

        var questionSet = await _questionManagementService.GetQuestionSetByIdAsync(request.QuestionSetId);
        return Ok(new AdminQuestionSetResponse { Success = true, QuestionSet = questionSet });
    }

    [HttpPut("questionsets/name")]
    public async Task<ActionResult<AdminQuestionSetResponse>> UpdateQuestionSetName([FromBody] AdminUpdateNameQuestionSetRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.NewQuestionSetName))
        {
            return BadRequest(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Question set name cannot be empty" 
            });
        }

        var success = await _questionManagementService.UpdateQuestionSetNameAsync(request.QuestionSetId, request.NewQuestionSetName);
        if (!success)
        {
            return NotFound(new AdminQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Question set not found" 
            });
        }

        var questionSet = await _questionManagementService.GetQuestionSetByIdAsync(request.QuestionSetId);
        return Ok(new AdminQuestionSetResponse { Success = true, QuestionSet = questionSet });
    }

    [HttpDelete("questionsets")]
    public async Task<ActionResult<AdminDeleteQuestionSetResponse>> DeleteQuestionSet([FromBody] AdminDeleteQuestionSetRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminDeleteQuestionSetResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var success = await _questionManagementService.DeleteQuestionSetAsync(request.QuestionSetId);
        return Ok(new AdminDeleteQuestionSetResponse { Success = success });
    }
    #endregion
    
    #region Question Management
    [HttpGet("questions")]
    public async Task<ActionResult<AdminListQuestionsResponse>> GetQuestions([FromQuery] string questionSetId)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminListQuestionsResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var questions = await _questionManagementService.GetQuestionsInSetAsync(questionSetId);
        return Ok(new AdminListQuestionsResponse { Success = true, Questions = questions });
    }
    
    [HttpPost("questions")]
    public async Task<ActionResult<AdminQuestionResponse>> CreateQuestion([FromBody] AdminCreateQuestionRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminQuestionResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return BadRequest(new AdminQuestionResponse 
            { 
                Success = false, 
                ErrorMessage = "Question text cannot be empty" 
            });
        }

        var question = await _questionManagementService.CreateQuestionAsync(
            request.QuestionSetId, 
            request.QuestionText, 
            request.ExpectedResultText, 
            request.QuestionOrder, 
            request.MinWordLength, 
            request.MaxWordLength);

        return Ok(new AdminQuestionResponse { Success = true, QuestionModels = question });
    }

    [HttpPut("questions")]
    public async Task<ActionResult<AdminQuestionResponse>> UpdateQuestion([FromBody] AdminUpdateQuestionRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminQuestionResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return BadRequest(new AdminQuestionResponse 
            { 
                Success = false, 
                ErrorMessage = "Question text cannot be empty" 
            });
        }

        var question = await _questionManagementService.UpdateQuestionAsync(
            request.QuestionId, 
            request.QuestionText, 
            request.ExpectedResultText, 
            request.QuestionOrder, 
            request.MinWordLength, 
            request.MaxWordLength);

        return Ok(new AdminQuestionResponse { Success = true, QuestionModels = question });
    }

    [HttpDelete("questions")]
    public async Task<ActionResult<AdminDeleteQuestionResponse>> DeleteQuestion([FromBody] AdminDeleteQuestionRequest request)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminDeleteQuestionResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var success = await _questionManagementService.DeleteQuestionAsync(request.QuestionId);
        return Ok(new AdminDeleteQuestionResponse { Success = success });
    }
    #endregion
    
    #region Response Management
    [HttpGet("responses")]
    public async Task<ActionResult<AdminGetResponsesResponse>> GetResponses([FromQuery] string questionId)
    {
        string? token = GetBearerToken();
        if (!ValidateAdminToken(token))
        {
            return Unauthorized(new AdminGetResponsesResponse 
            { 
                Success = false, 
                ErrorMessage = "Invalid access token" 
            });
        }

        var responses = await _questionManagementService.GetResponsesForQuestionAsync(questionId);
        return Ok(new AdminGetResponsesResponse { Success = true, Responses = responses });
    }
    #endregion
}