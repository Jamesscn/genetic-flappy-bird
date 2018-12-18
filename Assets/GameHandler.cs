using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Neuron {
	int inputSize;
	float[] weights;

	public Neuron(int size) {
		inputSize = size;
		weights = new float[size];
		for (int i = 0; i < size; i++) {
			weights[i] = UnityEngine.Random.value - 0.5F;
		}
	}

	public float evaluate(float[] inputs) {
		float response = 0;
		for (int i = 0; i < inputSize; i++) {
			response += weights[i] * inputs[i];
		}
		return response;
	}

	public void mutate() {
		for (int i = 0; i < inputSize; i++) {
			weights[i] += (UnityEngine.Random.value - 0.5F);
		}
	}
}

public class Bird : IComparable {
	float height;
	float velocity;
	float acceleration;
	float fitness;
	float maxFitness;
	bool alive;
	public Neuron[] process;
	public Neuron output;

	public Bird() {
		fitness = 0;
		height = 0;
		velocity = 0;
		acceleration = -0.01F;
		process = new Neuron[6];
		for (int i = 0; i < 6; i++) {
			process[i] = new Neuron(2);
		}
		output = new Neuron(6);
		alive = true;
	}

	public float getHeight() {
		return height;
	}

	public float getMaxFitness() {
		return maxFitness;
	}

	public bool isAlive() {
		return alive;
	}

	public void fall() {
		if (alive) {
			velocity += acceleration;
			height += velocity;
			if (height > 4.2F) {
				height = 4.2F;
				velocity = 0;
			}
			fitness += 0.1F;
			if (fitness > maxFitness) {
				maxFitness = fitness;
			}
		} else {
			if (height > -3.35F) {
				velocity += acceleration;
				height += velocity;
			} else {
				height = -3.35F;
			}
		}
	}

	public void checkJump(float tubeHeight, float distanceToTube) {
		if (alive) {
			float[] inputs = new float[2];
			inputs[0] = height - tubeHeight;
			inputs[1] = distanceToTube;
			float[] processResponses = new float[6];
			for (int i = 0; i < 6; i++) {
				processResponses[i] = process[i].evaluate(inputs);
			}
			float jumpDeterminant = output.evaluate(processResponses);
			if (jumpDeterminant > 0.5) {
				velocity = 0.13F;
			}
		}
	}

	public void die() {
		alive = false;
		velocity = 0;
	}

	public void mutate() {
		for (int i = 0; i < 6; i++) {
			process[i].mutate();
		}
		output.mutate();
	}

	public void reset() {
		fitness = 0;
		height = 0;
		velocity = 0;
		alive = true;
	}

	public int CompareTo(object obj) {
		Bird otherBird = obj as Bird;
		if (maxFitness > otherBird.getMaxFitness()) {
			return -1;
		} else {
			return 1;
		}
	}
}

public class GameHandler : MonoBehaviour {

	public GameObject bird;
	public GameObject tubeSet1;
	public GameObject tubeSet2;
	public int birdsPerGen = 20;
	public float keepAmount = 0.2F;
	public float mutateAmount = 0.2F;
	public float tubeSpeed = 0.1F;
	public Text scoreboard;
	public Text generationText;
	public Slider survivalSlider;
	public Slider mutationSlider;
	public InputField birdField;
	public Button resetButton;
	private Bird[] birds;
	private GameObject[] physicalBirds;
	private int gen = 0;
	private float height1 = 0;
	private float height2 = 0;
	private int score = 0;
	private bool passed = false;

	Bird Breed(Bird a, Bird b) {
		Bird child = new Bird();
		int[] neuronSelection = { 0, 1, 2, 3, 4, 5, 6 };
		for (int i = 6; i >= 0; i--) {
			int randIndex = UnityEngine.Random.Range(0, i + 1);
			int tmp = neuronSelection[randIndex];
			neuronSelection[randIndex] = neuronSelection[i];
			neuronSelection[i] = tmp;
		}
		for (int i = 0; i < 4; i++) {
			if (neuronSelection[i] == 6) {
				child.output = a.output;
			} else {
				child.process[neuronSelection[i]] = a.process[neuronSelection[i]];
			}
		}
		for (int i = 4; i < 7; i++) {
			if (neuronSelection[i] == 6) {
				child.output = b.output;
			} else {
				child.process[neuronSelection[i]] = b.process[neuronSelection[i]];
			}
		}
		return child;
	}

	void Start () {
		birds = new Bird[birdsPerGen];
		for (int i = 0; i < birdsPerGen; i++) {
			birds[i] = new Bird();
		}
		height1 = 4 * (UnityEngine.Random.value - 0.5F);
		height2 = 4 * (UnityEngine.Random.value - 0.5F);
		physicalBirds = new GameObject[birdsPerGen];
		for (int i = 0; i < birdsPerGen; i++) {
			physicalBirds[i] = Instantiate(bird);
			physicalBirds[i].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(UnityEngine.Random.value, 1, 1);
		}
		resetButton.onClick.AddListener(Reset);
	}

