using System;

namespace ConsoleApplication1 {
	class Program {
		static Random rng = new Random();
		const float TEST_TIME_IN_SECONDS = 1000.0f;
		const float CRIT_CHANCE          = 0.25f;
		const float MH_ATTACK_SPEED      = 2.6f;
		const float OH_ATTACK_SPEED      = 1.6f;
		const float MISS_CHANCE          = 0.28f;
		const float DODGE_CHANCE         = 0.05f;
		const float FLURRY_DURATION      = 15.0f;
		const float FLURRY_MULTIPLIER    = 1.3f;
		const int   MAX_FLURRY_CHARGES   = 3;

		static int   m_flurryCharges = 0;
		static float m_nextMh        = 0.0f;
		static float m_nextOh        = 0.0f;

		static float m_currentTimer    = 0.0f;
		static float m_enteredFlurryAt = 0.0f;
		static float m_flurryTimer     = 0.0f;

		static int m_attackCount = 0;
		static int m_flurryCount = 0;
		static int m_missCount   = 0;
		static int m_dodgeCount  = 0;
		static int m_critCount   = 0;

		static void UpdateFlurryCount() {
			--m_flurryCharges;
			++m_flurryCount;

			if (m_flurryCharges == 0) {
				OnExitedFlurry();
				m_flurryTimer += m_currentTimer - m_enteredFlurryAt;
			}
		}

		static void OnEnteredFlurry(bool a_mhTrigger) {
			m_enteredFlurryAt = m_currentTimer;

			if (a_mhTrigger) {
				m_nextOh -=  (m_nextOh - m_currentTimer)-((m_nextOh - m_currentTimer) / FLURRY_MULTIPLIER);
			} else {
				m_nextMh -=  (m_nextMh - m_currentTimer)-((m_nextMh - m_currentTimer) / FLURRY_MULTIPLIER);
			}

			if (m_nextOh < m_currentTimer) {
				AttackAndGetNextTimer(MH_ATTACK_SPEED, out m_nextOh);
			}

			if (m_nextMh < m_currentTimer) {
				AttackAndGetNextTimer(MH_ATTACK_SPEED, out m_nextMh);
			}
		}

		static void OnExitedFlurry() {
			m_nextMh -= (m_nextMh - m_currentTimer) * FLURRY_MULTIPLIER;
			m_nextOh -= (m_nextOh - m_currentTimer) * FLURRY_MULTIPLIER;
		}

		static void Main(string[] args) {
			while (m_currentTimer <= TEST_TIME_IN_SECONDS) {
				if (m_flurryCharges > 0 && m_currentTimer == m_enteredFlurryAt + FLURRY_DURATION) {
					m_flurryCharges = 0;
					OnExitedFlurry();
				}

				if (m_nextMh <= m_currentTimer) {
					++m_attackCount;

					if (AttackAndGetNextTimer(MH_ATTACK_SPEED, out m_nextMh)) {
						OnEnteredFlurry(true);
					}
				} else if (m_nextOh <= m_currentTimer) {
					++m_attackCount;

					if (AttackAndGetNextTimer(OH_ATTACK_SPEED, out m_nextOh)) {
						OnEnteredFlurry(false);
					}
				}

				if (m_flurryCharges > 0) {
					m_currentTimer = Math.Min(Math.Min(m_nextMh, m_nextOh), m_enteredFlurryAt + FLURRY_DURATION);
				} else {
					m_currentTimer = Math.Min(m_nextMh, m_nextOh);
				}
			}

			m_flurryTimer += m_currentTimer - m_enteredFlurryAt;

			Console.WriteLine("Attacks " + m_attackCount + " Flurry Attacks " + m_flurryCount + " (" + ((float)m_flurryCount / (float)m_attackCount) * 100.0f + "%)");
			Console.WriteLine("Total battle time in seconds " + TEST_TIME_IN_SECONDS + " of which was flurry " + m_flurryTimer + " (" + (m_flurryTimer / TEST_TIME_IN_SECONDS) * 100.0f + "%)");
			Console.WriteLine("Attack Count: " + m_attackCount);
			Console.WriteLine("Flurry Count: " + m_flurryCount);
			Console.WriteLine("Miss Count: " + m_missCount);
			Console.WriteLine("Dodge Count: " + m_dodgeCount);
			Console.WriteLine("Crit Count: " + m_critCount);
			Console.Read();
		}

		static bool AttackAndGetNextTimer(float a_wpnAttackSpeed, out float a_nextAttack) {
			float randomNumber = (float)rng.NextDouble();
			bool enteredFlurry = false;

			if (randomNumber < DODGE_CHANCE) {
				//Dodge!
				++m_dodgeCount;
			} else if (randomNumber < DODGE_CHANCE + MISS_CHANCE) {
				//Miss!
				++m_missCount;
			} else {
				if (m_flurryCharges > 0) {
					UpdateFlurryCount();
				}

				if (randomNumber < CRIT_CHANCE + DODGE_CHANCE + MISS_CHANCE) {
					//Crit!
					++m_critCount;

					if (m_flurryCharges == 0) {
						enteredFlurry = true;
					}
					m_flurryCharges = MAX_FLURRY_CHARGES;
				}
			}

			if (m_flurryCharges > 0) {
				a_nextAttack = m_currentTimer + (a_wpnAttackSpeed / FLURRY_MULTIPLIER);
			} else {
				a_nextAttack = m_currentTimer + a_wpnAttackSpeed;
			}

			return enteredFlurry;
		}
	}
}
