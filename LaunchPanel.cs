using UnityEngine;

public class LaunchPanel : MonoBehaviour
{
    public float bounceForce = 10f; // The force with which the player will be bounced up
    public Rigidbody playerRigidbody; // Public variable for the Rigidbody

    public AudioSource Dash;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Assign the Rigidbody if it's not already assigned
            if (playerRigidbody == null)
            {
                playerRigidbody = other.GetComponent<Rigidbody>();
            }

            if (playerRigidbody != null)
            {
                // Apply an upward force
                LaunchActivate();
            }
             
        }

        void LaunchActivate()
        {
            playerRigidbody.AddForce(Vector3.forward * bounceForce, ForceMode.Impulse);
            Dash.Play();
        }
    }

}
