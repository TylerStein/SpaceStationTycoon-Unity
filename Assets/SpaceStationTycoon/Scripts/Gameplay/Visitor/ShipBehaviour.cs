using UnityEngine;
using System.Collections.Generic;

namespace SST.Gameplay.Visitor
{
    using SST.Utilities;
    using SST.Data;
    using SST.Gameplay.Modules;
    using SST.Gameplay.Controllers;

    public class ShipBehaviour : MonoBehaviour
    {
        public ShipData instanceData;
        public ModuleBehaviour dockModule;
        public StateManager stateManager;

        public GameObject shipObject;

        public Vector3 dockWorldPosition;
        public Vector3 edgeWorldPosition;

        public StateMachine stateMachine;
        public List<uint> guestIds;
        public bool isDocked = false;
        public bool hasDocked = false;
        public float moveSpeed = 10f;

        public Interpolation interpolation;
        public void Awake() {
            interpolation = new Interpolation();
            stateMachine = new StateMachine();

            State idleState = new State("idle", UpdateState_idle);
            State enterState = new State("enter", UpdateState_enter, EnterState_enter);
            State dockedState = new State("docked", UpdateState_docked);
            State exitState = new State("exit", UpdateState_exit, EnterState_exit);

            stateMachine.AddState(idleState);
            stateMachine.AddState(enterState);
            stateMachine.AddState(dockedState);
            stateMachine.AddState(exitState);

            idleState.AddTransition(enterState);
            idleState.AddTransition(dockedState);
            idleState.AddTransition(exitState);

            enterState.AddTransition(dockedState);
            dockedState.AddTransition(exitState);

            stateMachine.StartState(idleState.stateName);
        }

        public void Update() {
            if (stateMachine != null) {
                stateMachine.UpdateActiveState();
            }
        }

        private void UpdateState_idle(StateMachine stateMachine) {
            if (!isDocked) {
                if (hasDocked) {
                    stateMachine.Command("exit");
                } else {
                    stateMachine.Command("enter");
                }
            } else {
                stateMachine.Command("docked");
            }
        }

        private void EnterState_enter(StateMachine stateMachine, string fromState) {
            interpolation.Reset();
            if (dockModule.instanceData.occupants.Contains(instanceData.id) == false) {
                dockModule.instanceData.occupants.Add(instanceData.id);
            }
        }

        private void UpdateState_enter(StateMachine stateMachine) {
            float dist = Vector3.Distance(shipObject.transform.position, dockWorldPosition);
            if (dist < 0.01f) {
                shipObject.transform.position = dockWorldPosition;
                isDocked = true;
                stateMachine.Command("docked");
            } else {
                interpolation.Update(moveSpeed * stateManager.timeStateController.timeScale * Time.deltaTime);
                Vector3 move = interpolation.Cosine(shipObject.transform.position, dockWorldPosition);
                shipObject.transform.position = move;
            }
        }

        private void UpdateState_docked(StateMachine stateMachine) {
            // todo: check guests state
            hasDocked = true;

            bool guestNotDone = false;
            GuestBehaviour guest;
            foreach (var guestId in guestIds) {
                guest = stateManager.visitorStateController.GetGuestBehaviour(guestId);
                if (!guest.onShip || guest.hasNeed) {
                    guestNotDone = true;
                    if (guest.onShip) {
                        // guest leaves ship
                        guest.guestObject.transform.position = shipObject.transform.position;
                        guest.guestObject.SetActive(true);
                        guest.navAgent.Initialize();
                    }
                }
            }

            if (guestNotDone == false) {
                // all guests on ship
                stateMachine.Command("exit");
            }
        }

        private void EnterState_exit(StateMachine stateMachine, string fromState) {
            interpolation.Reset();
        }

        private void UpdateState_exit(StateMachine stateMachine) {
            isDocked = false;
            float dist = Vector3.Distance(shipObject.transform.position, edgeWorldPosition);
            if (dist < 0.2f) {
                shipObject.transform.position = edgeWorldPosition;
                DespawnSelf();
                // todo: despawn
            } else {
                interpolation.Update(moveSpeed * stateManager.timeStateController.timeScale * Time.deltaTime);
                Vector3 move = interpolation.Cosine(shipObject.transform.position, edgeWorldPosition);
                shipObject.transform.position = move;
            }
        }

        private void DespawnSelf() {
            dockModule.instanceData.occupants.Remove(instanceData.id);

            GuestBehaviour guest;
            foreach (uint guestId in guestIds) {
                // remove guest from state
                stateManager.visitorStateController.RemoveGuest(instanceData.id);

                // remove guest object
                guest = stateManager.visitorStateController.GetGuestBehaviour(guestId);
                if (guest != null) {
                    Destroy(guest.guestObject);
                }
            }

            // remove ship from state
            stateManager.visitorStateController.RemoveShip(instanceData.id);

            // remove ship object
            Destroy(shipObject);
        }
    }
}