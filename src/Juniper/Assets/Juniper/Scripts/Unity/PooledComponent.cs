using System;
using System.Collections.Generic;

using UnityEngine;

namespace Juniper
{
    public struct PooledComponent<T> where T : Component
    {
        public readonly T Value;
        public readonly bool IsNew;

        public PooledComponent(GameObject obj, Predicate<T> predicate, Action<T> onCreate)
        {
            if (predicate == null)
            {
                predicate = (_) => true;
            }

            Value = Array.Find(obj.GetComponents<T>(), predicate);
            IsNew = Value == null;
            if (IsNew)
            {
                Value = obj.AddComponent<T>();
                onCreate?.Invoke(Value);
            }
        }

        public PooledComponent(GameObject obj, bool isNew)
            : this(obj, null, null)
        {
            IsNew |= isNew;
        }

        public PooledComponent(T value, bool isNew)
        {
            Value = value;
            IsNew = isNew;
        }

        public static implicit operator T(PooledComponent<T> obj)
        {
            return obj.Value;
        }

        public void SetActive(bool active)
        {
            Value.SetActive(active);
        }

        public void Destroy()
        {
            UnityEngine.Object.DestroyImmediate(Value);
        }

        // Summary: The tag of this game object.
        public string tag
        {
            get
            {
                return Value.tag;
            }
            set
            {
                Value.tag = value;
            }
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object or
        // any of its children.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // parameter: Optional parameter to pass to the method (can be any value).
        //
        // options: Should an error be raised if the method does not exist for a given target object?
        public void BroadcastMessage(string methodName)
        {
            Value.BroadcastMessage(methodName);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object or
        // any of its children.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // parameter: Optional parameter to pass to the method (can be any value).
        //
        // options: Should an error be raised if the method does not exist for a given target object?
        public void BroadcastMessage(string methodName, object parameter)
        {
            Value.BroadcastMessage(methodName, parameter);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object or
        // any of its children.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // parameter: Optional parameter to pass to the method (can be any value).
        //
        // options: Should an error be raised if the method does not exist for a given target object?
        public void BroadcastMessage(string methodName, object parameter = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
        {
            Value.BroadcastMessage(methodName, parameter, options);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object or
        // any of its children.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // parameter: Optional parameter to pass to the method (can be any value).
        //
        // options: Should an error be raised if the method does not exist for a given target object?
        public void BroadcastMessage(string methodName, SendMessageOptions options)
        {
            Value.BroadcastMessage(methodName, options);
        }

        // Summary: Is this game object tagged with tag ?
        //
        // Parameters: tag: The tag to compare.
        public bool CompareTag(string tag)
        {
            return Value.CompareTag(tag);
        }

        // Summary: Returns the component of Type type if the game object has one attached, null if
        // it doesn't.
        //
        // Parameters: type: The type of Component to retrieve.
        public Component GetComponent(Type type)
        {
            return Value.GetComponent(type);
        }

        public U GetComponent<U>()
        {
            return Value.GetComponent<U>();
        }

        // Summary: Returns the component with name type if the game object has one attached, null if
        // it doesn't.
        //
        // Parameters: type:
        public Component GetComponent(string type)
        {
            return Value.GetComponent(type);
        }

        public U GetComponentInChildren<U>()
        {
            return Value.GetComponentInChildren<U>();
        }

        public U GetComponentInChildren<U>(bool includeInactive = false)
        {
            return Value.GetComponentInChildren<U>(includeInactive);
        }

        public Component GetComponentInChildren(Type t, bool includeInactive)
        {
            return Value.GetComponentInChildren(t, includeInactive);
        }

        // Summary: Returns the component of Type type in the GameObject or any of its children using
        // depth first search.
        //
        // Parameters: t: The type of Component to retrieve.
        //
        // Returns: A component of the matching type, if found.
        public Component GetComponentInChildren(Type t)
        {
            return Value.GetComponentInChildren(t);
        }

        // Summary: Returns the component of Type type in the GameObject or any of its parents.
        //
        // Parameters: t: The type of Component to retrieve.
        //
        // Returns: A component of the matching type, if found.
        public Component GetComponentInParent(Type t)
        {
            return Value.GetComponentInParent(t);
        }

        public U GetComponentInParent<U>()
        {
            return Value.GetComponentInParent<U>();
        }

        public U[] GetComponents<U>()
        {
            return Value.GetComponents<U>();
        }

        public void GetComponents<U>(List<U> results)
        {
            Value.GetComponents(results);
        }

        public void GetComponents(Type type, List<Component> results)
        {
            Value.GetComponents(type, results);
        }

        // Summary: Returns all components of Type type in the GameObject.
        //
        // Parameters: type: The type of Component to retrieve.
        public Component[] GetComponents(Type type)
        {
            return Value.GetComponents(type);
        }

        public void GetComponentsInChildren<U>(List<U> results)
        {
            Value.GetComponentsInChildren(results);
        }

        public void GetComponentsInChildren<U>(bool includeInactive, List<U> result)
        {
            Value.GetComponentsInChildren(includeInactive, result);
        }

        public Component[] GetComponentsInChildren(Type t)
        {
            return Value.GetComponentsInChildren(t);
        }

        // Summary: Returns all components of Type type in the GameObject or any of its children.
        //
        // Parameters: t: The type of Component to retrieve.
        //
        // includeInactive: Should Components on inactive GameObjects be included in the found set?
        // includeInactive decides which children of the GameObject will be searched. The GameObject
        // that you call GetComponentsInChildren on is always searched regardless.
        public Component[] GetComponentsInChildren(Type t, bool includeInactive)
        {
            return Value.GetComponentsInChildren(t, includeInactive);
        }

        public U[] GetComponentsInChildren<U>()
        {
            return Value.GetComponentsInChildren<U>();
        }

        public U[] GetComponentsInChildren<U>(bool includeInactive)
        {
            return Value.GetComponentsInChildren<U>(includeInactive);
        }

        public U[] GetComponentsInParent<U>()
        {
            return Value.GetComponentsInParent<U>();
        }

        public Component[] GetComponentsInParent(Type t)
        {
            return Value.GetComponentsInParent(t);
        }

        public void GetComponentsInParent<U>(bool includeInactive, List<U> results)
        {
            Value.GetComponentsInParent(includeInactive, results);
        }

        public U[] GetComponentsInParent<U>(bool includeInactive)
        {
            return Value.GetComponentsInParent<U>(includeInactive);
        }

        // Summary: Returns all components of Type type in the GameObject or any of its parents.
        //
        // Parameters: t: The type of Component to retrieve.
        //
        // includeInactive: Should inactive Components be included in the found set?
        public Component[] GetComponentsInParent(Type t, bool includeInactive = false)
        {
            return Value.GetComponentsInParent(t, includeInactive);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // value: Optional parameter for the method.
        //
        // options: Should an error be raised if the target object doesn't implement the method for
        // the message?
        public void SendMessage(string methodName, SendMessageOptions options)
        {
            Value.SendMessage(methodName, options);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // value: Optional parameter for the method.
        //
        // options: Should an error be raised if the target object doesn't implement the method for
        // the message?
        public void SendMessage(string methodName, object value, SendMessageOptions options)
        {
            Value.SendMessage(methodName, value, options);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // value: Optional parameter for the method.
        //
        // options: Should an error be raised if the target object doesn't implement the method for
        // the message?
        public void SendMessage(string methodName)
        {
            Value.SendMessage(methodName);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object.
        //
        // Parameters: methodName: Name of the method to call.
        //
        // value: Optional parameter for the method.
        //
        // options: Should an error be raised if the target object doesn't implement the method for
        // the message?
        public void SendMessage(string methodName, object value)
        {
            Value.SendMessage(methodName, value);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object and
        // on every ancestor of the behaviour.
        //
        // Parameters: methodName: Name of method to call.
        //
        // value: Optional parameter value for the method.
        //
        // options: Should an error be raised if the method does not exist on the target object?
        public void SendMessageUpwards(string methodName, SendMessageOptions options)
        {
            Value.SendMessageUpwards(methodName, options);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object and
        // on every ancestor of the behaviour.
        //
        // Parameters: methodName: Name of method to call.
        //
        // value: Optional parameter value for the method.
        //
        // options: Should an error be raised if the method does not exist on the target object?
        public void SendMessageUpwards(string methodName, object value)
        {
            Value.SendMessageUpwards(methodName, value);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object and
        // on every ancestor of the behaviour.
        //
        // Parameters: methodName: Name of method to call.
        //
        // value: Optional parameter value for the method.
        //
        // options: Should an error be raised if the method does not exist on the target object?
        public void SendMessageUpwards(string methodName, object value = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
        {
            Value.SendMessageUpwards(methodName, value, options);
        }

        // Summary: Calls the method named methodName on every MonoBehaviour in this game object and
        // on every ancestor of the behaviour.
        //
        // Parameters: methodName: Name of method to call.
        //
        // value: Optional parameter value for the method.
        //
        // options: Should an error be raised if the method does not exist on the target object?
        public void SendMessageUpwards(string methodName)
        {
            Value.SendMessageUpwards(methodName);
        }

        public Transform transform
        {
            get
            {
                return Value.transform;
            }
        }

        public GameObject gameObject
        {
            get
            {
                return Value.gameObject;
            }
        }

        public PooledComponent<U> Ensure<U>()
            where U : Component
        {
            return Value.Ensure<U>();
        }

        public PooledComponent<U> Ensure<U>(Predicate<U> predicate)
            where U : Component
        {
            return Value.Ensure(predicate);
        }

        public PooledComponent<U> Ensure<U>(string path)
            where U : Transform
        {
            return Value.Ensure<U>(path);
        }

        public PooledComponent<U> Ensure<U>(string path, string creationPath = null)
            where U : Transform
        {
            return Value.Ensure<U>(path, creationPath);
        }

        public PooledComponent<U> Ensure<U>(string path, Func<GameObject> create)
            where U : Transform
        {
            return Value.Ensure<U>(path, null, create);
        }

        public PooledComponent<U> Ensure<U>(string path, string creationPath, Func<GameObject> create)
            where U : Transform
        {
            return Value.Ensure<U>(path, creationPath, create);
        }

        public PooledComponent<Transform> EnsureParent(string name, params Transform[] exclusionList)
        {
            return Value.EnsureParent(name, exclusionList);
        }

        public U Query<U>(string path)
        {
            return Value.Query<U>(path);
        }

        public Transform Query(string path)
        {
            return Value.Query<Transform>(path);
        }

        public Transform Find(string name)
        {
            return Value.transform.Find(name);
        }

        public PooledComponent<T> SetScale(Vector3 s)
        {
            Value.SetScale(s);
            return this;
        }

        public PooledComponent<T> SetAnchors(Vector2 anchorMin, Vector2 anchorMax)
        {
            Value.SetAnchors(anchorMin, anchorMax);
            return this;
        }

        public PooledComponent<T> SetPivot(Vector2 pivot)
        {
            Value.SetPivot(pivot);
            return this;
        }

        public PooledComponent<T> SetPosition(Vector3 position)
        {
            Value.SetPosition(position);
            return this;
        }

        public PooledComponent<T> SetWidth(float width)
        {
            Value.SetWidth(width);
            return this;
        }

        public PooledComponent<T> SetHeight(float height)
        {
            Value.SetHeight(height);
            return this;
        }

        public PooledComponent<T> SetSize(float width, float height)
        {
            Value.SetSize(width, height);
            return this;
        }

        public PooledComponent<T> SetSize(Vector2 sizeDelta)
        {
            Value.SetSize(sizeDelta);
            return this;
        }

        public void Deactivate()
        {
            Value.Deactivate();
        }

        public void Activate()
        {
            Value.Activate();
        }
    }
}
