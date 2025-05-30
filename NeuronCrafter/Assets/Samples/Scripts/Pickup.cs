using UnityEngine;

namespace NeuronCrafter.Sample
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private Transform ArenaMiddlePoint;
        [SerializeField] private float TeleportRange;
        private void Start()
        {
            Teleport();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<SimpleAIController>() != null)
            {
                other.GetComponent<SimpleAIController>().OnPickUp();
                Teleport();
            }
        }

        private void Teleport()
        {
            transform.position = new Vector3(ArenaMiddlePoint.position.x + Random.Range(-TeleportRange, TeleportRange), 0.7f, ArenaMiddlePoint.position.z + Random.Range(-TeleportRange, TeleportRange));
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<SimpleAIController>() != null)
            {
                other.GetComponent<SimpleAIController>().OnPickUp();
                Teleport();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<SimpleAIController>() != null)
            {
                other.GetComponent<SimpleAIController>().OnPickUp();
                Teleport();
            }
        }
    }
}
