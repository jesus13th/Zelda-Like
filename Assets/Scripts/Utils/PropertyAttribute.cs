using UnityEngine;

public class ShowIfAttribute : PropertyAttribute {
    public string ConditionFieldName { get; private set; }
    public bool Negation { get; private set; }

    public ShowIfAttribute(string conditionFieldName, bool negation = false) {
        ConditionFieldName = conditionFieldName;
        Negation = negation;
    }
}