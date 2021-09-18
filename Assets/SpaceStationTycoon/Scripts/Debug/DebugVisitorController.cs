using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay
{
    using Navigation;
    using Building;
    using Data;
    using Controllers;
    using Modules;
    using Visitor;

    public class DebugVisitorController : MonoBehaviour
    {
        public StateManager stateManager;
        public BuildingGrid buildingGrid;
        public SpawningController spawningController;

        public ModuleTemplateData dockTemplate;
        public ModuleTemplateData pathTemplate;

        public Vector3 gridMaxWorld = Vector3.one;
        public Vector3 gridMinWorld = Vector3.one;

        public int maxShips = 1;

        public int minGuestsPerShip = 2;
        public int maxGuestsPerShip = 4;

        public string guestTargetModuleDomain = "module";
        public string dockModuleDomain = "module.dock";

        // Update is called once per frame
        public void Update() {
            if (stateManager.visitorStateController.ShipCount < maxShips) {
                TryCreateVisitor();
            }
        }

        void TryCreateVisitor() {
            ModuleBehaviour dock = SelectRandomUnoccupiedDock();
            if (dock == null) return;

            int guestCount = Random.Range(minGuestsPerShip, maxGuestsPerShip);
            SpawnShip(guestCount, dock);
        }

        ShipBehaviour SpawnShip(int guestCount, ModuleBehaviour dockModule) {
            Vector3 dir = (gridMaxWorld - gridMinWorld).normalized;
            Vector3 position = gridMinWorld + (dir * 10f) + (Vector3.up * 10f);
            GameObject ship = Instantiate(spawningController.shipPrefab, position, Quaternion.identity);
            ShipBehaviour shipBehaviour = ship.GetComponent<ShipBehaviour>();
            shipBehaviour.edgeWorldPosition = position;
            shipBehaviour.isDocked = false;
            shipBehaviour.hasDocked = false;
            shipBehaviour.dockModule = dockModule;
            shipBehaviour.dockWorldPosition = buildingGrid.GridToWorldSpace((Vector2Int)dockModule.instanceData.position);
            shipBehaviour.stateManager = stateManager;
            shipBehaviour.instanceData = new ShipData() {
                id = stateManager.idStateController.GetUniqueId(),
                currentFuel = 10,
                maxFuel = 10,
                fuelType = 1,
                currentRepair = 10,
                maxRepair = 10,
                repairType = 1
            };
            stateManager.visitorStateController.AddShip(shipBehaviour);
            shipBehaviour.guestIds = new List<uint>();

            for (int i = 0; i < guestCount; i++) {
                SpawnGuest(shipBehaviour);
            }

            return shipBehaviour;
        }

        GuestBehaviour SpawnGuest(ShipBehaviour ship) {
            GameObject guest = Instantiate(spawningController.guestPrefab);
            GuestBehaviour guestBehaviour = guest.GetComponent<GuestBehaviour>();
            guestBehaviour.shipBehaviour = ship;
            guestBehaviour.onShip = true;
            guestBehaviour.hasNeed = true;
            guestBehaviour.stateManager = stateManager;
            guestBehaviour.navAgent.buildGrid = buildingGrid;
            guestBehaviour.instanceData = new GuestData() {
                id = stateManager.idStateController.GetUniqueId(),
                shipId = ship.instanceData.id,
                goods = 1,
                credits = 1,
                energy = 1,
                social = 1,
                hunger = 1,
                fun = 1,
                safety = 1,
                comfort = 1
            };
            guestBehaviour.needModuleDomain = guestTargetModuleDomain;
            stateManager.visitorStateController.AddGuest(guestBehaviour);
            ship.guestIds.Add(guestBehaviour.instanceData.id);
            guest.SetActive(false);
            return guestBehaviour;
        }

        ModuleBehaviour SelectRandomUnoccupiedDock() {
            // get list of all docks
            List<uint> openDockIds = new List<uint>();

            // filter list to all docks who do not have a ship on them
            foreach (ModuleData dock in stateManager.moduleStateController.FindModules(dockModuleDomain)) {
                if (dock.occupants.Count == 0) {
                    openDockIds.Add(dock.id);
                }
            }
    
             // no docks available
            if (openDockIds.Count == 0) return null;

            // select a random dock
            int rngDockIndex = Random.Range(0, openDockIds.Count);
            ModuleBehaviour dockBehaviour = stateManager.moduleStateController.GetModuleBehaviour(openDockIds[rngDockIndex]);

            return dockBehaviour;
        }
    }
}