using NUnit.Framework;

using System.Collections;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Juniper
{
    public class StateControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            go = Utils.CreatePrimitive(PrimitiveType.Cube);
            go.Deactivate();

            stater = go.AddComponent<MockStateController>();
            stater.enabled = false;
        }

        [TearDown]
        public void TearDown()
        {
            stater.Destroy();
            stater = null;

            go.Destroy();
            go = null;
        }

        [Test]
        public void EnterActivatesObject()
        {
            stater.Enter();

            Assert.IsTrue(go.activeInHierarchy);
        }

        [Test]
        public void EnterEnablesObject()
        {
            stater.Enter();

            Assert.IsTrue(stater.enabled);
        }

        [Test]
        public void EnterFiresEntering()
        {
            Assert.IsFalse(stater.FiredEntering);

            stater.Enter();
            Assert.IsTrue(stater.FiredEntering);
        }

        [UnityTest]
        public IEnumerator EnterFiresEntered()
        {
            Assert.IsFalse(stater.FiredEntered);

            stater.Enter();
            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.FiredEntered);
        }

        [Test]
        public void EnterDoesntFireExiting()
        {
            Assert.IsFalse(stater.FiredExiting);

            stater.Enter();

            Assert.IsFalse(stater.FiredExiting);
        }

        [UnityTest]
        public IEnumerator EnterDoesntFireExited()
        {
            Assert.IsFalse(stater.FiredExited);

            stater.Enter();
            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsFalse(stater.FiredExited);
        }

        [UnityTest]
        public IEnumerator EnterCompletesAsynchronously()
        {
            stater.Enter();
            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.IsComplete);
        }

        [UnityTest]
        public IEnumerator EnterDoesntDisable()
        {
            stater.Enter();

            yield return null;

            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.enabled);
        }

        [UnityTest]
        public IEnumerator EnterDoesntDeactivate()
        {
            stater.Enter();

            yield return null;

            stater.Complete();

            yield return stater.Waiter;

            Assert.IsTrue(go.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator ExitFiresExiting()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.FiredExiting);

            stater.Exit();
            Assert.IsTrue(stater.FiredExiting);

            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
        }

        [UnityTest]
        public IEnumerator ExitFiresExited()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.FiredExited);

            stater.Exit();
            Assert.IsFalse(stater.FiredExited);

            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.FiredExited);
        }

        [UnityTest]
        public IEnumerator ExitDoesntFireEntering()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            stater.Exit();
            Assert.IsFalse(stater.FiredEntering);

            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsFalse(stater.FiredEntering);
        }

        [UnityTest]
        public IEnumerator ExitDoesntFireEntered()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            stater.Exit();
            Assert.IsFalse(stater.FiredEntered);

            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsFalse(stater.FiredEntered);
        }

        [UnityTest]
        public IEnumerator ExitCompletesAsynchronously()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            stater.Exit();
            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.IsComplete);
        }

        [UnityTest]
        public IEnumerator ExitDisables()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            stater.Exit();
            yield return null;
            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsFalse(stater.enabled);
        }

        [UnityTest]
        public IEnumerator ExitDoesntDeactivate()
        {
            stater.Enter();
            stater.Complete();
            yield return stater.Waiter;

            stater.Exit();
            yield return null;
            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(go.activeInHierarchy);
        }

        [Test]
        public void SetEnabledFiresEntering()
        {
            go.Activate();
            Assert.IsFalse(stater.FiredEntering);

            stater.enabled = true;
            Assert.IsTrue(stater.FiredEntering);
        }

        [UnityTest]
        public IEnumerator SetEnabledFiresEntered()
        {
            go.Activate();
            Assert.IsFalse(stater.FiredEntered);

            stater.enabled = true;
            yield return null;
            stater.Complete();
            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.FiredEntered);
        }

        [Test]
        public void SetEnabledDoesntFireExiting()
        {
            go.Activate();
            Assert.IsFalse(stater.FiredExiting);

            stater.enabled = true;
            Assert.IsFalse(stater.FiredExiting);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntFireExited()
        {
            go.Activate();

            Assert.IsFalse(stater.FiredExited);

            stater.enabled = true;

            yield return null;

            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsFalse(stater.FiredExited);
        }

        [UnityTest]
        public IEnumerator SetEnabledCompletesAsynchronously()
        {
            go.Activate();
            stater.enabled = true;

            yield return null;

            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.IsComplete);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntDisable()
        {
            go.Activate();
            stater.enabled = true;

            yield return null;

            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(stater.enabled);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntDeactivate()
        {
            go.Activate();
            stater.enabled = true;

            yield return null;

            stater.Complete();

            yield return stater.Waiter;

            Assert.IsFalse(stater.IsTimedOut, "timed out");
            Assert.IsTrue(go.activeInHierarchy);
        }

        private GameObject go;

        private MockStateController stater;

        private class MockStateController : AbstractStateController
        {
            public bool FiredEntering
            {
                get; private set;
            }

            public bool FiredEntered
            {
                get; private set;
            }

            public bool FiredExiting
            {
                get; private set;
            }

            public bool FiredExited
            {
                get; private set;
            }

            public void Awake()
            {
                onEntering.AddListener(() =>
                    FiredEntering = true);
                onEntered.AddListener(() =>
                    FiredEntered = true);
                onExiting.AddListener(() =>
                    FiredExiting = true);
                onExited.AddListener(() =>
                    FiredExited = true);
            }

            private float startTime;

            public bool IsTimedOut
            {
                get
                {
                    return Time.time - startTime > 1;
                }
            }

            public override bool IsComplete
            {
                get
                {
                    return base.IsComplete || IsTimedOut;
                }
            }

            public void Complete()
            {
                state = STOPPED;
            }

            public override void Exit()
            {
                startTime = Time.time;
                FiredEntering = FiredEntered = FiredExiting = FiredExited = false;
                base.Exit();
            }

            protected override void OnEnabled()
            {
                startTime = Time.time;
                FiredEntering = FiredEntered = FiredExiting = FiredExited = false;
                base.OnEnabled();
            }
        }
    }
}