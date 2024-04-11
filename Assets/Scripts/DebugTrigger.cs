using UnityEngine;
public class DebugTrigger : MonoBehaviour
{

    [SerializeField] private DebugType Type;
    [SerializeField] private OrganPool Pool;

  [SerializeField] private float ChangeByValue = 10;
   public enum DebugType
    {
        SpawnRandomOrgan,
        DealDamage,
        Heal,
        ReplenishEssense



    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
           switch(Type)
            {

                case DebugType.SpawnRandomOrgan:
                    print("Spawned Random Organ");
                    Instantiate(OrganSpawner.ReturnOrgan(OrganSpawner.ChooseRarity(1,1,1 ,Pool)));
                    break;
                case DebugType.DealDamage:
                    print("Dealt " + ChangeByValue.ToString() + " Damage");
                    player.ChangeHealth(ChangeByValue);
                    break;

                case DebugType.Heal:
                    print("Healed " + ChangeByValue.ToString() + " Health");
                    player.ChangeHealth(-ChangeByValue);
                    break;

                case DebugType.ReplenishEssense:
                    print("Replenished " + ChangeByValue.ToString() + " Essense");
                    player.EssenseCapacity += ChangeByValue;
                    break;
            }

        }



    }

}
