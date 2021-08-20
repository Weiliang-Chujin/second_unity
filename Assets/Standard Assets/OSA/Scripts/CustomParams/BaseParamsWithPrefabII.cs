using Com.TheFallenGames.OSA.CustomParams;

[System.Serializable]
public class BaseParamsWithPrefabII : BaseParamsWithPrefab
{
    /// <summary>
    /// if true, then apply size from prefab property
    /// </summary>
    public bool ApplyItemSize = true;

    protected override void InitItemPrefab()
    {
        if (ApplyItemSize)
        {
            AssertValidWidthHeight(ItemPrefab);
            
            _ItemPrefabSize = -1f; // so the prefab's size will be recalculated next time is accessed
            DefaultItemSize = ItemPrefabSize;
        }
    }
}