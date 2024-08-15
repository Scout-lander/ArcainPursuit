using System.Collections;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.UI;

public class HurtUI : MonoBehaviour
{
    public Image targetImage; // The Image component to change color
    public Color hurtColor = Color.red; // The color to change to when hurt
    public float hurtDuration = 1f; // Duration to keep the hurt color
    public float expressionDuration = 1.5f;

    private Color originalColor;
    public Character character;

    private Coroutine expressionCoroutine;
    private Coroutine colorCoroutine;
    private float lastDamageTimeColor;
    private float lastDamageTimeExpression;

    private void Start()
    {
        if (targetImage != null)
        {
            originalColor = targetImage.color;
        }
    }

    public void FlashHurtColor()
    {
        lastDamageTimeColor = Time.time;
        lastDamageTimeExpression = Time.time;

        if (colorCoroutine == null)
        {
            colorCoroutine = StartCoroutine(ChangeColor());
        }

        if (expressionCoroutine == null)
        {
            expressionCoroutine = StartCoroutine(Expression());
        }
    }

    private IEnumerator ChangeColor()
    {
        targetImage.color = hurtColor;

        while (Time.time - lastDamageTimeColor < hurtDuration)
        {
            yield return null;
        }

        targetImage.color = originalColor;
        colorCoroutine = null;
    }

    private IEnumerator Expression()
    {
        character.SetExpression("Angry");

        while (Time.time - lastDamageTimeExpression < expressionDuration)
        {
            yield return null;
        }

        character.SetExpression("Default");
        expressionCoroutine = null;
    }
}
