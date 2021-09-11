using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Modules {
    using SST.Data;

    public abstract class ModuleBehaviour : MonoBehaviour
    {
        public ModuleTemplateData templateData;
        public ModuleData instanceData;

        public virtual void OnBuildModule() {

        }

        public virtual void OnActivateModule() {

        }

        public virtual void OnDestroyModule() {

        }

        public virtual void OnDeactivateModule() {

        }

        public void OnDrawGizmosSelected() {
            Vector3 size = new Vector3(templateData.size.y, 1f, templateData.size.x);
            Vector3 center = new Vector3((templateData.size.y / 2f) - 0.5f, 0.5f, (templateData.size.x / 2f) - 0.5f);
            Gizmos.DrawWireCube(center, size);

            size = Vector3.one * 0.5f;
            center = new Vector3(templateData.portal.point.y, 0.5f, templateData.portal.point.x);
            Gizmos.DrawWireCube(center, size);
        }
    }
}