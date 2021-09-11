using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.AI
{
    using Building;

    public class GridNavAgent : MonoBehaviour
    {
        public float moveSpeed = 4f;
        public float minTargetDistance = 0.15f;

        public Vector2Int currentPosition;
        public Vector2Int goalPosition;

        public Vector3 currentPositionWorld;
        public Vector3 goalPositionWorld;

        public BuildingGrid buildGrid;
        public AStarNavigation navigation;

        public List<BuildCell> walkableCells;
        public Grid2D<AStarCell> navGrid;

        public Stack<AStarCell> path = new Stack<AStarCell>();
        public bool hasPath = false;
        public Vector2Int checkpointPosition;
        public Vector3 checkpointWorldPosition;

        private void Awake() {
            if (buildGrid != null) {
                navGrid = new Grid2D<AStarCell>(buildGrid.gridSize.x, buildGrid.gridSize.y, new AStarCell());
                navigation = new AStarNavigation(navGrid);
            }
        }

        private void Start() {
            UpdatePosition();
            RebuildNavGrid();
           // SetRandomGoal();

            buildGrid.gridChanged.AddListener(() => {
                RebuildNavGrid();
            });
        }

        public void SetGoalAndNavigate(Vector2Int target) {
            // Set goal position
            goalPosition = target;
            goalPositionWorld = buildGrid.GridToWorldSpace(goalPosition);

            // Update the path
            RecalculatePath();
        }

        public void RebuildNavGrid(bool force = false) {
            if (force || buildGrid.gridSize.x != navGrid.xSize || buildGrid.gridSize.y != navGrid.ySize) {
                navGrid = new Grid2D<AStarCell>(buildGrid.gridSize.x, buildGrid.gridSize.y, new AStarCell());
                navigation = new AStarNavigation(navGrid);
            }

            // Fill grid
            for (int x = 0; x < buildGrid.gridSize.x; x++) {
                for (int y = 0; y < buildGrid.gridSize.y; y++) {
                    BuildCell buildCell = buildGrid.GetCell(new Vector2Int(x, y), 0);
                    int navCellIndex = navGrid.Index(x, y);
                    Vector2Int navCellPosition = new Vector2Int(x, y);
                    navGrid.Set(navCellIndex, new AStarCell(navCellIndex, navCellPosition, buildCell.navType != ENavType.Walkable));
                }
            }

            if (hasPath) {
                RecalculatePath();
                if (!hasPath) SetRandomGoal();
            }
        }

        public void RecalculatePath() {
            // Don't move to blocked target
            AStarCell targetCell = navGrid.Get(goalPosition.x, goalPosition.y);
            if (targetCell == null || targetCell.blocked == true) {
                hasPath = false;
                return;
            }

            // Ensure accurate position
            UpdatePosition();

            // Calculate navigation path
            hasPath = navigation.Search(path, currentPosition, goalPosition);

            // Set the next target
            RecalculateTarget();
        }

        public void RecalculateTarget() {
            if (hasPath && path.Count > 0) {
                checkpointPosition = path.Peek().position;
                checkpointWorldPosition = buildGrid.GridToWorldSpace(checkpointPosition);
                checkpointWorldPosition += new Vector3(Random.Range(-0.45f, 0.45f), 0f, Random.Range(-0.45f, 0.45f));
            }
        }

        public void UpdatePosition() {
            currentPositionWorld = transform.position;
            currentPosition = buildGrid.WorldToGridSpace(currentPositionWorld);
        }

        private void Update() {
            UpdatePosition();

            if (hasPath) {
                if (IsAtTarget()) {
                    path.Pop();
                    if (path.Count == 0) {
                        // SetRandomGoal();
                        hasPath = false;
                    } else {
                        RecalculateTarget();
                    }
                } else {
                    // move toward current path target
                    Vector3 toTarget = checkpointWorldPosition - transform.position;
                    transform.position += toTarget.normalized * Time.deltaTime * moveSpeed;
                }
            }
        }

        private bool IsAtTarget() {
            return currentPosition == checkpointPosition
                && Vector3.Distance(currentPositionWorld, checkpointWorldPosition) < minTargetDistance;
        }

        private void SetRandomGoal() {
            // Update position
            UpdatePosition();

            // Get current position index
            int currentPosIndex = navGrid.Index(currentPosition.x, currentPosition.y);

            // Create a random goal
            int randomIndex = -1;
            while (randomIndex == -1) {
                randomIndex = Random.Range(0, navGrid.Length);
                if (currentPosIndex == randomIndex || navGrid.Get(randomIndex).blocked == true) {
                    // Don't select current cell
                    randomIndex = -1;
                }
            }

            // Set goal position
            goalPosition = navGrid.Position(randomIndex);
            goalPositionWorld = buildGrid.GridToWorldSpace(goalPosition);

            // Update the path
            RecalculatePath();
        }

        private void OnDrawGizmos() {
            if (buildGrid != null) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(currentPositionWorld, 0.5f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(goalPositionWorld, 0.5f);

                Gizmos.DrawLine(transform.position, checkpointWorldPosition);
            }
        }
    }
}