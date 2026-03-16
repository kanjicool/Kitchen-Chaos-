using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO from;
    public KitchenObjectSO to;
    public int cutCount;
}
