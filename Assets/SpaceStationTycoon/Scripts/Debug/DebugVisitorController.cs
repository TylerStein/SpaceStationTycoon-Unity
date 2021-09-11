using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay
{
    using AI;
    using Building;
    using Data;
    using Controllers;
    using Modules;

    public class DebugVisitorController : MonoBehaviour
    {
        public StateManager stateManager;
        public BuildingGrid buildingGrid;
        public SpawningController spawningController;
        public List<DebugVisitor> visitors = new List<DebugVisitor>();

        public ModuleTemplateData dockTemplate;
        public ModuleTemplateData pathTemplate;
        public float shipMoveSpeed = 5f;

        public List<Vector3> dockPositions = new List<Vector3>();
        public List<Vector3> pathPositions = new List<Vector3>();

        public Vector3 gridMaxWorld = Vector3.one;
        public Vector3 gridMinWorld = Vector3.one;

        public int maxVisitors = 1;
        public int maxVisitorCrew = 4;

        public string targetModuleDomain = "module";

        // Start is called before the first frame update
        void Start() {
            OnGridChange();
            buildingGrid.gridChanged.AddListener(() => {
                OnGridChange();
            });
        }

        // Update is called once per frame
        void Update() {
            if (visitors.Count == 0) {
                SpawnVisitor();
            }

            List<DebugVisitor> toDelete = new List<DebugVisitor>();
            foreach (var visitor in visitors) {
                if (visitor.docked) {
                    UpdateDockedVisitor(visitor, toDelete);
                } else {
                    UpdateUndockedVisitor(visitor, toDelete);
                }
            }

            foreach (var visitor in toDelete) {
                visitors.Remove(visitor);
            }
        }

        void SpawnVisitor() {
            int rngIndex = Random.Range(0, dockPositions.Count);
            Vector3 dir = (gridMaxWorld - gridMinWorld).normalized;
            Vector3 position = gridMinWorld + (dir * 10f) + (Vector3.up * 10f);
            GameObject ship = Instantiate(spawningController.shipPrefab, position, Quaternion.identity);

            List<DebugCrew> crew = new List<DebugCrew>();
            for (int i = 0; i < Random.Range(1, maxVisitorCrew); i++) {
                GameObject crewObject = Instantiate(spawningController.crewPrefab, position, Quaternion.identity);
                GridNavAgent navAgent = crewObject.GetComponent<GridNavAgent>();
                navAgent.buildGrid = buildingGrid;
                crew.Add(new DebugCrew() {
                    crewObject = crewObject,
                    navAgent = navAgent,
                    onShip = true,
                });
                crewObject.SetActive(false);
            }

            DebugVisitor visitor = new DebugVisitor() {
                shipObject = ship,
                crew = crew,
                offScreenPosition = position,
                targetDockPosition = dockPositions[rngIndex],
                dockGridPosition = buildingGrid.WorldToGridSpace(dockPositions[rngIndex]),
                docked = false,
            };

            visitors.Add(visitor);
        }

        void OnGridChange() {
            gridMaxWorld = buildingGrid.GridToWorldSpace(new Vector2Int(buildingGrid.gridSize.x - 1, buildingGrid.gridSize.y - 1));
            gridMinWorld = buildingGrid.GridToWorldSpace(new Vector2Int(0, 0));

            dockPositions.Clear();
            pathPositions.Clear();

            List<BuildCell> builtCells = buildingGrid.FindCells(EBuildType.Occupied, 0);
            foreach (var cell in builtCells) {
                if (cell.gameObject.GetComponent<Modules.ModuleBehaviour>().templateData == dockTemplate) {
                    Vector3Int gridPos = buildingGrid.grid.Position(cell.gridIndex);
                    Vector3 worldPos = buildingGrid.GridToWorldSpace(new Vector2Int(gridPos.x, gridPos.y));
                    dockPositions.Add(worldPos);
                } else if (cell.gameObject.GetComponent<Modules.ModuleBehaviour>().templateData == pathTemplate) {
                    Vector3Int gridPos = buildingGrid.grid.Position(cell.gridIndex);
                    Vector3 worldPos = buildingGrid.GridToWorldSpace(new Vector2Int(gridPos.x, gridPos.y));
                    pathPositions.Add(worldPos);
                }
            }
        }

        void UpdateDockedVisitor(DebugVisitor visitor, List<DebugVisitor> toDelete) {
            bool crewIsOut = false;
            foreach (DebugCrew crew in visitor.crew) {
                if (crew.onShip == false) {
                    crewIsOut = true;
                    break;
                }
            }

            if (crewIsOut) {
                // update crew
                foreach (DebugCrew crew in visitor.crew) {
                    if (crew.onShip) {
                        continue;
                    }

                    if (crew.needSatisfied) {
                        // can return to ship
                        if (crew.navAgent.hasPath) {
                            // on the way to ship
                        } else {
                            if (Vector2Int.Distance(crew.navAgent.currentPosition, visitor.dockGridPosition) == 0) {
                                // reached ship
                                crew.onShip = true;
                                crew.crewObject.SetActive(false);
                            } else {
                                crew.navAgent.SetGoalAndNavigate(visitor.dockGridPosition);
                            }
                        }
                    } else {
                        // can go to module
                        if (crew.navAgent.hasPath) {
                            // on the way to module
                        } else {
                            // get goal position
                            if (Vector2Int.Distance(crew.navAgent.currentPosition, crew.goalPosition) == 0) {
                                // reached goal
                                crew.needSatisfied = true;
                            } else {
                                crew.navAgent.SetGoalAndNavigate(crew.goalPosition);
                            }
                        }
                    }
                }
            } else {
                // leave
                float dist = Vector3.Distance(visitor.shipObject.transform.position, visitor.offScreenPosition);
                if (dist < 0.5f) {
                    visitor.shipObject.transform.position = visitor.offScreenPosition;
                    for (int i = visitor.crew.Count - 1; i >= 0; i--) {
                        Destroy(visitor.crew[i].crewObject);
                    }

                    Destroy(visitor.shipObject);
                    toDelete.Add(visitor);
                    // visitors.Remove(visitor);
                } else {
                    Vector3 move = Vector3.Lerp(visitor.shipObject.transform.position, visitor.offScreenPosition, Time.deltaTime * shipMoveSpeed);
                    visitor.shipObject.transform.position = move;
                }
            }
        }

        void UpdateUndockedVisitor(DebugVisitor visitor, List<DebugVisitor> toDelete) {
            float dist = Vector3.Distance(visitor.shipObject.transform.position, visitor.targetDockPosition);
            if (dist < 0.5f) {
                visitor.shipObject.transform.position = visitor.targetDockPosition;
                visitor.docked = true;

                // send out crew
                foreach (DebugCrew crew in visitor.crew) {
                    crew.crewObject.transform.position = visitor.targetDockPosition;
                    Vector3 offset = new Vector3(Random.Range(-0.15f, 0.15f), 0f, Random.Range(-0.15f, 0.15f));
                    crew.crewObject.transform.position += offset;
                    crew.needSatisfied = false;
                    crew.onShip = false;
                    crew.crewObject.SetActive(true);

                    // decide on a destination
                    var targets = stateManager.buildStateController.FindModules(targetModuleDomain);
                    if (targets.Count == 0) {
                        // can't do anything :(
                        crew.goalPosition = visitor.dockGridPosition;
                    } else {
                        int rngIndex = Random.Range(0, targets.Count);
                        ModuleData targetModule = targets[rngIndex];
                        ModuleBehaviour behaviour = stateManager.buildStateController.GetModuleBehaviour(targetModule.id);
                        Vector2Int modulePos = new Vector2Int(targetModule.position.x, targetModule.position.y);
                        BuildCell cell = buildingGrid.GetCell(modulePos, targetModule.position.z);

                        var portal = behaviour.templateData.portal;
                        Vector2Int rotatedPoint = buildingGrid.GetRotatedPoint(behaviour.templateData.pivot, portal.point, targetModule.direction) + modulePos;
                        DirectionFlags rotatedDirection = Direction.RotateFlags(portal.direction, targetModule.direction);
                        BuildCell[] bufferCells = new BuildCell[4];
                        int cellCount = buildingGrid.GetCellsWithNavTypeInDirections(rotatedPoint, rotatedDirection, ENavType.Walkable, bufferCells);
                        if (cellCount == 0) {
                            // can't do anything yet again :(
                            crew.goalPosition = visitor.dockGridPosition;
                        } else {
                            int rngCell = Random.Range(0, cellCount - 1);
                            Vector3Int gridPos = buildingGrid.IndexToGridSpace(bufferCells[rngCell].gridIndex);
                            crew.goalPosition = new Vector2Int(gridPos.x, gridPos.y);
                        }

                    }

                    //int rngIndex = Random.Range(0, pathPositions.Count);
                    //Vector2Int goalPosition = buildingGrid.WorldToGridSpace(pathPositions[rngIndex]);
                    //crew.goalPosition = goalPosition;

                    crew.navAgent.UpdatePosition();
                    crew.navAgent.RebuildNavGrid(true);
                }
            } else {
                Vector3 move = Vector3.Lerp(visitor.shipObject.transform.position, visitor.targetDockPosition, Time.deltaTime * shipMoveSpeed);
                visitor.shipObject.transform.position = move;
            }
        }
    }

    [SerializeField]
    public class DebugVisitor
    {
        public GameObject shipObject;
        public List<DebugCrew> crew;
        public Vector3 targetDockPosition;
        public Vector2Int dockGridPosition;
        public Vector3 offScreenPosition;
        public bool docked;
    }

    [SerializeField]
    public class DebugCrew
    {
        public GameObject crewObject;
        public GridNavAgent navAgent;
        public Vector2Int goalPosition;
        public bool onShip;
        public bool needSatisfied;
    }
}