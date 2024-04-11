using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    private TextMeshProUGUI HP;
    private TextMeshProUGUI Essense;
    private TextMeshProUGUI AmmoCounter;

    private void Awake()
    {
        HP = transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>();
        Essense = transform.GetChild(2).transform.GetComponent<TextMeshProUGUI>();
        AmmoCounter = transform.GetChild(3).transform.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateHealth( float max , float current)
    {

        HP.text = current.ToString() + " / " + max.ToString();
    }

    public void UpdateAmmo(float current, float remaining)
    {
        AmmoCounter.text = current.ToString() + " / " + remaining.ToString();
    }

    public void UpdateEssense(float max, float current)
    {
        HP.text = current.ToString() + " / " + max.ToString();
    }


}
