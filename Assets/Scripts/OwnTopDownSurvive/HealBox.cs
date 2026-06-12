using UnityEngine;

public class HealBox : MonoBehaviour
{
    public float healAmount = 10f;

    public void Disappear()
    {
        Destroy(gameObject);
    }
}
