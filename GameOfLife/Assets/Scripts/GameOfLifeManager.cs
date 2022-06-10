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

    public class GameOfLifeManager : MonoBehaviour {
        private int numberCells = 25;
        [SerializeField] private GameObject _cellPrefab;
        private GameObject[,] _cellsGameObjects;
        private int[,] grid;
        private int cols;
        private int rows;
        [SerializeField] private int resolution = 10;

        private void Start() {
            // cols = width / resolution;
            // rows = height / resolution;
            cols = resolution;
            rows = resolution;
            grid = new int[cols, rows];
            _cellsGameObjects = new GameObject[cols, rows];
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    grid[i, j] = Random.Range(0, 2);
                    _cellsGameObjects[i, j] = Instantiate(_cellPrefab, transform);
                    _cellsGameObjects[i, j].transform.position = new Vector3(i, j, 0);
                }
            }
        }

        private void Update() {
            var next = new int[cols, rows];
            
            // render
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    int x = i * resolution;
                    int y = j * resolution;
                    if (grid[i,j] == 1) {
                        _cellsGameObjects[i,j].SetActive(true);
                        // rect(x, y, resolution - 1, resolution - 1);
                    }
                    else {
                        _cellsGameObjects[i,j].SetActive(false);
                    }
                }
            }

            // Compute next based on grid
            for (int i = 0; i < cols; i++) {
                for (int j = 0; j < rows; j++) {
                    int state = grid[i, j];
                    // Count live neighbors!
                    int neighbors = CountNeighbors(grid, i, j);

                    if (state == 0 && neighbors == 3) {
                        next[i, j] = 1;
                    }
                    else if (state == 1 && (neighbors < 2 || neighbors > 3)) {
                        next[i, j] = 0;
                    }
                    else {
                        next[i, j] = state;
                    }
                }
            }

            grid = next;
        }

        private int CountNeighbors(int[,] grid, int x, int y) {
            int sum = 0;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int col = (x + i + cols) % cols;
                    int row = (y + j + rows) % rows;
                    sum += grid[col, row];
                }
            }

            sum -= grid[x, y];
            return sum;
        }
        /*
         // For the compute shader:
         
            Vector2Int size = new Vector2Int(256, 128);
            float[,] debugArray = new float[size.x, size.y];
            //fill with data
            ComputeBuffer debugBuffer = new ComputeBuffer(size.x * size.y, sizeof(float));
            debugBuffer.SetData(debugArray);
            computeShader.SetBuffer(kernelHandle, Shader.PropertyToID("debugBuffer"), debugBuffer);
            //dispatch
            debugBuffer.GetData(debugArray);
         */
    }
}