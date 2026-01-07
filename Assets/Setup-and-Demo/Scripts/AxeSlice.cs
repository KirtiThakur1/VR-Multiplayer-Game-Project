using UnityEngine;
using UnityEngine.SceneManagement;

public class AxeSlice : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing we hit is a Fruit
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit != null)
        {
            // Register score
            ScoreManager.Instance.RegisterSlice(fruit);

            // Cut / destroy the fruit
            fruit.Slice();
        }
    }
}
