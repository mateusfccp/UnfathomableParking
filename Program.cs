using UnfathomableParking.Models;
using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

ParkingBeach p = new ParkingBeach(10,4,1000);
var scene = new ParkingBeachScene(p);
var engine = new Engine(scene);
engine.Start();
Console.Clear();
