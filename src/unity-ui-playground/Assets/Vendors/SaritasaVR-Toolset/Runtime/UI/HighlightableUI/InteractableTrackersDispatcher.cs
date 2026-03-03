using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Global dispatcher for all InteractableStateTracker components.
    /// This class is responsible for updating these components and
    /// keeping their states up to date.
    /// </summary>
    public class InteractableTrackersDispatcher : MonoBehaviour
    {
        private static InteractableTrackersDispatcher instance;

        private static bool destroyed;

        private Dictionary<InteractableStateTracker, int> trackersDict = new Dictionary<InteractableStateTracker, int>();

        /// <summary>
        /// Objects are also stored in contiguous array for better iteration speed in Update loop.
        /// </summary>
        [SerializeField]
        private List<InteractableStateTracker> trackersBuffer = new List<InteractableStateTracker>();

        /// <summary>
        /// Get global instance of the class. Will return null if dispatcher already destroyed.
        /// </summary>
        public static InteractableTrackersDispatcher Instance
        {
            get
            {
                if (!UnityEngine.Application.isPlaying)
                {
                    return null;
                }

                if (destroyed)
                {
                    return null;
                }

                if (instance == null)
                {
                    var gameObject = new GameObject("Interactable Trackers Dispatcher (Singleton)");
                    instance = gameObject.AddComponent<InteractableTrackersDispatcher>();
                    DontDestroyOnLoad(gameObject);

                    instance.trackersDict.Clear();
                    instance.trackersBuffer.Clear();
                }

                return instance;
            }
        }

        /// <summary>
        /// Get read only list of all trackables.
        /// </summary>
        public IReadOnlyCollection<InteractableStateTracker> GetAllTrackers()
        {
            return trackersBuffer;
        }

        /// <summary>
        /// Register tracker.
        /// </summary>
        public void Register(InteractableStateTracker tracker)
        {
            int index = AddToTrackerToBuffer(tracker);
            trackersDict.Add(tracker, index);
        }

        /// <summary>
        /// Unregister tracker.
        /// </summary>
        public void Unregister(InteractableStateTracker tracker)
        {
            var id = trackersDict[tracker];
            trackersDict.Remove(tracker);

            RemoveTrackerFromBuffer(id);
        }

        private int AddToTrackerToBuffer(InteractableStateTracker tracker)
        {
            trackersBuffer.Add(tracker);
            return trackersBuffer.Count - 1;
        }

        private void RemoveTrackerFromBuffer(int index)
        {
            int lastIndex = trackersBuffer.Count - 1;

            if (lastIndex > index)
            {
                trackersBuffer[index] = trackersBuffer[lastIndex];
                var movedTracker = trackersBuffer[index];
                trackersDict[movedTracker] = index;
            }

            trackersBuffer.RemoveAt(lastIndex);
        }

        private void Awake()
        {
            if (instance != null)
            {
                throw new System.Exception("Trying to create more than one instance of singleton: Interactable Trackers Dispatcher");
            }
        }

        private void OnDestroy()
        {
            instance = null;
            destroyed = true;
        }

        private void Update()
        {
            for (int i = 0; i < trackersBuffer.Count; i++)
            {
                trackersBuffer[i].UpdateInteractableState();
            }
        }
    }
}