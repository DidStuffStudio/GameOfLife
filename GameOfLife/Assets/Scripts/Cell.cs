using UnityEngine;

namespace DidSuffStudio {
    public class Cell: MonoBehaviour {
        public Vector3 position; // cell position
        // create an enum for the state of the cell
        public enum State { Alive, Dead };
        public State state { get; private set; } // cell state
        // public int neighbours { get; private set; } // number of alive neighbours
        [SerializeField] private MeshRenderer rend; // renderer of the cell

        public void Die() {
            state = State.Dead;
            rend.material.color = Color.black;
            // gameObject.SetActive(false);
            // IDEA --> slowly reduce the alpha of the cell
            // print(name + " is dead");
        }

        public void Survive() {
            state = State.Alive;
            // gameObject.SetActive(true);
            // print(name + "Cell is alive");
            // set the color of the cell
            rend.material.color = Color.white;
        }

        public void Reproduce() {
            state = State.Alive;
            // gameObject.SetActive(true);
            // print(name + "Cell is alive");
            // set the color of the cell
            rend.material.color = Color.white;
        }
    }
}