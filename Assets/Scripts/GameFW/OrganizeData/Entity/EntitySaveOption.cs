using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFW.OrganizeData.Entity
{
    /// <summary>
    /// 实体信息存储选项
    /// </summary>
    public class EntitySaveOption
    {
        #region 构造器
        public EntitySaveOption(int categoryIndex, int specieIndex, int teamIndex, bool ifSaveHierachy, bool ifSaveTrans, bool ifLoadRuntime, string categoryName, string name, string assetName)
        {
            this.categoryIndex = specieIndex;
            this.teamIndex = teamIndex;
            this.ifSaveHierachy = ifSaveHierachy;
            this.ifSaveTrans = ifSaveTrans;
            this.ifLoadRuntime = ifLoadRuntime;
            this.categoryName = categoryName;
            this.name = name;
            this.assetName = assetName;
        }
        #endregion
        /// <summary>
        /// 类型
        /// </summary>
        public int categoryIndex;
        /// <summary>
        /// 种类
        /// </summary>
        public int specieIndex;
        /// <summary>
        /// 类型名字
        /// </summary>
        public string categoryName;
        /// <summary>
        /// 种类名字
        /// </summary>
        public string name;
        /// <summary>
        /// 对应的资源名
        /// </summary>
        public string assetName;
        /// <summary>
        /// 所属队伍
        /// </summary>
        public int teamIndex;
        /// <summary>
        /// 是否保存层次信息
        /// </summary>
        public bool ifSaveHierachy;
        /// <summary>
        /// 是否保存变换信息
        /// </summary>
        public bool ifSaveTrans;
        /// <summary>
        /// 是否是在游戏运行过程中加载(而不是场景一开始就加载)
        /// </summary>
        public bool ifLoadRuntime;
    }
}
