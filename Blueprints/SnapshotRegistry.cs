using System;
using System.Collections.Generic;

namespace Blueprints {
    public sealed class SnapshotRegistry {
        private Dictionary<Blueprint, int> references = new Dictionary<Blueprint, int>();

        private const int FLAG_DISPOSED = -1;

        public static SnapshotRegistry Instance { get; private set; }

        public SnapshotRegistry() {
            Instance = this;
        }

        public static void DestroyInstance() {
            Instance = null;
        }

        public void Register(Blueprint blueprint) {
            if (references.ContainsKey(blueprint))
                throw new ArgumentException("Cannot register a blueprint more than once");
            references.Add(blueprint, 0);
        }

        public void AddReferences(Blueprint blueprint, int count) {
            if (!references.ContainsKey(blueprint))
                throw new ArgumentException("Cannot add references to an unregistered blueprint");
            if (references[blueprint] == FLAG_DISPOSED)
                throw new ArgumentException("Cannot add references to a disposed blueprint");
            references[blueprint] += count;
        }

        public void RemoveReference(Blueprint blueprint) {
            if (!references.ContainsKey(blueprint))
                throw new ArgumentException("Cannot add references to an unregistered blueprint");
            if (references[blueprint] == FLAG_DISPOSED)
                throw new ArgumentException("Cannot remove references from a disposed blueprint");
            references[blueprint]--;
            if (references[blueprint] <= 0) {
                blueprint.Dispose();
                references[blueprint] = FLAG_DISPOSED;
            }
        }

        public bool TryDispose(Blueprint blueprint) {
            if (!references.ContainsKey(blueprint))
                return false;
            if (references[blueprint] == 0) {
                blueprint.Dispose();
                references[blueprint] = FLAG_DISPOSED;
                return true;
            }
            return false;
        }
    }
}
