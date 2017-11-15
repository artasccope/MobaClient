
public class RuntimeAssetLoadInfo
{
    public int categoryId;
    public int specieId;
    public string prefabABName;
    public bool activeSelf;

    public RuntimeAssetLoadInfo() { }

    public RuntimeAssetLoadInfo(int categoryId, int specieId, string prefabABName, bool activeSelf) {
        this.categoryId = categoryId;
        this.specieId = specieId;
        this.prefabABName = prefabABName;
        this.activeSelf = activeSelf;
    }
}
