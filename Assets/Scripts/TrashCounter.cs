using UnityEngine;
using System.Linq;

public class TrashCounter : MonoBehaviour
{
    public void Interact(Player player)
    {
        string[] listKitchenObject = player.HasKitchenObject();
        if (listKitchenObject.Length > 0 )
        {
            Debug.Log("Destroy Item!");
            KitchenObject[] playerKitchenObject = player.GetComponentsInChildren<KitchenObject>();          
            if (playerKitchenObject.Length > 0)
            {
                foreach (KitchenObject obj in playerKitchenObject)
                {
                    Destroy(obj.gameObject);
                }

            }
        }
    }
}
