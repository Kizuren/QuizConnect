namespace WritingServer.Models;

#region Admin Login
public class AdminLoginRequest
{
    public string LoginId { get; set; } = string.Empty;
}

public class AdminLoginResponse
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class AdminLogoutRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class AdminLogoutResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
#endregion

#region User Administration
public class AdminAddUserRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class AdminEditUserRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string NewUserName { get; set; } = string.Empty;
}

public class AdminDeleteUserRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class AdminUserResetRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class AdminUserResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AdminUserListRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class AdminUserListResponse
{
    public bool Success { get; set; }
    public UserModelList Users { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
#endregion

#region Question Set Management
public class AdminCreateQuestionSetRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetName { get; set; } = string.Empty;
}

public class AdminUpdateOrderQuestionSetRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
    public int QuestionSetOrder { get; set; }
}

public class AdminUpdateLockQuestionSetRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
    public bool Locked { get; set; }
}

public class AdminUpdateNameQuestionSetRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
    public string NewQuestionSetName { get; set; } = string.Empty;
}

public class AdminDeleteQuestionSetRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
}

public class AdminQuestionSetResponse
{
    public bool Success { get; set; }
    public QuestionSet QuestionSet { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class AdminDeleteQuestionSetResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AdminListQuestionSetsRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class AdminListQuestionSetsResponse
{
    public bool Success { get; set; }
    public QuestionSetList QuestionSets { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
#endregion

#region Question Management
public class AdminCreateQuestionRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string ExpectedResultText { get; set; } = string.Empty;
    public int QuestionOrder { get; set; }
    public int MinWordLength { get; set; }
    public int MaxWordLength { get; set; }
}

public class AdminUpdateQuestionRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string ExpectedResultText { get; set; } = string.Empty;
    public int QuestionOrder { get; set; }
    public int MinWordLength { get; set; }
    public int MaxWordLength { get; set; }
}

public class AdminDeleteQuestionRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
}

public class AdminQuestionResponse
{
    public bool Success { get; set; }
    public QuestionModels QuestionModels { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class AdminDeleteQuestionResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AdminListQuestionsRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
}

public class AdminListQuestionsResponse
{
    public bool Success { get; set; }
    public QuestionModelList Questions { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
#endregion

#region Reponses
public class AdminGetResponsesRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
}

public class AdminGetResponsesResponse
{
    public bool Success { get; set; }
    public ResponseList Responses { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
#endregion