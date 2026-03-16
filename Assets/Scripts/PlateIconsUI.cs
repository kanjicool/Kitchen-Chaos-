using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{

    [SerializeField] private PlateKitchenObject plateKitchenObject; // Reference to the plate logic [2]
    [SerializeField] private Transform iconTemplate; // The UI element to duplicate [4]

    private void Awake()
    {
        // Hide the template so it doesn't show up in the UI by default [5, 6]
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Listen for when an ingredient is added to the plate [2]
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        // Update the icons whenever the plate contents change [4]
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // 1. Clean up old icons from previous updates [5, 7]
        foreach (Transform child in transform)
        {
            if (child == iconTemplate) continue; // Don't destroy the template! [5]
            Destroy(child.gameObject);
        }

        // 2. Cycle through all ingredients currently on the plate [4]
        foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
        {
            // 3. Instantiate the template for each ingredient [8]
            Transform iconTransform = Instantiate(iconTemplate, transform);

            // 4. Enable the duplicated icon and set its specific sprite [5]
            iconTransform.gameObject.SetActive(true);

            // This calls a helper script on the template to set the Sprite [9]
            if (iconTransform.TryGetComponent(out PlateIconSingleUI iconSingleUI))
            {
                iconSingleUI.SetKitchenObjectSO(kitchenObjectSO);
            }
            else
            {
                Debug.LogWarning("PlateIconSingleUI component missing on IconTemplate!", this);
            }
        }
    }
}