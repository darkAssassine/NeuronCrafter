using UnityEngine;

namespace NeuronCrafter.Sample
{
    public class ArenaManager : MonoBehaviour
    {
        [SerializeField] private Arena arena;
        [SerializeField] private int xAmount;
        [SerializeField] private int yAmount;

        private void Awake()
        {
            for (int y = 0; y < yAmount; y++)
            {
                for (int x = 0; x < xAmount; x++)
                {
                    Vector3 pos = new Vector3(x * 30, 0, y * 30);
                    Arena newArena = Instantiate(arena, pos, Quaternion.identity);
                    newArena.Initialize(("Agent_" + y.ToString() + x.ToString()));
                }
            }
        }
    }
}
