using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Score Settings")]
    public int baseScore = 10;  

    public void Slice()
    {
       
        Destroy(gameObject);
    }
}
