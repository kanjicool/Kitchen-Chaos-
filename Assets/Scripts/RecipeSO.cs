using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
}
