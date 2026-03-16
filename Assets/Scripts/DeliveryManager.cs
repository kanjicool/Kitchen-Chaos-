using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] private DeliveryUI deliveryUI;
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 3;

    private void Awake()
    {
        waitRecipeSOList = new List<RecipeSO>();    
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log(waitingRecipeSO.recipeName);
                waitRecipeSOList.Add(waitingRecipeSO);
                deliveryUI.UpdateVisual(waitRecipeSOList);
            }
                
        }
    }

    public bool DeliveryRecipe(List<KitchenObjectSO> plateKitchenObjectSOList) {
        Debug.Log("--- Delivery Attempt ---");
        Debug.Log("Plate contains: " + string.Join(", ", plateKitchenObjectSOList.Select(so => so.objectname)));

        for (int i = 0; i < waitRecipeSOList.Count; ++i)
        {
            RecipeSO waitingRecipeSO = waitRecipeSOList[i];
            Debug.Log($"Checking Order #{i}: {waitingRecipeSO.recipeName}");
            Debug.Log($"  Requirements: {string.Join(", ", waitingRecipeSO.kitchenObjectSOList.Select(so => so.objectname))}");

            // 1. Check if the number of ingredients matches exactly
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObjectSOList.Count)
            {
                // 2. Use a copy of the plate ingredients name list to track matches
                List<string> plateIngredientNamesCopy = plateKitchenObjectSOList.Select(so => so.objectname).ToList();
                bool ingredientsMatch = true;

                foreach (KitchenObjectSO recipeIngredientSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    string recipeIngredientName = recipeIngredientSO.objectname;

                    if (plateIngredientNamesCopy.Contains(recipeIngredientName))
                    {
                        // Match found by name! Remove it from copy so it's not reused
                        plateIngredientNamesCopy.Remove(recipeIngredientName);
                    }
                    else
                    {
                        // Ingredient name NOT found on plate
                        Debug.Log($"  FAILED: Missing {recipeIngredientName} on plate.");
                        ingredientsMatch = false;
                        break;
                    }
                }

                if (ingredientsMatch)
                {
                    // Everything matched by name! Proceed with delivery
                    Debug.Log("  MATCH FOUND! Delivering...");
                    waitRecipeSOList.RemoveAt(i);
                    deliveryUI.UpdateVisual(waitRecipeSOList);
                    return true;
                }
            }
            else
            {
                Debug.Log($"  FAILED: Count mismatch. Recipe needs {waitingRecipeSO.kitchenObjectSOList.Count}, Plate has {plateKitchenObjectSOList.Count}");
            }
        }
        
        Debug.Log("No matching recipes found for this plate.");
        return false;
    }
}
