using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ServingRack : Table
{
   [SF] private OrderManager orderManager;
   [SF] private ScoreManager scoreManager;
   [SF] private Sink sink;
   [SF] private ParticleSystem servingVfx;

   public override bool Interact(PlayerController_Stage player)
   {
      if (player.pickedItem is not Plate plate) return false;
      if (!plate.HasIngredient()) return false;
      if (!orderManager.HasActiveOrder()) return false;
      if (!orderManager.FindMatchingOrder(plate.GetIngredients(), out int baseScore, out float ratio))
      {
         return false;
      }

      PlayServingVfx();
      scoreManager.UpdateScore(baseScore, ratio);
      
      plate.ClearPlate();
      sink.PlaceItem(player.DetachItem());
      
      return true;
   }
   
   private void PlayServingVfx()
   {
      if (servingVfx is null) return;
      if (servingVfx.isPlaying) StopServingVfxSmoothly();
      servingVfx.Play();
   }

   private void StopServingVfxSmoothly()
   {
      servingVfx?.Stop(true,ParticleSystemStopBehavior.StopEmitting);
   }
}
