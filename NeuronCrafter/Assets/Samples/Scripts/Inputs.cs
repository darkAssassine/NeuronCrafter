using UnityEngine;
using UnityEngine.InputSystem;

namespace NeuronCrafter.Sample
{
    public class Inputs : MonoBehaviour
    {
        public Vector2 MovementAxis { get; private set; }

        public Vector2 Look { get; private set; }
        public float UpAndDownAxis { get; private set; }
        public float SpeedMulti { get; private set; }

        [SerializeField, Header("InputAction")]
        private InputActionAsset playerActionAsset;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction upAndDownAction;
        private InputAction scrollAction;

        private Vector2 oldMousePos;

        private void Awake()
        {
            SpeedMulti = 1f;
            InputActionMap inputActionMap = playerActionAsset.FindActionMap("Base");

            moveAction = inputActionMap.FindAction("Movement");
            lookAction = inputActionMap.FindAction("Look");
            upAndDownAction = inputActionMap.FindAction("UpDown");
            scrollAction = inputActionMap.FindAction("Scroll");

            scrollAction.performed += Scroll;
        }

        private void Scroll(InputAction.CallbackContext context)
        {
            float y = context.ReadValue<Vector2>().y;

            if (SpeedMulti <= 1)
            {
                SpeedMulti += y / 10;
            }
            else if (SpeedMulti <= 10)
            {
                SpeedMulti += y / 2;
            }
            else if (SpeedMulti <= 20)
            {
                SpeedMulti += y;
            }
            else
            {
                SpeedMulti += y * 2;
            }

            if (SpeedMulti <= 0)
            {
                SpeedMulti = 0.05f;
            }
        }

        private void Update()
        {
            EvaluateInputs();
        }

        private void EvaluateInputs()
        {
            MovementAxis = moveAction.ReadValue<Vector2>();
            UpAndDownAxis = upAndDownAction.ReadValue<float>();
            Look = lookAction.ReadValue<Vector2>();

        }

        private void Start()
        {
            Enable();
        }

        private void OnDisable()
        {
            Disable();
        }

        private void Disable()
        {
            moveAction.Disable();
            lookAction.Disable();
            upAndDownAction.Disable();
            lookAction.Disable();
            scrollAction.Disable();
        }

        private void Enable()
        {
            moveAction.Enable();
            lookAction.Enable();
            upAndDownAction.Enable();
            lookAction.Enable();
            scrollAction.Enable();
        }
    }
}
