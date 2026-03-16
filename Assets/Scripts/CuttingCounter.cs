using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CuttingCounter : MonoBehaviour
{
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private float cuttingProcess;
    public float cuttingSpeed = 5f;
    public float knifeSpeed = 0.2f;
    private Animator animator;
    private float timer = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (cuttingProcess > 0)
        {
            KitchenObject kitchenObject = this.GetComponentInChildren<KitchenObject>();
            if (kitchenObject == null) return;

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObject.GetKitchenObjectname());

            if (cuttingRecipeSO != null)
            {
                cuttingProcess += cuttingSpeed * Time.deltaTime;
                Cutting_FX(Time.deltaTime);

                int cuttingMax = cuttingRecipeSO.cutCount;
                ProcessBar processBar = this.GetComponentInChildren<ProcessBar>();
                float persent_process = (float)cuttingProcess / cuttingMax;
                processBar.CuttingCounter_OnProcessChanged(persent_process);

                if (cuttingProcess >= cuttingMax)
                {
                    Destroy(kitchenObject.gameObject);
                    Transform sliceTransform = Instantiate(cuttingRecipeSO.to.prefab, counterTopPoint);
                    sliceTransform.transform.localPosition = Vector3.zero;
                    processBar.CuttingCounter_OnProcessChanged(0f);
                    cuttingProcess = 0;
                }
            }
            else
            {
                cuttingProcess = 0;
            }
        }
    }

    private void Cutting_FX(float duration)
    {
        timer += duration;
        if (timer >= knifeSpeed)
        {
            animator.SetTrigger("Cut");
            timer = 0f;
        }
    }

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
            return;
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
            return;
        }

        if (listKitchenObject.Length == 1 && !listKitchenObject.Contains("Plate") && !this.HasKitchenObject())
        {
            KitchenObject playerKitchenObject = player.GetComponentInChildren<KitchenObject>();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(playerKitchenObject.GetKitchenObjectname());

            if (cuttingRecipeSO != null)
            {
                cuttingProcess = 0;
                int cuttingMax = cuttingRecipeSO.cutCount;
                playerKitchenObject.transform.parent = counterTopPoint;
                playerKitchenObject.transform.localPosition = Vector3.zero;

                ProcessBar processBar = this.GetComponentInChildren<ProcessBar>();
                float persent_process = (float)cuttingProcess / cuttingMax;
                processBar.CuttingCounter_OnProcessChanged(persent_process);

                cuttingProcess++;
                animator.SetTrigger("Cut");
                timer = 0f;
            }
        }
        else
        {
            if (this.HasKitchenObject() && listKitchenObject.Contains("Plate"))
            {
                KitchenObject kitchenObject = this.GetComponentInChildren<KitchenObject>();
                kitchenObject.transform.SetParent(player.transform);
                kitchenObject.transform.parent = player.GetKitchenObjectFollowTransform();
                kitchenObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(string kitchenObjectName)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.from.objectname == kitchenObjectName)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }

    public void InterActAlter(Player player) 
    { 

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
