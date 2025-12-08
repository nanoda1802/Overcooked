using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ServingRack : Table
{
   [SF] private OrderManager orderManager;
   [SF] private ScoreManager scoreManager;
   [SF] private Sink sink;

   public override bool Interact(PlayerController player)
   {
      if (player.pickedItem is not Plate plate) return false;
      if (!plate.HasIngredient()) return false;
      if (!orderManager.HasActiveOrder()) return false;

      if (!orderManager.FindMatchingOrder(plate.GetIngredients(), out int baseScore, out float ratio)) return true;
      scoreManager.ApplyScore(baseScore, ratio);
      
      plate.ClearPlate();
      sink.PlaceItem(player.DetachItem());
      
      return true;
   }
}
