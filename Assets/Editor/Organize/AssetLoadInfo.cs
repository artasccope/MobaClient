

public class AssetLoadInfo
{
    public string prefabABName;
    public string organizeDataABName;
    public int initialOrder;
    public bool activeSelf;

    public AssetLoadInfo(string prefabABName, string organizeDataABName, int initialOrder, bool activeSelf)
    {
        this.prefabABName = prefabABName;
        this.organizeDataABName = organizeDataABName;
        this.initialOrder = initialOrder;
        this.activeSelf = activeSelf;
    }
}
