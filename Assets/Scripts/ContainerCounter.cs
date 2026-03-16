using Unity.Collections;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;

public class ContainerCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;
    private KitchenObject kitchenObject;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Interact(Player player)
    {

        if (kitchenObject == null)
        {
            animator.SetTrigger("OpenClose");
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
            kitchenObjectTransform.localPosition = Vector3.zero;
            kitchenObject = kitchenObjectTransform.transform.GetComponent<KitchenObject>();
        }
        else
        {
            string[] listKitchenObject = player.HasKitchenObject();
            if (listKitchenObject.Length == 0)
            {
                kitchenObject.transform.SetParent(player.transform);
                kitchenObject.transform.parent = player.GetKitchenObjectFollowTransform();
                kitchenObject.transform.localPosition = Vector3.zero;
                ClearKitchenObject();
            }
            else if (player.HasPlate())
            {
                // Player is holding a Plate, try to add ingredient directly
                PlateKitchenObject plate = player.GetComponentInChildren<PlateKitchenObject>();
                if (plate.TryAddIngredient(kitchenObject.GetKitchenObjectSO()))
                {
                    Destroy(kitchenObject.gameObject);
                    ClearKitchenObject();
                }
            }
        }
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
}
