using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Fruit fruit = other.GetComponent<Fruit>();
        if (fruit == null)
            fruit = other.GetComponentInParent<Fruit>();

        if (fruit != null)
        {
            Destroy(fruit.gameObject);
            return;
        }

        Transform root = other.transform.root;
        if (root.CompareTag("Bomb"))
        {
            Destroy(root.gameObject);
        }
    }
}