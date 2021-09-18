using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using SST.Data;
    using SST.Gameplay.Visitor;

    public class VisitorStateController : MonoBehaviour
    {
        [SerializeField] private VisitorStateData data;
        private Dictionary<uint, ShipBehaviour> shipBehaviours;
        private Dictionary<uint, GuestBehaviour> guestBehaviours;

        public int ShipCount => shipBehaviours.Count;
        public int GuestCount => guestBehaviours.Count;

        public VisitorStateController() {
            shipBehaviours = new Dictionary<uint, ShipBehaviour>();
            guestBehaviours = new Dictionary<uint, GuestBehaviour>();
            data = new VisitorStateData() {
                guests = new List<GuestData>(),
                ships = new List<ShipData>(),
            };
        }

        public GuestBehaviour GetGuestBehaviour(uint guestId) {
            return guestBehaviours[guestId];
        }

        public ShipBehaviour GetShipBehaviour(uint shipid) {
            return shipBehaviours[shipid];
        }

        public void AddShip(ShipBehaviour ship) {
            int index = data.ships.FindIndex((x) => x.id == ship.instanceData.id);
            if (index == -1) {
                shipBehaviours.Add(ship.instanceData.id, ship);
                data.ships.Add(ship.instanceData);
            }
        }

        public void AddGuest(GuestBehaviour crew) {
            int index = data.guests.FindIndex((x) => x.id == crew.instanceData.id);
            if (index == -1) {
                guestBehaviours.Add(crew.instanceData.id, crew);
                data.guests.Add(crew.instanceData);
            }
        }

        public void RemoveShip(uint shipId) {
            int index = data.ships.FindIndex((x) => x.id == shipId);
            if (index != -1) {
                shipBehaviours.Remove(shipId);
                data.ships.RemoveAt(index);
            }
        }

        public void RemoveGuest(uint guestId) {
            int index = data.guests.FindIndex((x) => x.id == guestId);
            if (index != -1) {
                guestBehaviours.Remove(guestId);
                data.guests.RemoveAt(index);
            }
        }

        public void SetData(VisitorStateData data) {
            this.data = data;
        }
    }
}
