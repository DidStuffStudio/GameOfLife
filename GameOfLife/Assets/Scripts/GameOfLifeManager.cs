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

    public class GameOfLifeManager : MonoBehaviour
    {
        [Header("Game Of Life cells")]
        [SerializeField] private int rows = 5;
        [SerializeField] private int columns = 5;
        private int numberCells = 25;
        [SerializeField] private GameObject _cellPrefab;
        private Cell [] _cells;
        private bool [] _cellsNextGen;

        /*
        [Header("Grid settings")]
        [SerializeField] private float cellGridOffset = 0.1f; // offset between cells in the grid
        */

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
    }
}