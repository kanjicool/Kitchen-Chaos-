using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DeliveryCounter : MonoBehaviour
{
    [SerializeField] private DeliveryManager deliveryManager;

    public void Interact(Player player)
    {
        if (player.HasPlate())
        {
            PlateKitchenObject plateKitchenObject = player.GetComponentInChildren<PlateKitchenObject>();
            
            // Get the list of ingredients on the plate
            List<KitchenObjectSO> kitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

            // Try to deliver the recipe using the list of SOs directly
            if (deliveryManager.DeliveryRecipe(kitchenObjectSOList))
            {
                Debug.Log("Delivery Success!");

                KitchenObject[] allHeldObjects = player.GetComponentsInChildren<KitchenObject>();

                foreach (KitchenObject obj in allHeldObjects)
                {
                    Destroy(obj.gameObject);
                }

            }
            else
            {
                Debug.Log("Delivery Failed: Recipe does not match orders!");
            }
        }
    }
}
