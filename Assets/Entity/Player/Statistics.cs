using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    [Header("Values")]
    public Vector3 baseHealth;
    public Vector3 baseATP;
    public float baseSpeed;

    private Regenerable health;
    public Regenerable Health() { return health; }
    private Regenerable atp;
    public Regenerable ATP() { return atp; }
    private Statistic speed;
    public Statistic Speed() { return speed; }

    void Start()
    {
        health = new Regenerable(baseHealth.x, baseHealth.y, baseHealth.z);
        atp = new Regenerable(baseATP.x, baseATP.y, baseATP.z);
        speed = new Statistic(baseSpeed);
    }

    void FixedUpdate() {
        Regenerate(health);
        Regenerate(atp);
        Calculate(speed);
    }

    void Regenerate(Regenerable statistic) {
        statistic.Calculate();
        statistic.regeneration.Calculate();
        statistic.value = Mathf.Clamp(statistic.value + statistic.regeneration.CalculatedValue(), 0, statistic.CalculatedValue());
    }

    void Calculate(Statistic statistic) {
        statistic.Calculate();
    }

    public class Regenerable : Statistic {
        public Statistic regeneration;
        public float maxValue;

        public Regenerable(float value, float maxValue, float regeneration) : base(value) {
            this.maxValue = maxValue;
            this.regeneration = new Statistic(regeneration);
        }

        protected override float CalculationValue() {
            return maxValue;
        }
    }

    public class Statistic {
        public List<Modifier> modifiers;
        public float value;

        public Statistic(float value) {
            this.value = value;
            this.modifiers = new List<Modifier>();
        }

        public void AddModifier(Modifier modifier) {
            this.modifiers.Add(modifier);
        }

        protected virtual float CalculationValue() {
            return value;
        }

        private float calculatedValue;
        public float CalculatedValue() {
            return calculatedValue;
        }

        public void Calculate() {
            float calc = CalculationValue();
            foreach (Modifier modifier in modifiers) {
                if (modifier.op == Modifier.Operator.ADDITION) {
                    calc += modifier.value;
                }
            }
            foreach (Modifier modifier in modifiers) {
                if (modifier.op == Modifier.Operator.MULTIPLY) {
                    calc *= modifier.value;
                }
            }
            float lowest = -1;
            foreach (Modifier modifier in modifiers) {
                if (modifier.op == Modifier.Operator.SET) {
                    if (lowest == -1 || modifier.value < lowest) {
                        lowest = modifier.value;
                    }
                }
            }
            if (lowest != -1) {
                calc = lowest;
            }
            modifiers.Clear();
            calculatedValue = calc;
        }
    }

    public class Modifier {
        public float value;
        public Operator op;

        public Modifier(float value, Operator op) {
            this.value = value;
            this.op = op;
        }

        public enum Operator {
            ADDITION, MULTIPLY, SET
        }
    }
}
