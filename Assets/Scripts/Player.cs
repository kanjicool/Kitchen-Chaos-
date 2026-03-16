using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;   

public class Player : MonoBehaviour
{
    // --- ตัวแปรสำหรับปรับแต่งใน Inspector ---
    [SerializeField] private float movespeed = 5f;       // ความเร็วในการเคลื่อนที่
    [SerializeField] private float rotatespeed = 8f;     // ความเร็วในการหมุนหน้าตัวละคร
    [SerializeField] private Transform holdPoint;        // จุดตำแหน่งที่ใช้ถือของ (Kitchen Objects)

    // --- ตัวแปรภายในสำหรับระบบควบคุมและสถานะ ---
    private PlayerInputActions inputActions;             // อ้างอิงถึงระบบ Input Actions
    private Vector2 moveInput;                           // เก็บค่าแกนการเคลื่อนที่ (X, Y)
    private bool isWalking = false;                      // สถานะว่ากำลังเดินอยู่หรือไม่

    private void Awake()
    {
        // เริ่มต้นสร้าง Instance ของ Input Actions
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // เปิดใช้งาน Input และลงทะเบียนเหตุการณ์เมื่อมีการกดปุ่มเคลื่อนที่
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        // ยกเลิกการลงทะเบียนและปิดใช้งาน Input เมื่อไม่ได้ใช้
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Disable();
    }

    void Update()
    {
        // 1. คำนวณทิศทางการเคลื่อนที่ในแนวราบ (X, Z)
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // 2. ขยับตำแหน่งของผู้เล่นตามทิศทางและความเร็ว
        transform.position += moveDir * movespeed * Time.deltaTime;

        // 3. ปรับสถานะการเดิน (ถ้า moveDir ไม่เป็น 0 แสดงว่าเดินอยู่)
        isWalking = moveDir != Vector3.zero;

        // 4. การหมุนตัวละครให้หันไปตามทิศทางที่เดินอย่างนุ่มนวล
        if (isWalking)
        {
            transform.forward = Vector3.Slerp(
                transform.forward,
                moveDir,
                Time.deltaTime * rotatespeed
            );
        }
    }

    // ฟังก์ชันสำหรับตรวจสอบว่าผู้เล่นกำลังเดินอยู่หรือไม่ (เพื่อไปใช้ใน Animator)
    public bool IsWalking()
    {
        return isWalking;
    }

    // เมื่อเริ่มชนกับวัตถุอื่น
    private void OnCollisionEnter(Collision collision)
    {
        // ตรวจสอบ Tag ของวัตถุที่ชน และจัดการการโต้ตอบ (Interact) ของแต่ละเคาน์เตอร์
        if (collision.gameObject.tag == "Counter") 
        {
            // เปิดโหมด Selected Visual และเรียก Interact ของ ClearCounter
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            selectedCounter.gameObject.SetActive(true);
            ClearCounter clearcounter = collision.gameObject.GetComponent<ClearCounter>();
            clearcounter.Interact(this);
        }
        else if (collision.gameObject.tag == "Container")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            selectedCounter.gameObject.SetActive(true);
            ContainerCounter containercounter = collision.gameObject.GetComponent<ContainerCounter>();
            containercounter.Interact(this);
            Debug.Log(collision.gameObject.name);
        }
        else if (collision.gameObject.tag == "Cutting")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            selectedCounter.gameObject.SetActive(true);
            CuttingCounter containercounter = collision.gameObject.GetComponent<CuttingCounter>();
            containercounter.Interact(this);
            Debug.Log(collision.gameObject.name);
        }
        else if (collision.gameObject.tag == "PlatesCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            selectedCounter.gameObject.SetActive(true);
            PlateCounter platecounter = collision.gameObject.GetComponent<PlateCounter>();
            platecounter.Interact(this);
            Debug.Log(collision.gameObject.name);
        }
        else if (collision.gameObject.tag == "TrashCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            selectedCounter.gameObject.SetActive(true);
            TrashCounter trashcounter = collision.gameObject.GetComponent<TrashCounter>();
            trashcounter.Interact(this);
            Debug.Log(collision.gameObject.name);
        }
        else if (collision.gameObject.tag == "StoveCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            selectedCounter.gameObject.SetActive(true);
            StoveCounter stovecounter = collision.gameObject.GetComponent<StoveCounter>();
            stovecounter.Interact(this);
            Debug.Log(collision.gameObject.name);
        }
        else if (collision.gameObject.tag == "DeliveryCounter")
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(true);
            DeliveryCounter deliveryCounter = collision.gameObject.GetComponent<DeliveryCounter>();
            deliveryCounter.Interact(this);
            Debug.Log(collision.gameObject.name);
        }
    }

    // เมื่อออกจากระยะการชน
    private void OnCollisionExit(Collision collision)
    {
        // ปิดโหมด Selected Visual เมื่อเลิกสัมผัสกับเคาน์เตอร์
        string[] validTags = { "Counter", "Container", "Cutting", "PlatesCounter", "TrashCounter", "StoveCounter", "DeliveryCounter" };
        if (validTags.Contains(collision.gameObject.tag))
        {
            Transform selectedCounter = collision.gameObject.transform.Find("Selected");
            if (selectedCounter != null) selectedCounter.gameObject.SetActive(false);
            Debug.Log(collision.gameObject.name);
        }
    }

    // ฟังก์ชันดึงค่าตำแหน่งสำหรับวางวัตถุที่ถืออยู่
    public Transform GetKitchenObjectFollowTransform()
    {
        return holdPoint;
    }

    // ฟังก์ชันตรวจสอบว่าผู้เล่นถือวัตถุอะไรอยู่ในมือบ้าง (ส่งคืนเป็นรายชื่อ String)
    public string[] HasKitchenObject()
    {
        KitchenObject[] playerKitchenObject = this.GetComponentsInChildren<KitchenObject>();
        string[] listkitchenObject = {};
        if (playerKitchenObject.Length > 0)
        {
            foreach (KitchenObject obj in playerKitchenObject)
            {
                System.Array.Resize(ref listkitchenObject, listkitchenObject.Length + 1);
                listkitchenObject[listkitchenObject.Length - 1] = obj.GetKitchenObjectname();
            }
        }

        return listkitchenObject;
    }

    // ฟังก์ชันตรวจสอบเฉพาะเจาะจงว่าตอนนี้ถือ "จาน" อยู่หรือไม่
    public bool HasPlate()
    {
        KitchenObject playerPlate = this.GetComponentInChildren<KitchenObject>();
        bool isPlate = false;
        if (playerPlate != null)
        {
            if (playerPlate.GetKitchenObjectname() == "Plate")
            {
                isPlate = true;
            }
        }
        return isPlate;
    }
}
