using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StoveCounterV0 : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private KitchenObjectSO[] meatObjectOS;

    private float cuttingProcess;
    public float fryingSpeed = 1f;
    public float fryingMax = 100f;
    public float burningMax = 200f;
    private float fryingProcess;
    private int isCook = 0;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (fryingProcess > 0)
        {
            KitchenObject kitchenObject = this.GetComponentInChildren<KitchenObject>();
            fryingProcess += fryingSpeed * Time.deltaTime;
            FryingBar processBar = this.GetComponentInChildren<FryingBar>();

            float currentMax = (isCook == 0) ? fryingMax : burningMax;
            float persent_process = (float)(fryingProcess) / currentMax;
            processBar.FryingCounter_OnProcessChanged(persent_process);

            if ((fryingProcess) >= currentMax)
            {
                Destroy(kitchenObject.gameObject);
                Transform sliceTransform = Instantiate(meatObjectOS[isCook].prefab, counterTopPoint);
                sliceTransform.transform.localPosition = Vector3.zero;
                processBar.FryingCounter_OnProcessChanged(0f);
                fryingProcess = 0f;
                if (isCook == 0)
                {
                    isCook = 1;
                    fryingProcess = 0.001f; // Started burning
                    animator.SetBool("IsFlashing", true);
                    Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
                    if (warningUI != null) warningUI.gameObject.SetActive(true);
                }
            }
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

                // Reset Stove
                fryingProcess = 0f;
                FryingBar processBar = this.GetComponentInChildren<FryingBar>();
                processBar.FryingCounter_OnProcessChanged(0f);
                animator.SetBool("IsFlashing", false);
                Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
                if (warningUI != null) warningUI.gameObject.SetActive(false);
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
            Debug.Log("Frying");
            KitchenObject playerKitchenObject = player.GetComponentInChildren<KitchenObject>();
            fryingProcess = 0;
            isCook = 0;
            if (playerKitchenObject.GetKitchenObjectname() == "Meat")
            {
                stoveCounter.gameObject.SetActive(true);
                playerKitchenObject.transform.SetParent(this.transform);
                playerKitchenObject.transform.parent = counterTopPoint;
                playerKitchenObject.transform.localPosition = Vector3.zero;

                FryingBar processBar = this.GetComponentInChildren<FryingBar>();
                float persent_process = (float)fryingProcess / fryingMax;
                processBar.FryingCounter_OnProcessChanged(persent_process);
                fryingProcess++;
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
                fryingProcess = 0f;
                FryingBar processBar = this.GetComponentInChildren<FryingBar>();
                processBar.FryingCounter_OnProcessChanged(0f);
                animator.SetBool("IsFlashing", false);
                Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
                warningUI.gameObject.SetActive(false);

            }
        }

    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
    public bool HasKitchenObject()
    {
        KitchenObject hasKitchenObject = this.GetComponentInChildren<KitchenObject>();
        return hasKitchenObject != null;
    }

}