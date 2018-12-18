# Genetic Flappy Bird
My take on making a genetic algorithm that plays Flappy Bird using Unity3D.

[Link to the demonstration.](https://jamesscn.github.io/genetic-flappy-bird/)

This project was inspired by Machine-Learning-Flappy-Bird by **ssusnic**, who has [an article](https://www.askforgametask.com/tutorial/machine-learning-algorithm-flappy-bird/) on how the project works.

<img src="logo.png" alt="logo" width=200/>

**Implementation of the genetic algorithm**

A set number of birds are placed in the game with Neural Networks that contain two input neurons, six processing neurons and one output neuron. The first input neuron indicates the vertical distance between the bird and the center of the tubes opening, and the second one represents the horizontal distance to the nearest tube. The output neuron makes the bird jump if its value is greater than 0.5.

The six processing neurons and the output neuron are initialized with random weights which are modified only in the case of a mutation. The fittest birds are bred with eachother depending on the survival rate and the rest are killed off. Breeding takes random neurons from both parents and places them in the children's neural networks. Once the children are born, a select amount of them undergo mutation based on the mutation rate.

Fitness is calculated as the total amount of time that the bird stays in the air.