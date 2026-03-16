using UnityEngine;

public class PlateCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;
    private Transform PlateObject;

    private void Start()
    {
        PlateObject = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
        PlateObject.localPosition = Vector3.zero;
    }

    public void Interact(Player player)
    {
        if (PlateObject != null)
        {
            // Only allow picking up a plate if the player's hands are empty
            if (player.HasKitchenObject().Length == 0)
            {
                // Player picks up the plate
                PlateObject.transform.SetParent(player.transform);
                PlateObject.transform.parent = player.GetKitchenObjectFollowTransform();
                PlateObject.transform.localPosition = new Vector3(0f, -0.1f, 0.2f);

                // Create a replacement Plate on the counter
                Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
                kitchenObjectTransform.localPosition = Vector3.zero;
                PlateObject = kitchenObjectTransform;
            }
            else
            {
                Debug.Log("Hands not empty! Cannot pick up plate.");
            }
        }
    }

}
