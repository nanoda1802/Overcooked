using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ServingRack : Table
{
   [SF] private Sink sink;
   private int _curScore; // [임시] 추후 GM으로 이동
   private readonly List<FoodOrder> _orders = new List<FoodOrder>(10);
   
   // private void Start()
   // {
   //    InvokeRepeating(nameof(MakeDummyOrder),1f,30f); // [임시]
   // }
   //
   // private void Update() // [임시]
   // {
   //    if (_orders.Count == 0) return;
   //    
   //    List<int> expiredOrders = new List<int>(_orders.Count);
   //    
   //    for (int i = 0; i < _orders.Count; i++)
   //    {
   //       _orders[i].timeCheck += Time.deltaTime;
   //       if (_orders[i].IsExpired(out int score))
   //       {
   //          _curScore -= score;
   //          if (_curScore < 0) _curScore = 0;
   //          Debug.Log($"만료로 {score}만큼 감점! 현재 점수는 {_curScore}!");
   //          expiredOrders.Add(i);
   //       }
   //    }
   //
   //    if (expiredOrders.Count == 0) return;
   //    foreach (int i in expiredOrders) _orders.RemoveAt(i);
   // }

   public override bool Interact(PlayerController player)
   {
      if (player.pickedItem is not Plate plate) return false;
      if (!plate.HasIngredient()) return false;
      if (_orders.Count == 0)
      {
         Debug.Log("들어온 주문이 없어!");
         return false;
      }
      
      _curScore += plate.GetPlateScore(_orders); // 임시
      Debug.Log($"제출 성공! 현재 점수는 {_curScore}!");
      
      plate.ClearPlate();
      sink.PlaceItem(player.DetachItem());
      
      return true;
   }
   
   private void MakeDummyOrder() // [임시]
   {
      HashSet<ItemType> recipe = new HashSet<ItemType>(3);
      int rnd = Random.Range(2, 5);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < rnd; i++)
      {
         ItemType type = (ItemType) i;
         sb.Append(type).Append(',');
         recipe.Add(type);
      }
      // _orders.Add(new FoodOrder(Time.time, recipe));
      Debug.Log($"새 주문! {sb}! (주문시간:{Time.time})");
   }
}
