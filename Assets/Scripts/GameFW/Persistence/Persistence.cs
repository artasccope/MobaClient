using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameFW.Persistence
{
    /// <summary>
    /// 持久化类
    /// </summary>
    public class Persistence:IPersistence
    {
        #region 单例
        public static Persistence Instance {
            get {
                return Nested.m_pInstance;
            }
        }

        private class Nested {
            internal static readonly Persistence m_pInstance = new Persistence();
            Nested() { }           
        }

        private Persistence() { }
        #endregion

        public void SaveInt(string name, int value) {
            
        }

        public void SaveFloat(string name, float value)
        {

        }

        public void SaveString(string name, string value)
        {

        }

        public void SaveBoolean(string name, bool value)
        {

        }

        public int LoadInt(string name)
        {
            throw new NotImplementedException();
        }

        public float LoadFloat(string name)
        {
            throw new NotImplementedException();
        }

        public string LoadString(string name)
        {
            throw new NotImplementedException();
        }

        public bool LoadBoolean(string name)
        {
            throw new NotImplementedException();
        }
    }
}
