using UnityEngine;

namespace SST.Gameplay.Visitor
{
    using Utilities;
    using AI;
    using Building;

    public class CrewController : MonoBehaviour
    {
        public StateMachine stateMachine;
        public GridNavAgent navAgent;
        public BuildingGrid grid;
        public GameObject crewObject;

        public bool onShip = true;
        public bool hasNeed = false;
        public Vector2Int moduleEntranceGoalGridPosition;
        public Vector2Int dockedShipGridPosition;

        public void Initialize() {
            stateMachine = new StateMachine();

            State idleState = new State("idle", UpdateState_idle);
            State navModuleState = new State("nav_module", UpdateState_navigateToModule);
            State enterModuleState = new State("enter_module", UpdateState_enterModule);
            State stayModuleState = new State("stay_module", UpdateState_stayModule);
            State leaveModuleState = new State("leave_module", UpdateState_leaveModule);
            State navShipState = new State("nav_ship", UpdateState_navigateToShip);

            idleState.AddTransition(navModuleState);
            idleState.AddTransition(navShipState);

            navModuleState.AddTransition(idleState);
            navShipState.AddTransition(idleState);

            navModuleState.AddTransition(enterModuleState);
            enterModuleState.AddTransition(stayModuleState);
            stayModuleState.AddTransition(leaveModuleState);
            leaveModuleState.AddTransition(idleState);

            stateMachine.StartState(idleState.stateName);
        }

        private void UpdateState_idle(StateMachine stateMachine) {
            //if (hasNeed) { 
            //    if (onShip) {
            //        // get off ship
            //        crewObject.SetActive(true);
            //        crewObject.transform.position = grid.GridToWorldSpace(dockedShipGridPosition);
            //        Vector3 offset = new Vector3(Random.Range(-0.15f, 0.15f), 0f, Random.Range(-0.15f, 0.15f));
            //        crewObject.transform.position += offset;

            //        onShip = false;

            //        int rngIndex = Random.Range(0, pathPositions.Count);
            //        Vector2Int goalPosition = buildingGrid.WorldToGridSpace(pathPositions[rngIndex]);
            //        crew.goalPosition = goalPosition;

            //        crew.navAgent.UpdatePosition();
            //        crew.navAgent.RebuildNavGrid(true);
            //    }
            
            //}
        }

        private void UpdateState_navigateToModule(StateMachine stateMachine) {

        }

        private void UpdateState_enterModule(StateMachine stateMachine) {

        }

        private void UpdateState_stayModule(StateMachine stateMachine) {

        }

        private void UpdateState_leaveModule(StateMachine stateMachine) {

        }

        private void UpdateState_navigateToShip(StateMachine stateMachine) {
        }
    }
}