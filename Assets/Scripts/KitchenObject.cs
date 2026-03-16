using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] KitchenObjectSO kitchenobject;

    protected virtual void Awake()
    {
        // Base Awake logic if any
    }

    public string GetKitchenObjectname() {
        return kitchenobject.objectname;
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenobject;
    }
}
