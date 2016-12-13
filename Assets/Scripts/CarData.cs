using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CarData", menuName = "Game Content/Car", order = 1)]
public class CarData : ScriptableObject
{
	public Sprite colourizableSprite;
	public Sprite fixedColourSprite;
	public Color[] colourOptions;
	public float maxSpeed;
	public float maxAcceleration;
	public float stopDistance;

	public void Initialize(CarController car) {
		car.Engine.MaxSpeed = maxSpeed;
		car.Engine.MaxAcceleration = maxAcceleration;
		car.stopDistance = stopDistance;
		car.carColours = new List<Color> (colourOptions);

		SpriteRenderer colourizableRenderer = car.GetComponent<SpriteRenderer>();
		colourizableRenderer.sprite = colourizableSprite;
		foreach (SpriteRenderer renderer in car.GetComponentsInChildren<SpriteRenderer>()) {
			if (renderer == colourizableRenderer || renderer.GetComponent<TurnIndicator>() != null) {
				continue;
			}
			else {
				renderer.sprite = fixedColourSprite;
				break;
			}
		}
	}
}

