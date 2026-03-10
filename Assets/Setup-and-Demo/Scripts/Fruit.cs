using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Score Settings")]
    public int baseScore = 10;

    [HideInInspector] public bool sliced = false;

    public string DisplayName
    {
        get
        {
            return gameObject.name.Replace("(Clone)", "").Trim();
        }
    }

    public void Slice()
    {
        Destroy(gameObject);
    }
}
