using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;
    [SerializeField] private Transform iconItemTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    public void UpdateVisual(List<RecipeSO> waittingRecipeListSO) {
        foreach (Transform child in container) {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (RecipeSO recipe in waittingRecipeListSO) {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            Text textTitle = recipeTransform.Find("RecipeTitle").GetComponent<Text>();
            textTitle.text = recipe.recipeName;
            foreach (Transform child in recipeTransform.Find("IconContainer"))
            {
                if (child == iconItemTemplate) continue;
                Destroy(child.gameObject);
            }
           
            foreach (KitchenObjectSO kitchenitem in recipe.kitchenObjectSOList)
            {
                Transform iconItem = Instantiate(iconItemTemplate, recipeTransform.Find("IconContainer"));
                iconItem.GetComponent<Image>().sprite = kitchenitem.sprite;
                iconItem.gameObject.SetActive(true);
            }
            recipeTransform.gameObject.SetActive(true);
        }
    }
}
