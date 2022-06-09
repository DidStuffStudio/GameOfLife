using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DidSuffStudio {
    /*
    Conway's Game Of Life
    Rules:
    1 - Any live cell with fewer than two live neighbours dies --> underpopulation
    2 - Any live cell with two or three live neighbours lives on to the next generation --> survival
    3 - Any live cell with more than three live neighbours dies --> overpopulation
    4 - Any dead cell with exactly three live neighbours becomes a live cell --> reproduction
    */

    // TODO --> use compute shaders to speed up the process
    // TODO --> use shader instead of using gameobject to draw the grid
    // TODO --> Make it 3D
    // TODO --> Add known patterns --> block glider, blinker, pulsar, gosper glider gun...
    
    // Based on CatLikeCoding's implementation --> https://github.com/CodingTrain/Coding-Challenges/tree/main/085_The_Game_of_Life
    // rules for 3D based on --> https://github.com/renderguy7768/cgol

    public class GameOfLifeManager3D : MonoBehaviour {
        private int numberCells = 25;
        [SerializeField] private GameObject _cellPrefab;
        private GameObject[,,] _cellsGameObjects;
        private int[,,] grid;
        private int cols;
        private int rows;
        private int depths;
        [SerializeField] private int resolution = 10;
        [SerializeField] private float cellGridOffset = 0.1f;
        [SerializeField] private float delayTime = 0.1f;

        private void Start() {
            // cols = width / resolution;
            // rows = height / resolution;
            cols = resolution;
            rows = resolution;
            depths = resolution;
            grid = new int[cols, rows, depths];
            _cellsGameObjects = new GameObject[cols, rows, depths];
            print(cellGridOffset);
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    for (int k = 0; k < depths; k++) {
                        grid[i, j, k] = Random.Range(0, 2);
                        _cellsGameObjects[i, j, k] = Instantiate(_cellPrefab, transform);
                        _cellsGameObjects[i, j, k].transform.position = new Vector3(i, j, k) * cellGridOffset;
                    }
                }
            }

            StartCoroutine(Delay());
        }

        private IEnumerator Delay() {
            while (true) {
                yield return new WaitForSeconds(delayTime);
                ApplyRules();
            }
        }

        private void ApplyRules() {
            var next = new int[cols, rows, depths];
            
            // render
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    for (int k = 0; k < depths; k++) {
                        if (grid[i, j, k] == 1) {
                            _cellsGameObjects[i, j, k].SetActive(true);
                        }
                        else {
                            _cellsGameObjects[i, j, k].SetActive(false);
                        }
                    }
                }
            }

            // Compute next based on grid
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    for (int k = 0; k < depths; k++) {
                        int state = grid[i, j, k];
                        // Count live neighbors!
                        int sum = 0;
                        int neighbors = CountNeighbors(grid, i, j, k);

                        if (state == 0 && (neighbors >= 8 && neighbors <= 12)) {
                            next[i, j, k] = 1;
                        }
                        else if (state == 1 && (neighbors < 7 || neighbors > 13)) {
                            next[i, j, k] = 0;
                        }
                        else {
                            next[i, j, k] = state;
                        }
                    }
                }
            }

            grid = next;
        }

        private int CountNeighbors(int[,,] grid, int x, int y, int z) {
            int sum = 0;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    for (int k = -1; k < 2; k++) {
                        int col = (x + i + cols) % cols;
                        int row = (y + j + rows) % rows;
                        int depth = (z + k + depths) % depths;
                        sum += grid[col, row, depth];
                    }
                }
            }

            sum -= grid[x, y, z];
            return sum;
        }
    }
}