	void Update () {
		int dead = 0;
		for (int i = 0; i < birdsPerGen; i++) {
			birds[i].fall();
			if (Mathf.Abs(tubeSet1.transform.position.x) < Mathf.Abs (tubeSet2.transform.position.x)) {
				birds[i].checkJump(height1, tubeSet1.transform.position.x);
			} else {
				birds[i].checkJump(height2, tubeSet2.transform.position.x);
			}
			if (birds[i].getHeight() < -3.35F) {
				birds[i].die();
			}
			if (tubeSet1.transform.position.x < 2.35F && tubeSet1.transform.position.x > -2.35F) {
				if (birds[i].getHeight() > tubeSet1.transform.position.y + 1 || birds[i].getHeight() < tubeSet1.transform.position.y - 1) {
					birds[i].die();
				}
			}
			if (tubeSet2.transform.position.x < 2.35F && tubeSet2.transform.position.x > -2.35F) {
				if (birds[i].getHeight() > tubeSet2.transform.position.y + 1 || birds[i].getHeight() < tubeSet2.transform.position.y - 1) {
					birds[i].die();
				}
			}
			if (!birds[i].isAlive()) {
				physicalBirds[i].transform.position = new Vector3(physicalBirds[i].transform.position.x - tubeSpeed, birds[i].getHeight(), 0);
				dead++;
			} else {
				physicalBirds[i].transform.position = new Vector3(0, birds[i].getHeight(), 0);
			}
		}
		if (dead == birdsPerGen) {
			gen++;
			score = 0;
			Array.Sort(birds);
			for (int i = (int)(birdsPerGen * keepAmount); i < birdsPerGen; i++) {
				int indexA = UnityEngine.Random.Range(0, (int)(birdsPerGen * keepAmount));
				int indexB = UnityEngine.Random.Range(0, (int)(birdsPerGen * keepAmount));
				birds[i] = Breed(birds[indexA], birds[indexB]);
			}
			for (int i = (int)(birdsPerGen * (1 - mutateAmount)); i < birdsPerGen; i++) {
				birds[i].mutate();
			}
			for (int i = 0; i < birdsPerGen; i++) {
				birds[i].reset();
				height1 = 4 * (UnityEngine.Random.value - 0.5F);
				tubeSet1.transform.position = new Vector3(8, height1, -1);
				height2 = 4 * (UnityEngine.Random.value - 0.5F);
				tubeSet2.transform.position = new Vector3(24, height2, -1);
			}
		}
		tubeSet1.transform.position = new Vector3 (tubeSet1.transform.position.x - tubeSpeed, height1, -1);
		tubeSet2.transform.position = new Vector3 (tubeSet2.transform.position.x - tubeSpeed, height2, -1);
		if (tubeSet1.transform.position.x < -16) {
			passed = false;
			height1 = 4 * (UnityEngine.Random.value - 0.5F);
			tubeSet1.transform.position = new Vector3(16, height1, -1);
		}
		if (tubeSet2.transform.position.x < -16) {
			passed = false;
			height2 = 4 * (UnityEngine.Random.value - 0.5F);
			tubeSet2.transform.position = new Vector3(16, height2, -1);
		}
		if (tubeSet1.transform.position.x < 0 && !passed) {
			score++;
			passed = true;
		}
		if (tubeSet2.transform.position.x < 0 && !passed) {
			score++;
			passed = true;
		}
		scoreboard.text = score.ToString();
		generationText.text = "Generation: " + gen.ToString();
	}

	void Reset() {
		for (int i = 0; i < birdsPerGen; i++) {
			Destroy(physicalBirds[i]);
		}
		keepAmount = survivalSlider.value;
		mutateAmount = mutationSlider.value;
		int prev = birdsPerGen;
		int.TryParse(birdField.text, out birdsPerGen);
		if (birdsPerGen == 0) {
			birdsPerGen = prev;
		}
		gen = 0;
		score = 0;
		birds = new Bird[birdsPerGen];
		for (int i = 0; i < birdsPerGen; i++) {
			birds[i] = new Bird();
		}
		height1 = 4 * (UnityEngine.Random.value - 0.5F);
		height2 = 4 * (UnityEngine.Random.value - 0.5F);
		physicalBirds = new GameObject[birdsPerGen];
		for (int i = 0; i < birdsPerGen; i++) {
			physicalBirds[i] = Instantiate(bird);
			physicalBirds[i].GetComponent<SpriteRenderer>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
		}
	}
}
