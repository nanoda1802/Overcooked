using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "StageResultData", menuName = "SO/Stage/StageResult")]
public class StageResultData : ScriptableObject
{
   [SF] private int score;
   [SF] private int maxCombo;
   [SF] private int deliveredOrder;
   [SF] private int totalOrder;
   private const float COMBO_MODIFIER = 0.2f;
   public int Score => score;
   public int MaxCombo => maxCombo;
   public int DeliveredOrder => deliveredOrder;
   public float ComboModifier => COMBO_MODIFIER;
   
   public void Init()
   {
      score = maxCombo = deliveredOrder = totalOrder = 0;
   }

   public void ApplyPoint(int point)
   {
      score += point;
      if (score < 0) score = 0;
   }

   public bool IsMaxCombo(int combo)
   {
      return combo > maxCombo;
   }

   public void UpdateMaxCombo(int combo)
   {
      maxCombo = combo;
   }

   public void CountDeliveredOrder()
   {
      deliveredOrder++;
   }

   public void CountTotalOrder()
   {
      totalOrder++;
   }

   public float CalculateDeliverRate()
   {
      return (float) deliveredOrder / totalOrder;
   }

   public int CalculateIncome()
   {
      return (int) (score * (1 + maxCombo * 0.1f) * (1 + CalculateDeliverRate()));
   }
}
