using Unity.Collections;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private Transform counterTopPoint;


    public void Interact(Player player)
    {
        string[] listKitchenObject = player.HasKitchenObject();

        // CASE 1: Player has a Plate and is picking up an ingredient from Counter
        if (player.HasPlate() && this.HasKitchenObject() && !this.GetComponentInChildren<PlateKitchenObject>())
        {
            PlateKitchenObject plate = player.GetComponentInChildren<PlateKitchenObject>();
            KitchenObject ingredient = this.GetComponentInChildren<KitchenObject>();
            if (plate.TryAddIngredient(ingredient.GetKitchenObjectSO()))
            {
                Destroy(ingredient.gameObject);
            }
            return; // STOP! Don't let it fall through to stacking logic
        }

        // CASE 2: Player has an ingredient and is putting it on a Plate on the Counter
        if (!player.HasPlate() && listKitchenObject.Length == 1 && listKitchenObject[0] != "Plate" && this.HasKitchenObject() && this.GetComponentInChildren<PlateKitchenObject>())
        {
            PlateKitchenObject plate = this.GetComponentInChildren<PlateKitchenObject>();
            KitchenObject ingredient = player.GetComponentInChildren<KitchenObject>();
            if (plate.TryAddIngredient(ingredient.GetKitchenObjectSO()))
            {
                Destroy(ingredient.gameObject);
            }
            return; // STOP! Don't let it fall through to stacking logic
        }

        // FALLBACK: Original logic for placing/picking up items
        if (listKitchenObject.Length > 0 && !this.HasKitchenObject())
        {
            Debug.Log("Place Item!");
            KitchenObject[] playerKitchenObject = player.GetComponentsInChildren<KitchenObject>();
            if (playerKitchenObject.Length > 0)
            {
                float order_object = 0.1f;
                foreach (KitchenObject obj in playerKitchenObject)
                {
                    obj.transform.SetParent(this.transform);
                    obj.transform.parent = counterTopPoint;
                    if (obj.GetKitchenObjectname() == "Plate")
                        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
                    else
                        obj.transform.localPosition = new Vector3(0f, order_object, 0f);
                    order_object += 0.1f;
                }
            }
        }
        else
        {
            KitchenObject[] kitchenObject = this.GetComponentsInChildren<KitchenObject>();
            Debug.Log("Pick up!");
            if (kitchenObject.Length > 0 && listKitchenObject.Length == 0)
            {
                float order_object = 0.1f;
                foreach (KitchenObject obj in kitchenObject)
                {
                    obj.transform.SetParent(player.transform);
                    obj.transform.parent = player.GetKitchenObjectFollowTransform();
                    if (obj.GetKitchenObjectname() == "Plate")
                        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
                    else
                        obj.transform.localPosition = new Vector3(0f, order_object, 0f);
                    order_object += 0.1f;
                }
            }
        }
    }
    public Transform GetKitchenObjectFollowTransform() 
    {
        return counterTopPoint;
    }
    public bool HasKitchenObject()
    {
        KitchenObject playerKitchenObject = this.GetComponentInChildren<KitchenObject>();
        return playerKitchenObject != null;
    }

}
