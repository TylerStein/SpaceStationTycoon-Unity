using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SST.Gameplay.Visitor
{
    using SST.Utilities;
    using SST.Gameplay.Navigation;
    using SST.Gameplay.Controllers;
    using SST.Data;
    using SST.Gameplay.Modules;
    using SST.Gameplay.Building;

    public class GuestBehaviour : MonoBehaviour
    {
        public GuestData instanceData;
        public StateManager stateManager;
        public StateMachine stateMachine;
        public GridNavAgent navAgent;
        public GameObject guestObject;

        public bool onShip = true;
        public bool hasNeed = true;
        public string needModuleDomain = "module";

        public ShipBehaviour shipBehaviour;
        public ModuleBehaviour activeTargetModule;
        public Vector2Int activeTargetPosition;

        public void Awake() {
            stateMachine = new StateMachine();

            State idleState = new State("idle", UpdateState_idle);
            State navModuleState = new State("nav_module", UpdateState_navigateToModule);
            State enterModuleState = new State("enter_module", UpdateState_enterModule);
            State stayModuleState = new State("stay_module", UpdateState_stayModule);
            State leaveModuleState = new State("leave_module", UpdateState_leaveModule);
            State navShipState = new State("nav_ship", UpdateState_navigateToShip);
            State waitOnShipState = new State("wait_ship", UpdateState_waitOnShip);

            stateMachine.AddState(idleState);
            stateMachine.AddState(navModuleState);
            stateMachine.AddState(enterModuleState);
            stateMachine.AddState(stayModuleState);
            stateMachine.AddState(leaveModuleState);
            stateMachine.AddState(navShipState);
            stateMachine.AddState(waitOnShipState);

            idleState.AddTransition(navModuleState);
            idleState.AddTransition(navShipState);

            navModuleState.AddTransition(idleState);
            navShipState.AddTransition(idleState);
            navShipState.AddTransition(waitOnShipState);

            navModuleState.AddTransition(enterModuleState);
            enterModuleState.AddTransition(stayModuleState);
            stayModuleState.AddTransition(leaveModuleState);
            leaveModuleState.AddTransition(idleState);

            stateMachine.StartState(idleState.stateName);
        }

        private bool updateGoalTarget() {
            // decide on a destination
            activeTargetPosition = navAgent.currentPosition;
            activeTargetModule = null;

            var targets = stateManager.moduleStateController
                .FindModules(needModuleDomain)
                .Where(x => x.id != shipBehaviour.dockModule.instanceData.id)
                .ToList();

            if (targets.Count == 0) {
                // can't do anything
                return false;
            } else {
                int rngIndex = Random.Range(0, targets.Count);
                ModuleData targetModule = targets[rngIndex];
                ModuleBehaviour behaviour = stateManager.moduleStateController.GetModuleBehaviour(targetModule.id);
                Vector2Int modulePos = (Vector2Int)targetModule.position;
                BuildCell cell = navAgent.buildGrid.GetCell(modulePos, targetModule.position.z);

                var portal = behaviour.templateData.portal;
                Vector2Int rotatedPoint = navAgent.buildGrid.GetRotatedPoint(behaviour.templateData.pivot, portal.point, targetModule.direction) + modulePos;
                DirectionFlags rotatedDirection = Direction.RotateFlags(portal.direction, targetModule.direction);
                BuildCell[] bufferCells = new BuildCell[4];
                int cellCount = navAgent.buildGrid.GetCellsWithNavTypeInDirections(rotatedPoint, rotatedDirection, ENavType.Walkable, bufferCells);
                if (cellCount == 0) {
                    // can't do anything
                    return false;
                } else {
                    int rngCell = Random.Range(0, cellCount - 1);
                    int gridIndex = bufferCells[rngCell].gridIndex;

                    Vector3Int gridPos = navAgent.buildGrid.IndexToGridSpace(gridIndex);
                    activeTargetPosition = new Vector2Int(gridPos.x, gridPos.y);
                    activeTargetModule = behaviour;

                    var cell1 = navAgent.buildGrid.GetCell(activeTargetPosition);
                    var cell2 = navAgent.buildGrid.grid.Get(gridIndex);
                    var cell3 = navAgent.buildGrid.grid.Index(gridPos.x, gridPos.y, gridPos.z);

                    navAgent.SetGoalAndNavigate(activeTargetPosition);
                    return true;
                }
            }
        }

        private bool navigateToShip() {
            if (shipBehaviour.dockModule) {
                activeTargetModule = shipBehaviour.dockModule;
                activeTargetPosition = (Vector2Int)shipBehaviour.dockModule.instanceData.position;
                navAgent.SetGoalAndNavigate(activeTargetPosition);
                return true;
            }

            return false;
        }

        private void Update() {
            if (stateMachine != null) {
                if (shipBehaviour.isDocked == false) {
                    // don't do things while on ship
                    return;
                }

                stateMachine.UpdateActiveState();
            }
        }

        private void UpdateState_idle(StateMachine stateMachine) {
            if (hasNeed) {
                if (navAgent.hasPath) {
                    stateMachine.Command("nav_module");
                } else {
                    bool hasNextTarget = updateGoalTarget();
                    if (hasNextTarget) {
                        onShip = false;
                    }
                }
            } else if (!onShip && !hasNeed) {
                // got stuck returning to ship, try again
                navigateToShip();
                stateMachine.Command("nav_ship");
            }
        }

        private void UpdateState_navigateToModule(StateMachine stateMachine) {
            if (!navAgent.hasPath) {
                // path lost, check if at target
                if (Vector2Int.Distance(navAgent.currentPosition, activeTargetPosition) == 0) {
                    // at target
                    stateMachine.Command("enter_module");
                } else {
                    // target unreachable
                    stateMachine.Command("idle");
                }
            }
        }

        private void UpdateState_enterModule(StateMachine stateMachine) {
            stateMachine.Command("stay_module");
        }

        private void UpdateState_stayModule(StateMachine stateMachine) {
            hasNeed = false;
            stateMachine.Command("leave_module");
        }

        private void UpdateState_leaveModule(StateMachine stateMachine) {
            navigateToShip();
            stateMachine.Command("idle");
        }

        private void UpdateState_navigateToShip(StateMachine stateMachine) {
            if (!navAgent.hasPath) {
                // path lost, check if at target
                if (Vector2Int.Distance(navAgent.currentPosition, activeTargetPosition) == 0) {
                    // at target
                    onShip = true;
                    stateMachine.Command("wait_ship");
                    guestObject.SetActive(false);
                } else {
                    // target unreachable, do something else
                    hasNeed = true;
                    stateMachine.Command("idle");
                }
            }
        }

        private void UpdateState_waitOnShip(StateMachine stateMachine) {
            // wait here
        }
    }
}