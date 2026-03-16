using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StoveCounter : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Transform counterTopPoint;

    [Header("0:MeatCooked, 1:MeatBurnt, 2:FriedEgg, 3:BurntFriedEgg")]
    [SerializeField] private KitchenObjectSO[] meatObjectOS;

    private float cuttingProcess;
    public float fryingSpeed = 1f;
    public float fryingMax = 100f;
    public float burningMax = 200f;
    private float fryingProcess;
    private int isCook = 0; // 0 = กำลังทำให้สุก, 1 = สุกแล้วกำลังจะไหม้, 2 = ไหม้แล้ว
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
            if (kitchenObject == null) return;

            fryingProcess += fryingSpeed * Time.deltaTime;
            FryingBar processBar = this.GetComponentInChildren<FryingBar>();

            float currentMax = (isCook == 0) ? fryingMax : burningMax;
            float persent_process = (float)(fryingProcess) / currentMax;

            if (processBar != null)
            {
                processBar.FryingCounter_OnProcessChanged(persent_process);
            }

            // เมื่อถึงเวลา (สุก หรือ ไหม้)
            if (fryingProcess >= currentMax)
            {
                string currentObjectName = kitchenObject.GetKitchenObjectname();
                Destroy(kitchenObject.gameObject);

                int nextPrefabIndex = 0;

                // ตัดช่องว่างและทำเป็นตัวพิมพ์เล็ก เพื่อป้องกันปัญหาตั้งชื่อผิดนิดหน่อยแล้วหาไม่เจอ
                string checkName = currentObjectName.ToLower().Replace(" ", "");

                // กรณีที่ 1: จาก ดิบ -> สุก
                if (isCook == 0)
                {
                    if (checkName.Contains("meat")) nextPrefabIndex = 0;       // ดึงเนื้อสุก
                    else if (checkName.Contains("egg")) nextPrefabIndex = 2;   // ดึงไข่ดาว
                    else Debug.LogWarning("ไม่พบชื่อของดิบที่ตรงเงื่อนไข: " + currentObjectName);

                    Transform sliceTransform = Instantiate(meatObjectOS[nextPrefabIndex].prefab, counterTopPoint);
                    sliceTransform.transform.localPosition = Vector3.zero;

                    if (processBar != null) processBar.FryingCounter_OnProcessChanged(0f);

                    isCook = 1; // เปลี่ยนเป็นสถานะสุกแล้ว (เริ่มนับเวลาไหม้)
                    fryingProcess = 0.001f; // เริ่มต้นจับเวลาไหม้

                    if (animator != null) animator.SetBool("IsFlashing", true);
                    Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
                    if (warningUI != null) warningUI.gameObject.SetActive(true);
                }
                // กรณีที่ 2: จาก สุก -> ไหม้
                else if (isCook == 1)
                {
                    if (checkName.Contains("meat")) nextPrefabIndex = 1;       // ดึงเนื้อไหม้
                    else if (checkName.Contains("egg")) nextPrefabIndex = 3;   // ดึงไข่ไหม้
                    else
                    {
                        Debug.LogWarning("หาชื่อ '" + currentObjectName + "' ไม่เจอ! เลยดึงเนื้อไหม้มาแทน");
                        nextPrefabIndex = 1; // กันพลาด
                    }

                    Transform burntTransform = Instantiate(meatObjectOS[nextPrefabIndex].prefab, counterTopPoint);
                    burntTransform.transform.localPosition = Vector3.zero;

                    if (processBar != null) processBar.FryingCounter_OnProcessChanged(0f);
                    fryingProcess = 0f;
                    isCook = 2; // เปลี่ยนเป็นสถานะไหม้แล้ว

                    if (animator != null) animator.SetBool("IsFlashing", false);
                    Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
                    if (warningUI != null) warningUI.gameObject.SetActive(false);
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
                ResetStove(); // เรียกใช้ฟังก์ชันรีเซ็ตเตา
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

        // CASE 3: วางของดิบลงบนเตาที่ว่างอยู่
        if (listKitchenObject.Length == 1 && !listKitchenObject.Contains("Plate") && !this.HasKitchenObject())
        {
            KitchenObject playerKitchenObject = player.GetComponentInChildren<KitchenObject>();
            string objName = playerKitchenObject.GetKitchenObjectname();
            string checkInteractName = objName.ToLower().Replace(" ", "");

            // เช็คว่าของที่ถืออยู่คือเนื้อ หรือ ไข่ (ใช้ Contains ป้องกันปัญหาวรรค/พิมพ์เล็กใหญ่)
            if (checkInteractName.Contains("meat") || checkInteractName.Contains("egg"))
            {
                Debug.Log("Frying: " + objName);
                isCook = 0;

                if (stoveCounter != null) stoveCounter.gameObject.SetActive(true);

                playerKitchenObject.transform.SetParent(this.transform);
                playerKitchenObject.transform.parent = counterTopPoint;
                playerKitchenObject.transform.localPosition = Vector3.zero;

                fryingProcess = 0.001f; // ใส่ค่ามากกว่า 0 เพื่อให้ Update ทำงานทันที
                FryingBar processBar = this.GetComponentInChildren<FryingBar>();
                if (processBar != null)
                {
                    float persent_process = (float)fryingProcess / fryingMax;
                    processBar.FryingCounter_OnProcessChanged(persent_process);
                }
            }
        }
        // CASE 4: หยิบของออกจากเตา
        else
        {
            if (this.HasKitchenObject() && listKitchenObject.Contains("Plate"))
            {
                KitchenObject kitchenObject = this.GetComponentInChildren<KitchenObject>();
                kitchenObject.transform.SetParent(player.transform);
                kitchenObject.transform.parent = player.GetKitchenObjectFollowTransform();
                kitchenObject.transform.localPosition = Vector3.zero;

                ResetStove(); // เรียกใช้ฟังก์ชันรีเซ็ตเตา
            }
        }
    }

    // สร้างฟังก์ชันแยกออกมาเพื่อให้โค้ดดูสะอาดขึ้นเมื่อต้องรีเซ็ตสถานะของเตา
    private void ResetStove()
    {
        fryingProcess = 0f;
        isCook = 0;

        FryingBar processBar = this.GetComponentInChildren<FryingBar>();
        if (processBar != null) processBar.FryingCounter_OnProcessChanged(0f);

        if (animator != null) animator.SetBool("IsFlashing", false);

        Transform warningUI = this.gameObject.transform.Find("StoveBurnWarningUI");
        if (warningUI != null) warningUI.gameObject.SetActive(false);
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