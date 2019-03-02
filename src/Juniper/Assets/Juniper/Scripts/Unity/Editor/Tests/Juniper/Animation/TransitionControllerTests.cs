using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Juniper.Animation
{
    public class TransitionControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            go = Utils.CreatePrimitive(PrimitiveType.Cube);
            go.Deactivate();
            trans = go.AddComponent<MockTransition>();
            trans.enabled = false;

            Assert.AreEqual(TRANS_START_VALUE, trans.Value, "Start Value");
        }

        [TearDown]
        public void TearDown()
        {
            trans.Destroy();
            trans = null;

            go.Destroy();
            go = null;
        }

        [Test]
        public void EnterStartsAtZero()
        {
            trans.Enter();
            Assert.AreEqual(0, trans.Value);
        }

        [Test]
        public void EnterStartsValueChanged()
        {
            trans.Enter();
            Assert.AreEqual(1, trans.ValueChanges.Count);
        }

        [UnityTest]
        public IEnumerator EnterEndsAtOne()
        {
            yield return Enter();
            Assert.AreEqual(1, trans.Value);
        }

        [UnityTest]
        public IEnumerator EnterEndsWithMoreValuesChanged()
        {
            yield return Enter();
            Assert.Greater(trans.ValueChanges.Count, 1);
        }

        [Test]
        public void SetEnabledStartsAtZero()
        {
            go.Activate();
            trans.enabled = true;
            Assert.AreEqual(0, trans.Value);
        }

        [UnityTest]
        public IEnumerator SetEnabledEndsAtOne()
        {
            yield return SetEnabled();
            Assert.AreEqual(1, trans.Value);
        }

        [Test]
        public void ExitStartsAtOne()
        {
            go.Activate();
            trans.enabled = true;
            trans.Exit();
            Assert.AreEqual(1, trans.Value);
        }

        [UnityTest]
        public IEnumerator ExitEndsAtZero()
        {
            go.Activate();
            trans.enabled = true;
            yield return Exit();
            Assert.AreEqual(0, trans.Value);
        }

        [UnityTest]
        public IEnumerator ExitDoesntCompleteRightAway()
        {
            yield return Enter();
            trans.Exit();
            for (var i = 0; i < 10; ++i)
            {
                Assert.IsFalse(trans.IsComplete, $"Not complete {i}");
                yield return null;
            }
            yield return Waiter();
            Assert.IsTrue(trans.IsComplete, "Complete");
        }

        [UnityTest]
        public IEnumerator ExitEndsWithMoreValuesChanged()
        {
            trans.enabled = true;
            go.Activate();
            trans.Exit();
            yield return Waiter();
            Assert.Greater(trans.ValueChanges.Count, 1, "Many changes");
        }

        [UnityTest]
        public IEnumerator ExitDoesntActivate()
        {
            yield return Exit();
            Assert.IsFalse(go.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator ExitDoesntEnable()
        {
            yield return Exit();
            Assert.IsFalse(trans.enabled);
        }

        [UnityTest]
        public IEnumerator ExitDisabledDoesntChangeValue()
        {
            yield return Exit();
            Assert.AreEqual(TRANS_START_VALUE, trans.Value);
        }

        [Test]
        public void EnterActivatesObject()
        {
            trans.Enter();
            Assert.IsTrue(go.activeInHierarchy);
        }

        [Test]
        public void EnterEnablesObject()
        {
            trans.Enter();
            Assert.IsTrue(trans.enabled);
        }

        [Test]
        public void EnterFiresEntering()
        {
            Assert.IsFalse(trans.FiredEntering);
            trans.Enter();
            Assert.IsTrue(trans.FiredEntering);
        }

        [UnityTest]
        public IEnumerator EnterFiresEntered()
        {
            Assert.IsFalse(trans.FiredEntered);
            yield return Enter();
            Assert.IsTrue(trans.FiredEntered);
        }

        [UnityTest]
        public IEnumerator EnterDoesntFireExiting()
        {
            Assert.IsFalse(trans.FiredExiting);
            yield return Enter();
            Assert.IsFalse(trans.FiredExiting);
        }

        [UnityTest]
        public IEnumerator EnterDoesntFireExited()
        {
            Assert.IsFalse(trans.FiredExited);
            yield return Enter();
            Assert.IsFalse(trans.FiredExited);
        }

        [UnityTest]
        public IEnumerator EnterCompletesAsynchronously()
        {
            yield return Enter();
            Assert.IsTrue(trans.IsComplete);
        }

        [UnityTest]
        public IEnumerator EnterDoesntDisable()
        {
            yield return Enter();
            Assert.IsTrue(trans.enabled);
        }

        [UnityTest]
        public IEnumerator EnterDoesntDeactivate()
        {
            yield return Enter();
            Assert.IsTrue(go.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator ExitFiresExiting()
        {
            yield return Enter();
            Assert.IsFalse(trans.FiredExiting);
            trans.Exit();
            Assert.IsTrue(trans.FiredExiting);
        }

        [UnityTest]
        public IEnumerator ExitFiresExited()
        {
            yield return Enter();
            Assert.IsFalse(trans.FiredExited);
            yield return Exit();
            Assert.IsTrue(trans.FiredExited);
        }

        [UnityTest]
        public IEnumerator ExitDoesntFireEntering()
        {
            yield return Enter();
            yield return Exit();
            Assert.IsFalse(trans.FiredEntering);
        }

        [UnityTest]
        public IEnumerator ExitDoesntFireEntered()
        {
            yield return Enter();
            yield return Exit();
            Assert.IsFalse(trans.FiredEntered);
        }

        [UnityTest]
        public IEnumerator ExitCompletesAsynchronously()
        {
            yield return Enter();
            yield return Exit();
            Assert.IsTrue(trans.IsComplete);
        }

        [UnityTest]
        public IEnumerator ExitDisables()
        {
            yield return Enter();
            yield return Exit();
            Assert.IsFalse(trans.enabled);
        }

        [UnityTest]
        public IEnumerator ExitDoesntDeactivate()
        {
            yield return Enter();
            yield return Exit();
            Assert.IsTrue(go.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator SetEnabledFiresEntering()
        {
            Assert.IsFalse(trans.FiredEntering);
            yield return SetEnabled();
            Assert.IsTrue(trans.FiredEntering);
        }

        [UnityTest]
        public IEnumerator SetEnabledFiresEntered()
        {
            Assert.IsFalse(trans.FiredEntered);
            yield return SetEnabled();
            Assert.IsTrue(trans.FiredEntered);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntFireExiting()
        {
            Assert.IsFalse(trans.FiredExiting);
            yield return SetEnabled();
            Assert.IsFalse(trans.FiredExiting);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntFireExited()
        {
            Assert.IsFalse(trans.FiredExited);
            yield return SetEnabled();
            Assert.IsFalse(trans.FiredExited);
        }

        [UnityTest]
        public IEnumerator SetEnabledCompletesAsynchronously()
        {
            yield return SetEnabled();
            Assert.IsTrue(trans.IsComplete);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntDisable()
        {
            yield return SetEnabled();
            Assert.IsTrue(trans.enabled);
        }

        [UnityTest]
        public IEnumerator SetEnabledDoesntDeactivate()
        {
            yield return SetEnabled();
            Assert.IsTrue(go.activeInHierarchy);
        }

        private const float TRANS_TIME = 0.2f;
        private const float TRANS_START_VALUE = 0.5f;

        private GameObject go;

        private MockTransition trans;

        private IEnumerator SetEnabled()
        {
            go.Activate();
            trans.enabled = true;
            return Waiter();
        }

        private WaitUntil Waiter() => new WaitUntil(() =>
            trans.IsComplete);

        private IEnumerator Enter()
        {
            trans.Enter();
            return Waiter();
        }

        private IEnumerator Exit()
        {
            trans.Exit();
            return Waiter();
        }

        private class MockTransition : AbstractTransitionController
        {
            public float Value = TRANS_START_VALUE;
            public List<float> ValueChanges = new List<float>();
            public override float TransitionLength =>
                TRANS_TIME;

            public bool FiredEntering { get; private set; }

            public bool FiredEntered { get; private set; }

            public bool FiredExiting { get; private set; }

            public bool FiredExited { get; private set; }

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
                onValueChanged.AddListener(() =>
                    ValueChanges.Add(Value));
            }

            public override void Exit()
            {
                FiredEntering = FiredEntered = FiredExiting = FiredExited = false;
                ValueChanges.Clear();
                base.Exit();
            }

            protected override void RenderValue(float value) =>
                Value = value;

            protected override void OnEnabled()
            {
                FiredEntering = FiredEntered = FiredExiting = FiredExited = false;
                ValueChanges.Clear();
                base.OnEnabled();
            }
        }
    }
}
