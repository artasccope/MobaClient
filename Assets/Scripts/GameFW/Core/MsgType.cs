using System;
using System.Collections.Generic;

namespace GameFW.Core
{
    /// <summary>
    /// 消息所属范围的类型
    /// </summary>
    public enum MsgType {
        UI = 0,
        Game = Settings.MsgSpan,
        Asset = Settings.MsgSpan*2,
        AI = Settings.MsgSpan*3,
        Entity = Settings.MsgSpan*4,
        Player = Settings.MsgSpan*5,
        Net = Settings.MsgSpan*6,
        Audio = Settings.MsgSpan*7,
        Input = Settings.MsgSpan*8,
    }
}
