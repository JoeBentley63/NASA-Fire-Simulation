# NASA-Fire-Simulation
This project was begun at the Dublin Nasa Space Apps hackathon over the weekend of April 29-302017. The aim was to use Nasa satellite data
to simulate the spread of wild fires, proving estimates of spread under certain conditions, help with evacuation planning, etc.

The challenge is described here: https://2017.spaceappschallenge.org/challenges/warning-danger-ahead/and-you-can-help-fight-fires/details

Given the time constraints of the hackathon, using multiple raw nasa datasets was proving difficult (each was sampled at a different scale
and so not easily overlayed) we decided to instead import maps directly from google maps satellite views and use image processing
techniques (mainly colour sampling) to make crude estimates of the location of vegitation, water, land, etc.

The c# project here is in two parts. First is the tool for importing screenshots of maps and converting them to csv for water cover, veg cover,
fire start points, etc. Second part is a celular automata simulation of fire spread accounting for wind direction, strenth. Model is simular to
https://bib.irb.hr/datoteka/278897.Ljiljana_Bodrozic_ceepus2006_2.pdf
