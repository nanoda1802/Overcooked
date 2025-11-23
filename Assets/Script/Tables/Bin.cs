using SF = UnityEngine.SerializeField;

public class Bin : Table
{
    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is null) return false;
        if (player.pickedItem is Plate plate)
        {
            plate.ClearPlate();
            return true;
        } 
        player.DetachItem().DisposeItem();
        return true;
    }
}
