using NUnit.Framework;

namespace BossFight.Combat.Tests
{
    public class HealthPoolTests
    {
        [Test]
        public void StartsFullAndAlive()
        {
            var pool = new HealthPool(100f);
            Assert.AreEqual(100f, pool.Current);
            Assert.IsFalse(pool.IsDead);
        }

        [Test]
        public void DamageReducesCurrent()
        {
            var pool = new HealthPool(100f);
            Assert.IsFalse(pool.Damage(30f), "not a killing blow");
            Assert.AreEqual(70f, pool.Current);
        }

        [Test]
        public void KillingBlowReportsDeathOnceAndClampsAtZero()
        {
            var pool = new HealthPool(50f);
            Assert.IsTrue(pool.Damage(80f));
            Assert.AreEqual(0f, pool.Current);
            Assert.IsTrue(pool.IsDead);
            Assert.IsFalse(pool.Damage(10f), "already dead, must not report death again");
        }

        [Test]
        public void ZeroOrNegativeDamageIsIgnored()
        {
            var pool = new HealthPool(100f);
            pool.Damage(0f);
            pool.Damage(-5f);
            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void RefillRestoresMax()
        {
            var pool = new HealthPool(100f);
            pool.Damage(100f);
            pool.Refill();
            Assert.AreEqual(100f, pool.Current);
            Assert.IsFalse(pool.IsDead);
        }
    }
}
