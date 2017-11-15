using GameFW.Core;

namespace GameFW.UI
{
    /// <summary>
    /// 输入UI消息
    /// </summary>
    public enum InputToUIMsgEnum
    {
        RegisterUIEvent = MsgType.UI + 1,

        MaxValue,
    }

    /// <summary>
    /// 选择界面消息
    /// </summary>
    public enum SelectUIEvent {
        HeroPressed = InputToUIMsgEnum.MaxValue,

        MaxValue,
    }
    
    /// <summary>
    /// HUD消息
    /// </summary>
    public enum HUDEvent {
        CreateHUD = SelectUIEvent.MaxValue,
        HideHUD,
        DestroyHUD,
        UpdateHUDPos,
        UpdateHUDHp,

        MaxValue,
    }
}
