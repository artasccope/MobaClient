using System;
using System.Collections.Generic;
using Protocol;
using Protocol.DTO;
using GameFW.GameMgr;
using GameFW.Core.Msg;
using GameFW.Core.Base;

namespace GameFW.NetClient.Time
{
    /// <summary>
    /// 对时处理器
    /// </summary>
    public class TimeHandler : NetBase, IHandler
    {
        #region 应用内消息处理
        protected override void Awake()
        {
            base.Awake();
            Regist();
            ushort[] newMsgIds = new ushort[1] {
                (ushort)NetEventTime.CheckTimeRequest,
            };
            AddNewMsgIds(newMsgIds);
            RegistSelf();

            CheckDelayRequest(3);
            GameRuntimeData.FloatingBwteenDatatimeAndUnityTime = DateTime.Now.Ticks * 0.0000001f - UnityEngine.Time.time;
        }
        /// <summary>
        /// 处理应用内消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventTime.CheckTimeRequest:
                    MsgInt msgInt = msg as MsgInt;
                    DelayCheckDTO delayCheckDTO = new DelayCheckDTO(msgInt.Int);//check多少次
                    delayCheckDTO.timeStamps.Add(DateTime.Now.Ticks);
                    Send(TimeProtocol.CHECK_CREQ, delayCheckDTO);
                    break;
            }
        }

        #endregion

        #region client申请对时

        /// <summary>
        /// 向服务器发送对时请求
        /// </summary>
        /// <param name="time"></param>
        private void CheckDelayRequest(int time)
        {
            DelayCheckDTO delayCheckDTO = new DelayCheckDTO(time);
            delayCheckDTO.timeStamps.Add(DateTime.Now.Ticks);
            Send(TimeProtocol.CHECK_CREQ, delayCheckDTO);
        }

        #endregion

        #region 网络消息处理

        /// <summary>
        /// 对时消息处理，分为server请求的和client请求的
        /// </summary>
        /// <param name="sm"></param>
        public override void OnMessageReceived(SocketModel sm)
        {
            switch (sm.command)
            {
                case TimeProtocol.CHECK_SRES:
                    DelayCheckDTO delayCheckDTO = sm.GetMessage<DelayCheckDTO>();
                    if (delayCheckDTO.timeStamps.Count <= delayCheckDTO.checkNum * 2)
                    {//对时次数不够，就继续对够
                        delayCheckDTO.timeStamps.Add(DateTime.Now.Ticks);
                        Send(TimeProtocol.CHECK_CREQ, delayCheckDTO);
                    }

                    if (delayCheckDTO.timeStamps.Count == delayCheckDTO.checkNum * 2 + 1)//这里的最后一个timestamp是client的
                    {//对时次数够了，计算时差和延迟
                        GameRuntimeData.delayAndFloating = GetDelayAndFloatingOdd(delayCheckDTO.timeStamps);
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgDelayAndFloating((ushort)NetEventTime.DelayGot, GameRuntimeData.delayAndFloating));
                    }
                    break;
                case TimeProtocol.CHECK_SREQ:
                    DelayCheckDTO delayCheck = sm.GetMessage<DelayCheckDTO>();
                    if (delayCheck.timeStamps.Count < delayCheck.checkNum * 2)
                    {
                        delayCheck.timeStamps.Add(DateTime.Now.Ticks);
                        Send(TimeProtocol.CHECK_CRES, delayCheck);
                    }
                    else
                    {//对时次数够了，计算时差和延迟,这里注意最后一个时间戳是server发来的，因此需要再把现在的时间加上再计算
                        delayCheck.timeStamps.Add(DateTime.Now.Ticks);
                        GameRuntimeData.delayAndFloating = GetDelayAndFloatingEven(delayCheck.timeStamps);
                        MgrCenter.Instance.SendMsg(Msgs.GetMsgDelayAndFloating((ushort)NetEventTime.DelayGot, GameRuntimeData.delayAndFloating));
                    }
                    break;
            }
        }

        /// <summary>
        /// 网络消息类型
        /// </summary>
        /// <returns></returns>
        public override byte GetType()
        {
            return Protocol.Protocol.TYPE_TIME;
        }

        #endregion

        #region 时差和延迟计算

        /// <summary>
        /// 对时包总数为奇数的情况
        /// </summary>
        /// <param name="timeStamps"></param>
        /// <returns></returns>
        private DelayAndFloating GetDelayAndFloatingOdd(List<long> timeStamps)
        {
            long delay = 0;
            long floating = 0;
            for (int i = timeStamps.Count - 1; i >= 2; i -= 2)
            {
                delay += (timeStamps[i] - timeStamps[i - 2]);
                floating += (timeStamps[i] - timeStamps[i - 1]);//client - server
            }
            delay /= ((timeStamps.Count - 1) / 2);
            floating = floating / (timeStamps.Count / 2) - delay;//client - server
            DelayAndFloating delayAndFloating = new DelayAndFloating(delay, floating);

            return delayAndFloating;
        }

        /// <summary>
        /// 对时包总数为偶数的情况
        /// </summary>
        /// <param name="timeStamps"></param>
        /// <returns></returns>
        private DelayAndFloating GetDelayAndFloatingEven(List<long> timeStamps)
        {
            long delay = 0;
            long floating = 0;
            for (int i = timeStamps.Count - 1; i >= 2; i -= 2)
            {
                delay += (timeStamps[i] - timeStamps[i - 2]);
            }
            for (int j = timeStamps.Count - 1; j >= 1; j -= 2)
            {
                floating += (timeStamps[j] - timeStamps[j - 1]);//client-server
            }

            delay /= ((timeStamps.Count - 1) / 2);
            floating = floating / (timeStamps.Count / 2) - delay;
            DelayAndFloating delayAndFloating = new DelayAndFloating(delay, floating);

            return delayAndFloating;
        }

        #endregion
    }
}