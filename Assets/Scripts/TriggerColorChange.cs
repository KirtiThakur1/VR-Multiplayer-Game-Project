using UnityEngine;

public class TriggerColorChange : MonoBehaviour

{

        public Renderer rend;            // To store cube renderer
        public Color defaultColor = Color.gray;
        public Color triggeredColor = Color.yellow;

        private void Start()
        {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;  
    }

        private void OnTriggerEnter(Collider other)
        {
            // Only react to users (VR or Desktop)
            if (other.CompareTag("Player"))
            {
                rend.material.color = triggeredColor;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                rend.material.color = defaultColor;
            }
        }


}

