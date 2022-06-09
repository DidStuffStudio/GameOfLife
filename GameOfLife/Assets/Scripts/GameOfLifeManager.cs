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
                    int sum = 0;
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
        private void Start() {
            numberCells = rows * columns;
            // create the cells
            _cells = new Cell[numberCells];
            for (int y = 0; y < rows; y++) {
                for(int x = 0; x < columns; x++) {
                    var i = y * columns + x; // index in the cells array
                    // create a cell
                    var cellGo = Instantiate(_cellPrefab, transform);
                    cellGo.name = "Cell " + i;
                    var cell = cellGo.GetComponent<Cell>();
                    _cells[i] = cell;
                    _cells[i].position = new Vector3(x, y, 0);
                    _cells[i].transform.position = _cells[i].position;
                    // get random state for the cell
                    var randomNumber = Random.Range(0, 2);
                    // print(randomNumber);
                    // if random number is 0, the cell is dead
                    if (randomNumber == 0) _cells[i].Die();
                    // if random number is 1, the cell is alive
                    else _cells[i].Survive();
                } 
            } 

            // for (int i = 0; i < numberCells; i++) SatisfyRules(i);
            // SatisfyRules();
        }

        private void Update() {
            // go through each cell and satisfy its rules
            // for (int i = 0; i < numberCells; i++) SatisfyRules(i);
            SatisfyRules();
        }

        // satisfy the rules for life of a specific cell
        private void SatisfyRules() {
            _cellsNextGen = new bool[numberCells];
            for (int y = 0; y < rows; y++) {
                for(int x = 0; x < columns; x++) {
                    int aliveNeighbours = 0;
                    var idx = y * columns + x; // index in the cells array
                    // check the neighbours of the cell
                    for (int j = -1; j <= 1; j++) {
                        for (int i = -1; i <= 1; i++) {
                            // check that the neighbour is not the cell itself
                            if (i == 0 && j == 0) continue;

                            // int neighbourX = idx + i;
                            // int neighbourY = idx + j * columns;
                            // int nidx = neighbourY * numberCells + neighbourX;
                            int nidx = idx + i + j * columns;
                            // check if the neighbour is alive
                            // if (neighbourX >= 0 && neighbourX < numberCells && neighbourY >= 0 && neighbourY < numberCells) {
                            if (nidx >= 0 && nidx < numberCells && i != 0 && j != 0) {
                                if (_cells[idx].state == Cell.State.Alive) {
                                    aliveNeighbours++;
                                }
                            }
                        }
                    }

                    print("Alive neighbours: " + aliveNeighbours);

                    // check the rules
                    if (_cells[idx].state == Cell.State.Alive) {
                        // 1 - Any live cell with fewer than two live neighbours dies --> underpopulation
                        if (aliveNeighbours < 2) {
                            _cellsNextGen[idx] = false;
                        }
                        // 2 - Any live cell with two or three live neighbours lives on to the next generation --> survival 
                        else if (aliveNeighbours == 2 || aliveNeighbours == 3) {
                            _cellsNextGen[idx] = true;
                        }
                        // 3 - Any live cell with more than three live neighbours dies --> overpopulation
                        else if (aliveNeighbours > 3) {
                            _cellsNextGen[idx] = false;
                        }
                    }
                    // 4 - Any dead cell with exactly three live neighbours becomes a live cell --> reproduction 
                    else {
                        if (aliveNeighbours == 3) {
                            _cellsNextGen[idx] = true;
                        }
                    }
                }
            }
            UpdateNextGen();
        }

        private void UpdateNextGen() {
            // set the cells to be the same as the nextGenCells¨
            // go through each cell and set its state to the nextGenState
            for (int i = 0; i < numberCells; i++) {
                // if the nextGenState is true, the cell is alive. If false, the cell is dead
                if(_cellsNextGen[i]) {
                    _cells[i].Survive();
                } else {
                    _cells[i].Die();
                }

            }
        }
        */
    }
}