using NUnit.Framework;

namespace BossFight.Combat.Tests
{
    public class StaminaPoolTests
    {
        [Test]
        public void StartsFull()
        {
            var pool = new StaminaPool(100f, 20f, 1f);
            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void SpendReducesCurrent()
        {
            var pool = new StaminaPool(100f, 20f, 1f);
            Assert.IsTrue(pool.TrySpend(30f));
            Assert.AreEqual(70f, pool.Current);
        }

        [Test]
        public void CannotOverspend()
        {
            var pool = new StaminaPool(20f, 20f, 1f);
            Assert.IsFalse(pool.TrySpend(30f));
            Assert.AreEqual(20f, pool.Current, "a refused spend must not change stamina");
        }

        [Test]
        public void ExactCostIsAffordable()
        {
            var pool = new StaminaPool(20f, 20f, 1f);
            Assert.IsTrue(pool.TrySpend(20f));
            Assert.AreEqual(0f, pool.Current);
        }

        [Test]
        public void NoRegenDuringDelay()
        {
            var pool = new StaminaPool(100f, 20f, 1f);
            pool.TrySpend(50f);
            pool.Tick(0.5f);
            Assert.AreEqual(50f, pool.Current);
        }

        [Test]
        public void RegenStartsAfterDelayAndOnlyCountsTimeAfterIt()
        {
            var pool = new StaminaPool(100f, 20f, 1f);
            pool.TrySpend(50f);
            pool.Tick(1.5f);   // 1.0 s of delay, then 0.5 s of regen at 20/s
            Assert.AreEqual(60f, pool.Current, 0.001f);
        }

        [Test]
        public void RegenClampsAtMax()
        {
            var pool = new StaminaPool(100f, 20f, 0f);
            pool.TrySpend(10f);
            pool.Tick(10f);
            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void SpendingAgainRestartsTheDelay()
        {
            var pool = new StaminaPool(100f, 20f, 1f);
            pool.TrySpend(10f);
            pool.Tick(0.8f);
            pool.TrySpend(10f);
            pool.Tick(0.8f);
            Assert.AreEqual(80f, pool.Current, "delay restarted, so no regen should have happened yet");
        }
    }
}
