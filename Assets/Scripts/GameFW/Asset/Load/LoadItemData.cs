namespace GameFW.Asset.Load
{
    /// <summary>
    /// 加载资源时使用的配置数据
    /// </summary>
    public class LoadItemData
    {
        public int initialOrder;//加载顺序
        public bool isActive;//加载时是否设为active
        public string prefabABName;//加载的资源对应的prefab和所在包的名字
        public string organizeDataABName;//资源对应的组织数据(Scriptable Object)的名字及所在包的名字
    }
}
