using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ServingRack : Table
{
   [SF] private OrderManager orderManager;
   [SF] private ScoreManager scoreManager;
   [SF] private Sink sink;
   private int _curScore; // [임시] 추후 GM으로 이동
   private readonly List<FoodOrder> _orders = new List<FoodOrder>(10);

   public override bool Interact(PlayerController player)
   {
      if (player.pickedItem is not Plate plate) return false;
      if (!plate.HasIngredient()) return false;
      if (!orderManager.HasActiveOrder())
      {
         Debug.Log("들어온 주문이 없어!");
         return false;
      }

      if (!orderManager.FindMatchingOrder(plate.GetIngredientInfos(), out float ratio)) return true;
      scoreManager.ApplyScore(ratio);
      
      plate.ClearPlate();
      sink.PlaceItem(player.DetachItem());
      
      return true;
   }
}
