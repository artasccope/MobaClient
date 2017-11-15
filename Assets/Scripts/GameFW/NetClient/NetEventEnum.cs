using GameFW.Core;


namespace GameFW.NetClient
{
    /// <summary>
    /// 网络对时消息
    /// </summary>
    public enum NetEventTime{
        CheckTimeRequest = MsgType.Net,
        DelayGot,

        MaxValue,
    }

    /// <summary>
    /// 登录消息
    /// </summary>
    public enum NetEventLogin
    {
        LoginRequest = NetEventTime.MaxValue,
        LoginRes,

        MaxValue,
    }

    /// <summary>
    /// 注册消息
    /// </summary>
    public enum NetEventRegister {
        RegisterRequest = NetEventLogin.MaxValue,
        RegisterRes,

        MaxValue,
    }

    /// <summary>
    /// 用户消息
    /// </summary>
    public enum NetEventUser {
        HasNoUser = NetEventRegister.MaxValue,
        CreateUser,
        RequestUserInfo,
        GetUserInfo,
        CreateUserSuccess,
        CreateUserFailed,
        RequestOnline,
        OnlineSuccess,
        OnlineFailed,

        MaxValue,
    }

    /// <summary>
    /// 匹配消息
    /// </summary>
    public enum NetEventMatch {
        StartMatch = NetEventUser.MaxValue,
        StartMatchSuccess,
        CancelMatch,
        CancelMatchSuccess,

        MaxValue,
    }

    /// <summary>
    /// 选择消息
    /// </summary>
    public enum NetEventSelect {
        EnterRequest = NetEventMatch.MaxValue,
        EnterSres,
        SelectRequest,
        ReadyRequest,
        SomeOneEntered,
        SomeOneSelected,
        SomeOneReady,
        FightStart,

        MaxValue,
    }
}
