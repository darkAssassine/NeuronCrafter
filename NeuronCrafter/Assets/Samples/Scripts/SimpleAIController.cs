using System.Collections;
using UnityEngine;

namespace NeuronCrafter.Sample
{
    public class SimpleAIController : MonoBehaviour
    {
        [SerializeField] private Transform Arena;
        [SerializeField] private Transform pickUp;
        [SerializeField] private NeuralNetwork.NeuralNetwork agent;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float speed;
        [SerializeField] private float TrainIntervalInSeconds = 30;
        [SerializeField] private float FittnesEarningPerPickUp;
        [SerializeField] private float FittnesEarningForMoving;
        [SerializeField] private float FittnesLosePerSecond;
        [SerializeField] private float FittnesPerWalkingTowards;

        private float timeStanding;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(TrainIntervalInSeconds);
                agent.Train();
                transform.position = Arena.position;
            }
        }

        void FixedUpdate()
        {
            agent.AddFitnees(-Mathf.Abs(FittnesLosePerSecond) * Time.deltaTime);

            if (rb.linearVelocity.magnitude > 0.01f)
            {
                agent.AddFitnees(Mathf.Abs(FittnesEarningForMoving) * Time.deltaTime);
                timeStanding = 0;
            }
            else
            {
                timeStanding += Time.deltaTime;
                if (timeStanding > 1f)
                {
                    agent.Train();
                    transform.position = Arena.position;
                    timeStanding = 0;
                    return;
                }
            }
            Vector2 direction = new Vector2(pickUp.position.x - transform.position.x, pickUp.position.z - transform.position.z);
            direction = direction.normalized * 30;

            float[] inputs = { direction.x, direction.y };
            float[] output = agent.Run(inputs);

            float x = 0;
            float y = 0;

            if (output[0] > 0.5)
            {
                x += 1;
            }
            if (output[1] > 0.5)
            {
                x += -1;
            }
            if (output[2] > 0.5)
            {
                y += 1;
            }
            if (output[3] > 0.5)
            {
                y += -1;
            }

            rb.linearVelocity = new Vector3(x * speed, 0, y * speed);

            if (agent.FitnessValue < -6)
            {
                agent.Train();
                transform.position = Arena.position;
                timeStanding = 0;
                return;
            }
        }

        public void OnPickUp()
        {
            agent.AddFitnees(Mathf.Abs(FittnesEarningPerPickUp));
        }
    }
}
