using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    // สร้าง Event สำหรับแจ้งเตือนเมื่อมีการเพิ่มวัตถุดิบลงในจาน (ใช้สำหรับ UI และ Visual) [1]
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    // รายการวัตถุดิบที่รับได้ (กำหนดค่าจาก Unity Editor) [2]
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    // รายการวัตถุดิบที่มีอยู่บนจานในขณะนี้ [3]
    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();
        // เริ่มต้นรายการวัตถุดิบใหม่ [3]
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    // ฟังก์ชันสำหรับพยายามเพิ่มวัตถุดิบลงในจาน [4]
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // 1. ตรวจสอบก่อนว่าวัตถุดิบนี้อนุญาตให้ใส่ได้หรือไม่ [5]
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }

        // 2. ตรวจสอบว่าวัตถุดิบนี้มีอยู่บนจานแล้วหรือยัง (ป้องกันการเพิ่มซ้ำ) [4]
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        else
        {
            // 3. ถ้าผ่านเงื่อนไข ให้เพิ่มวัตถุดิบลงในรายการ [4]
            kitchenObjectSOList.Add(kitchenObjectSO);

            // ส่งสัญญาณ Event แจ้งเตือนว่ามีวัตถุดิบเพิ่มขึ้น [1]
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });

            return true;
        }
    }

    // ฟังก์ชันสำหรับดึงรายการวัตถุดิบทั้งหมดบนจาน [6, 7]
    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}