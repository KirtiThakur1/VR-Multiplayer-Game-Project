using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MarkClone : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.SetInt("IsClone", 1);
    }
}
