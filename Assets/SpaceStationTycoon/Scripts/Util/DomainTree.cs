using System;
using System.Collections.Generic;

namespace SST.DomainTree
{

	public class DomainTree<T>
	{
		private const char delimiter = '.';
		private DomainNode<T> root = null;

		public DomainTree() {
			root = new DomainNode<T>("_root");
		}

		public void Add(string key, T value) {
			string[] parts = key.Split(delimiter);
			int depth = 0;
			string activeKey = parts[depth];

			if (root.child == null) {
				root.child = new DomainNode<T>(activeKey);
			}

			DomainNode<T> next = root.child;
			while (next != null) {
				if (next.key == activeKey) {
					// found domain part or goal
					if (depth == parts.Length - 1) {
						// found domain goal already exists, replace it
						next.value = value;
						next.hasValue = true;
						return;
					} else if (next.child != null) {
						// found domain part with children, continue to child
						next = next.child;
						depth++;
						activeKey = parts[depth];
					} else {
						// found domain part without children, create child
						depth++;
						activeKey = parts[depth];
						DomainNode<T> child = new DomainNode<T>(activeKey);
						next.child = child;
						next = next.child;
					}
				} else {
					// this is not a step or goal node
					if (next.sibling != null) {
						// check the next node
						next = next.sibling;
					} else {
						// last node on this depth, add a new one
						DomainNode<T> sibling = new DomainNode<T>(activeKey);
						next.sibling = sibling;
						next = sibling;

						// if (depth < parts.Length - 1) {
							// depth++;
							// activeKey = parts[depth];
						// }
					}
				}
			}
		}

		public void Clear() {
			root.Clear();
        }

		public bool Remove(string key, bool allowWithChildren = true) {
			if (root.child == null) return false;

			string[] parts = key.Split(delimiter);
			int depth = 0;
			string activeKey = parts[depth];

			DomainNode<T> next = root.child, lastParent = root, lastSibling = null;
			while (next != null) {
				if (next.key == activeKey) {
					if (depth == parts.Length - 1) {
						// this is the goal node
						if (lastSibling != null) {
							lastSibling.sibling = next.sibling;
						}

						lastParent.child = lastSibling;
						return true;
					} else if (next.child != null) {
						// this is a step node, goal may be a child
						lastParent = next;
						lastSibling = null;
						next = next.child;
						depth++;
						activeKey = parts[depth];
					} else {
						// this is a step node, but it does not contain any children
						return false;
					}
				} else {
					// this is not a step or goal node
					lastSibling = next;
					next = next.sibling;
				}
			}


			return false;
		}

		public DomainNode<T> Search(string key, bool allowNoValue = false) {
			if (root.child == null) return null;
			return Search(root.child, key, allowNoValue);
		}

		public DomainNode<T> Search(DomainNode<T> node, string key, bool allowNoValue = false) {
			if (node == null)
				return default;

			string[] parts = key.Split(delimiter);
			int depth = 0;
			string activeKey = parts[depth];

			DomainNode<T> next = node;
			while (next != null) {
				if (next.key == activeKey) {
					if (depth == parts.Length - 1) {
						// this is the goal node
						if (next.hasValue || allowNoValue) return next;
						else return null;
					} else if (next.child != null) {
						// this is a step node, goal may be a child
						next = next.child;
						depth++;
						activeKey = parts[depth];
					} else {
						// this is a step node, but it does not contain any children
						return null;
					}
				} else {
					// this is not a step or goal node
					next = next.sibling;
				}
			}


			return null;
		}

		public List<DomainNode<T>> SearchAllChildren(string key) {
			DomainNode<T> node = Search(key, true);
			if (node == null) return new List<DomainNode<T>>();
			else return GetAllChildren(node);
		}

		public List<DomainNode<T>> GetAllChildren(DomainNode<T> node) {
			List<DomainNode<T>> immediateChildren = GetImmediateChildren(node);
			List<DomainNode<T>> allChildren = new List<DomainNode<T>>(immediateChildren);
			allChildren.Add(node);
			foreach (var childNode in immediateChildren) {
				allChildren.AddRange(GetAllChildren(childNode));
			}

			return allChildren;
		}

		public List<DomainNode<T>> GetImmediateChildren(DomainNode<T> node) {
			List<DomainNode<T>> children = new List<DomainNode<T>>();
			DomainNode<T> next = node.child;
			while (next != null) {
				children.Add(next);
				next = next.sibling;
			}
			return children;
		}

		public void Print() {
			List<string> keys = new List<string>();

			HashSet<DomainNode<T>> explored = new HashSet<DomainNode<T>>();
			Queue<DomainNode<T>> q = new Queue<DomainNode<T>>();
			explored.Add(root);
			q.Enqueue(root);
			while (q.Count > 0) {
				var next = q.Dequeue();
				while (next != null) {
					keys.Add(next.key);

					if (next.child != null && explored.Contains(next.child) == false) {
						q.Enqueue(next.child);
					}

					next = next.sibling;
				}
			}


			for (int i = 0; i < keys.Count; i++) {
				if (i != keys.Count - 1) {
					Console.Write(keys[i] + ", ");
				} else {
					Console.Write(keys[i] + "\n\n");
				}
			}
		}
	}

	public class DomainNode<T>
	{
		public DomainNode<T> child;
		public DomainNode<T> sibling;
		public string key;
		public T value;
		public bool hasValue;

		public DomainNode(string key) {
			this.key = key;
			hasValue = false;
		}

		public DomainNode(string key, T value) {
			this.value = value;
			this.key = key;
			hasValue = true;
		}

		public void Clear() {
			child = null;
			value = default;
			hasValue = false;
        }
	}
}
