using System;


namespace GameFW.GameMgr
{
    public class FlowMgr:IGameMgr
    {
        #region 单例
        public static FlowMgr Instance
        {
            get
            {
                return Nested.m_pInstance;
            }
        }

        private class Nested
        {
            internal static readonly FlowMgr m_pInstance = new FlowMgr();
            Nested() { }
        }

        private FlowMgr() { }
        #endregion

        #region 流程控制方法
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}
