using SF = UnityEngine.SerializeField;

public class ServingRack : Table
{
   [SF] private OrderManager orderManager;
   [SF] private ScoreManager scoreManager;
   [SF] private Sink sink;

   public override bool Interact(InStagePlayerController player)
   {
      if (player.pickedItem is not Plate plate) return false;
      if (!plate.HasIngredient()) return false;
      if (!orderManager.HasActiveOrder()) return false;

      if (!orderManager.FindMatchingOrder(plate.GetIngredients(), out int baseScore, out float ratio))
      {
         // [sfx] 제출할 수 없는 음식 블락 소리
         return true;
      }
      scoreManager.UpdateScore(baseScore, ratio);
      
      plate.ClearPlate();
      sink.PlaceItem(player.DetachItem());
      
      return true;
   }
}
