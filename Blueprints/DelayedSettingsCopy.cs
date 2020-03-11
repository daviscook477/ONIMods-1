using UnityEngine;
using System.Collections;

namespace Blueprints {
    /// <summary>
    /// Used as part of the settings copy for newly constructed/instant-placed buildings.
    /// Component that is added onto a newly constructed building. It will copy the settings
    /// on the `settingsSource` field to the building quickly after it finishes construction.
    /// 
    /// Although unlikely, it is possible for a building to be completed, but not yet have
    /// this component finished executing at the time at which a user saves their map.
    /// In this edge case, the settings that would have been copied to the building will
    /// be lost since this component is not serialized.
    /// </summary>
    public sealed class DelayedSettingsCopy : KMonoBehaviour {
        public GameObject settingsSource;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            StartCoroutine("DelayedCopy");
        }

        /// <summary>
        /// Coroutine that delays the execution of the CopySettings trigger by a single frame.
        /// Upon completion it destroys the source of the settings as it is no longer in use.
        /// Also destroys this script as it has nothing left to do after performing this delayed trigger.
        /// </summary>
        /// <returns>Some weird stuff with yield. I don't know how coroutines work.</returns>
        IEnumerator DelayedCopy()
        {
            yield return 0;
            gameObject.Trigger((int) GameHashes.CopySettings, settingsSource);
            Destroy(settingsSource);
        }
    }
}
