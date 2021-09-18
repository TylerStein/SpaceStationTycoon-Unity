using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Navigation
{
    public class AStarNavigation
    {
        public Grid2D<AStarCell> grid;

        public List<AStarCell> openList;
        public List<AStarCell> closedList;

        public Vector2Int start;
        public Vector2Int goal;

        private AStarNavigation() {
            //
        }

        public AStarNavigation(Grid2D<AStarCell> grid) {
            this.grid = grid;
            openList = new List<AStarCell>();
            closedList = new List<AStarCell>();
        }

        public bool Search(Stack<AStarCell> path, Vector2Int start, Vector2Int goal) {
            this.start = start;
            this.goal = goal;

            openList.Clear();
            closedList.Clear();

            AStarCell startCell = new AStarCell(grid.Index(start.x, start.y), start);
            AStarCell goalCell = new AStarCell(grid.Index(goal.x, goal.y), goal);

            AStarCell[] neighbors = new AStarCell[4];
            AStarCell bestCell;

            startCell.heuristic = (goal - start).magnitude;
            openList.Add(startCell);

            while (openList.Count > 0) {

                bestCell = GetBestCell();
                openList.Remove(bestCell);

                grid.GetNeighborsNoAlloc(bestCell.index, neighbors);
                for (int i = 0; i < 4; i++) {
                    AStarCell currentCell = neighbors[i];
                    if (currentCell.blocked) {
                        continue;
                    } else if (currentCell.index == goalCell.index) {
                        currentCell.parentIndex = bestCell.index;
                        ConstructPath(path, startCell, currentCell);
                        return true;
                    }

                    float g = bestCell.cost + (currentCell.position - bestCell.position).magnitude;
                    float h = (goal - currentCell.position).magnitude;
                    bool shouldContinue = currentCell.f < (g + h);

                    if (shouldContinue && openList.Contains(currentCell)) {
                        continue;
                    }

                    if (shouldContinue && closedList.Contains(currentCell)) {
                        continue;
                    }

                    currentCell.cost = g;
                    currentCell.heuristic = h;
                    currentCell.parentIndex = bestCell.index;

                    if (openList.Contains(currentCell) == false) {
                        openList.Add(currentCell);
                    }
                }

                if (closedList.Contains(bestCell) == false) {
                    closedList.Add(bestCell);
                }
            }

            path.Clear();
            return false;
        }

        public AStarCell GetBestCell() {
            AStarCell result = null;
            float f = float.PositiveInfinity;
            
            for (int i = 0; i < openList.Count; i++) {
                AStarCell cell = openList[i];
                if (cell.f < f) {
                    f = cell.f;
                    result = cell;
                }
            }

            return result;
        }

        private void ConstructPath(Stack<AStarCell> path, AStarCell start, AStarCell destination) {
            path.Clear();
            path.Push(destination);

            AStarCell current = destination;
            while (current.parentIndex != -1 && current.parentIndex != start.index) {
                current = grid.Get(current.parentIndex);
                path.Push(current);
            }
        }
    }

    public class AStarCell
    {
        public bool blocked;
        public float cost;
        public float heuristic;
        public float f => cost + heuristic;
        public int index;
        public int parentIndex;
        public Vector2Int position;

        public AStarCell(int index, Vector2Int position, bool blocked = false) {
            this.index = index;
            this.position = position;
            this.blocked = blocked;
            parentIndex = -1;
        }

        public AStarCell() {
            position = Vector2Int.zero;
            index = -1;
            parentIndex = -1;
            blocked = true;
        }
    }
}