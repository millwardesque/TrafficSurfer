using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarManager : MonoBehaviour {
	public CarController carPrefab;
	public CarData[] carTemplates;
	public int maxCars = 20;
	public int generationNumber = 0;
	
	public static CarManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	List<CarController> m_cars = new List<CarController>();
	public List<CarController> Cars { 
		get { return m_cars; }
	}
	
	public void GenerateCars(bool cleanupOldCars) {
		if (cleanupOldCars) {
			CleanupCars ();
		}

		int carCount = 0;
		GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
		int validRoadCount = 0;
		for (int i = 0; i < roads.Length; ++i) {
			// Skip roads with intersections, we only want to make cars on straight sections of road.
			if (roads[i].GetComponent<IntersectionManager>() == null) {
				validRoadCount++;
			}
		}

		if (validRoadCount == 0) {
			Debug.LogError ("Unable to generate cars: There aren't any roads to spawn cars on.");
		}

		int roadsPerCar = Mathf.CeilToInt(validRoadCount / maxCars);
		if (roadsPerCar == 0) {
			roadsPerCar = 1;
		}

		for (int i = 0; i < roads.Length; ++i) {
			// Skip roads with intersections, we only want to make cars on straight sections of road.
			if (roads[i].GetComponent<IntersectionManager>() != null || i % roadsPerCar != 0) {
				continue;
			}

			bool makeRightLaneCar = Random.Range(0, 2) == 0;
			bool makeLeftLaneCar = Random.Range(0, 2) == 0;

			// Create the car in the right lane
			if (makeRightLaneCar) {
				int carType = Random.Range(0, carTemplates.Length);
				CarController newCar1 = Instantiate<CarController> (carPrefab);
				newCar1.carData = carTemplates [carType];
				newCar1.name = "Car " + generationNumber + "." + carCount;
				newCar1.transform.SetParent(transform);
				newCar1.transform.Rotate(new Vector3(0f, 0f, -90f + roads[i].transform.rotation.eulerAngles.z));

				Sprite car1Sprite = newCar1.GetComponent<SpriteRenderer>().sprite;
				float spriteWidthOffset = car1Sprite.rect.width / 1.5f / car1Sprite.pixelsPerUnit;
				newCar1.transform.position = roads[i].transform.position + newCar1.transform.right * spriteWidthOffset;
				newCar1.transform.position += newCar1.transform.up * Random.Range(-0.25f, 0.25f);

				newCar1.ResetCar();
				m_cars.Add(newCar1);

				carCount++;
				if (carCount >= maxCars) {
					break;
				}
			}

			// Create the car in the left lane
			if (makeLeftLaneCar) {
				int carType = Random.Range(0, carTemplates.Length);
				CarController newCar2 = Instantiate<CarController> (carPrefab);
				newCar2.carData = carTemplates [carType];
				newCar2.name = "Car " + generationNumber + "." + carCount;
				newCar2.transform.SetParent(transform);
				newCar2.transform.Rotate(new Vector3(0f, 0f, -180f - 90f + roads[i].transform.rotation.eulerAngles.z));
				
				Sprite car2Sprite = newCar2.GetComponent<SpriteRenderer>().sprite;
				float spriteWidthOffset = car2Sprite.rect.width / 1.5f / car2Sprite.pixelsPerUnit;
				newCar2.transform.position = roads[i].transform.position + newCar2.transform.right * spriteWidthOffset;
				newCar2.transform.position += newCar2.transform.up * Random.Range(-0.25f, 0.25f);

				newCar2.ResetCar();
				m_cars.Add(newCar2);

				carCount++;
				if (carCount >= maxCars) {
					break;
				}
			}
		}

		generationNumber++;
	}

	public void CleanupCars() {
		for (int i = 0; i < m_cars.Count; i++) {
			GameObject.Destroy(m_cars[i].gameObject);
			m_cars[i] = null;
		}

        if (TargetCarIndicator.Instance != null) {
            TargetCarIndicator.Instance.TargetCar = null;
        }		
		Cars.Clear();
	}
}
