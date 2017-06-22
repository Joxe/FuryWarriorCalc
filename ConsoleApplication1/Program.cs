using System;

namespace ConsoleApplication1 {
	class Program {
		static Random rng = new Random();
		static float m_testTimeInSeconds = 1000.0f;
		static float m_crit = 0.35f;
		static float m_mhAtkSpd = 2.6f;
		static float m_ohAtkSpd = 1.6f;

		static int m_flurryCharges = 0;
		static float m_nextMh = 0.0f;
		static float m_nextOh = 0.0f;

		static float m_currentTimer = 0.0f;
		static int m_attackCount = 0;
		static int m_flurryCount = 0;
		static float m_enteredFlurryAt = 0.0f;
		static float m_flurryTimer = 0.0f;

		static void UpdateFlurryCount() {
			--m_flurryCharges;
			++m_flurryCount;

			if (m_flurryCharges == 0) {
				m_flurryTimer += m_currentTimer - m_enteredFlurryAt;
			}
		}

		static void OnEnteredFlurry(bool a_mhTrigger) {
			m_enteredFlurryAt = m_currentTimer;
			if (a_mhTrigger) {
				m_nextOh -= (m_nextOh - m_currentTimer) / 1.3f;
			} else {
				m_nextMh -= (m_nextMh - m_currentTimer) / 1.3f;
			}

			if (m_nextOh < m_currentTimer) {
				AttackAndGetNextTimer(m_mhAtkSpd, out m_nextOh);
			}

			if (m_nextMh < m_currentTimer) {
				AttackAndGetNextTimer(m_mhAtkSpd, out m_nextMh);
			}
		}

		static void Main(string[] args) {
			while (m_currentTimer <= m_testTimeInSeconds) {
				if (m_nextMh <= m_currentTimer) {
					++m_attackCount;

					if (m_flurryCharges > 0) {
						UpdateFlurryCount();
					}

					if (AttackAndGetNextTimer(m_mhAtkSpd, out m_nextMh)) {
						OnEnteredFlurry(true);
					}
				} else if (m_nextOh <= m_currentTimer) {
					++m_attackCount;

					if (m_flurryCharges > 0) {
						UpdateFlurryCount();
					}

					if (AttackAndGetNextTimer(m_ohAtkSpd, out m_nextOh)) {
						OnEnteredFlurry(false);
					}
				}

				m_currentTimer = Math.Min(m_nextMh, m_nextOh);
			}

			m_flurryTimer += m_currentTimer - m_enteredFlurryAt;

			Console.WriteLine("Attacks " + m_attackCount + " Flurry Attacks " + m_flurryCount + " (" + ((float)m_flurryCount / (float)m_attackCount) * 100.0f + "%)");
			Console.WriteLine("Total battle time in seconds " + m_testTimeInSeconds + " of which was flurry " + m_flurryTimer + " (" + (m_flurryTimer / m_testTimeInSeconds) * 100.0f + "%)");
			Console.Read();
		}

		static bool AttackAndGetNextTimer(float a_wpnAttackSpeed, out float a_nextAttack) {
			float critRandomNumber = (float)rng.NextDouble();
			bool enteredFlurry = false;

			if (critRandomNumber < m_crit) {
				if (m_flurryCharges == 0) {
					enteredFlurry = true;
				}
				m_flurryCharges = 3;
			}

			if (m_flurryCharges > 0) {
				a_nextAttack = m_currentTimer + (a_wpnAttackSpeed / 1.3f);
			} else {
				a_nextAttack = m_currentTimer + a_wpnAttackSpeed;
			}

			return enteredFlurry;
		}
	}
}
