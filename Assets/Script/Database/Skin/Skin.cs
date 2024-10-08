[System.Serializable]
public class Skin
{
    public string skinId;
    public string skinName;
    public int price;
    public string assetPath;
    public bool isPurchased;

    public Skin(string skinId, string skinName, int price, string assetPath, bool isPurchased)
    {
        this.skinId = skinId;
        this.skinName = skinName;
        this.price = price;
        this.assetPath = assetPath;
        this.isPurchased = isPurchased;
    }
}
    