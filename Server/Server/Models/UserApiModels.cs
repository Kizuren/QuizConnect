namespace WritingServer.Models;

#region User Login
public class UserLoginRequest
{
    public string Pin  { get; set; } = string.Empty;
}

public class UserLoginResponse
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class UserLogoutRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class UserLogoutResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
#endregion

#region User States
public class UserGetUsernameRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class UserGetUsernameResponse
{
    public bool Success { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class UserGetResetState
{
    public string AccessToken { get; set; } = string.Empty;
}

public class UserGetStateResponse
{
    public bool Success { get; set; }
    public bool ResetState { get; set; }
    public string? ErrorMessage { get; set; }
}
#endregion

#region Questions
public class UserGetQuestionSetsRequest
{
    public string AccessToken { get; set; } = string.Empty;
}

public class UserGetQuestionSetsResponse
{
    public bool Success { get; set; }
    public QuestionSetList QuestionSets { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class UserGetQuestionsRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionSetId { get; set; } = string.Empty;
}

public class UserGetQuestionsResponse
{
    public bool Success { get; set; }
    public QuestionModelList Questions { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class UserSubmitResponseRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string ResponseText { get; set; } = string.Empty;
}

public class UserSubmitResponseResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class UserGetResponsesRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
}

public class UserGetResponsesResponse
{
    public bool Success { get; set; }
    public ResponseList Responses { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
#endregion