using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFW.Asset.Mgr.Basic
{
    /// <summary>
    /// 卸载包
    /// </summary>
    public interface IUnload
    {
        void UnloadBundle(bool ifWithObj);
    }
}
