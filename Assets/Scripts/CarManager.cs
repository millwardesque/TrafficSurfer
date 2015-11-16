using UnityEngine;
using System.Collections;

public class CarManager : MonoBehaviour {
	public CarController[] carPrefabs;

	public static CarManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	public CarController[] Cars { 
		get { return GameObject.FindObjectsOfType<CarController>(); }
	}
	
	public void GenerateCars(bool cleanupOldCars) {
		if (cleanupOldCars) {
			CleanupCars ();
		}

		int carCount = 0;
		GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
		for (int i = 0; i < roads.Length; ++i) {
			// Skip roads with intersections, we only want to make cars on straight sections of road.
			if (roads[i].GetComponent<IntersectionManager>() != null) {
				continue;
			}

			bool makeRightLaneCar = Random.Range(0, 2) == 0;
			bool makeLeftLaneCar = Random.Range(0, 2) == 0;

			// Create the car in the right lane
			if (makeRightLaneCar) {
				int carType = Random.Range(0, carPrefabs.Length);
				CarController newCar1 = Instantiate<CarController>(carPrefabs[carType]);
				newCar1.name = "Car " + carCount;
				newCar1.transform.SetParent(transform);
				newCar1.transform.Rotate(new Vector3(0f, 0f, -90f + roads[i].transform.rotation.eulerAngles.z));

				Sprite car1Sprite = newCar1.GetComponent<SpriteRenderer>().sprite;
				float spriteWidthOffset = car1Sprite.rect.width / 1.5f / car1Sprite.pixelsPerUnit;
				newCar1.transform.position = roads[i].transform.position + newCar1.transform.right * spriteWidthOffset;
				newCar1.transform.position += newCar1.transform.up * Random.Range(-0.25f, 0.25f);

				newCar1.ResetCar();
				carCount++;
			}

			// Create the car in the left lane
			if (makeLeftLaneCar) {
				int carType = Random.Range(0, carPrefabs.Length);
				CarController newCar2 = Instantiate<CarController>(carPrefabs[carType]);
				newCar2.name = "Car " + carCount;
				newCar2.transform.SetParent(transform);
				newCar2.transform.Rotate(new Vector3(0f, 0f, -180f - 90f + roads[i].transform.rotation.eulerAngles.z));
				
				Sprite car2Sprite = newCar2.GetComponent<SpriteRenderer>().sprite;
				float spriteWidthOffset = car2Sprite.rect.width / 1.5f / car2Sprite.pixelsPerUnit;
				newCar2.transform.position = roads[i].transform.position + newCar2.transform.right * spriteWidthOffset;
				newCar2.transform.position += newCar2.transform.up * Random.Range(-0.25f, 0.25f);

				newCar2.ResetCar();
				carCount++;
			}
		}
	}

	public void CleanupCars() {
		CarController[] cars = this.Cars;
		for (int i = 0; i < cars.Length; i++) {
			GameObject.Destroy(cars[i].gameObject);
		}
	}
}
