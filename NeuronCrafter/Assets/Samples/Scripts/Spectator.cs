using UnityEngine;

namespace NeuronCrafter.Sample
{
    public class Spectator : MonoBehaviour
    {
        [SerializeField] private Inputs input;

        [SerializeField] private float ForwardSpeed;
        [SerializeField] private float SidewardsSpeed;
        [SerializeField] private float UpDownSpeed;
        [SerializeField] private float sensivityX;
        [SerializeField] private float sensivityY;

        private float yaw = 0.0f;
        private float pitch = 0.0f;


        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            transform.position += transform.forward * input.MovementAxis.y * ForwardSpeed * Time.deltaTime * input.SpeedMulti;
            transform.position += transform.right * input.MovementAxis.x * SidewardsSpeed * Time.deltaTime * input.SpeedMulti;
            transform.position += transform.up * input.UpAndDownAxis * UpDownSpeed * Time.deltaTime * input.SpeedMulti;

            yaw += input.Look.x * sensivityX * Time.deltaTime;
            pitch -= input.Look.y * sensivityY * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -88, 88);
            transform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
    }
}
