# Civilizidle

## Description
This repository contains a clicker type game, using the Unity 2d game engine. Setting of the game is medieval, with pixel art animations for the environments and characters.

## Pre-requisites
1. Unity version - 2019 3.3f or greater
2. Firebase Unity SDK - download [here](https://firebase.google.com/docs/unity/setup)
  - Add Firestore and Google Analytics for Firebase packages (6.14.0)

## Setup
1. Download the Firebase Unity SDK
1. Unzip it and import the required libraries to the parent forlder of the project. Required libraries include: Authentication, Analytics, Crashlytics and Firestore.

## Game Progression
- Player clicks on the environment to collect resources (Wood, Stone). 
- Players can hire villagers to increase the collection of resources (by a 2x factor). As the village expands, more materials are required. Villagers can be upgraded to improve the amount of resources they provide. 
- Players can build and upgrade their buildings, to automate the collection of resources. 

## Features
- Firebase Analytics - Captures metrics from the players such as the size of their village, how far players have upgraded their village and resources.
- Civilization can be upgraded 3 times, expanding the resource pool and characters.
