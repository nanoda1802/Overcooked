using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ServingRack : MonoBehaviour, IInteractable
{
   // 1. 플레이어가 물건을 들고 상호작용
   // 2. 들고 있는 물건이 ingredient가 1개 이상인 plate면 받음
   // 3. 우선 들어온 주문들 기준으로 ingredients 확인 
   // 4. 확인 결과에 따라 점수 누적
   // 4. plate 비우고 참조하는 Sink에 넣음 (일정 시간 후?)

   public int currentScore; // [임시]
   [SF] private Sink sink;
   private List<FoodOrder> orders;
   
   private void Start()
   {
      orders = new List<FoodOrder>(10);
      InvokeRepeating(nameof(MakeDummyOrder),1f,30f); // [임시]
   }

   private void Update() // [임시]
   {
      if (orders.Count == 0) return;
      
      List<int> expiredOrders = new List<int>(orders.Count);
      
      for (int i = 0; i < orders.Count; i++)
      {
         orders[i].timeCheck += Time.deltaTime;
         if (orders[i].IsExpired()) expiredOrders.Add(i);
      }

      if (expiredOrders.Count == 0) return;
      foreach (int i in expiredOrders) orders.RemoveAt(i);
   }

   public bool Interact(PlayerController player)
   {
      if (sink is null || player.pickedItem is not Plate plate || !plate.HasIngredient()) return false;

      currentScore += plate.GetPlateScore(orders); // 임시
      Debug.Log($"현재 점수는 {currentScore}!");
      plate.ClearPlate();
      player.DetachItem();
      sink.AttachItem(plate);
      return true;
   }
   
   private void MakeDummyOrder()
   {
      HashSet<ItemType> recipe = new HashSet<ItemType>(3);
      for (int i = 0; i < 3; i++) recipe.Add((ItemType)i);
      orders.Add(new FoodOrder(Time.time, recipe));
      Debug.Log($"새 주문! 빵, 양배추, 치즈! (주문시간:{Time.time})");
   }


   public bool BeginWork(PlayerController player) { return false; }
   public void StopWork(PlayerController player) { }
}
