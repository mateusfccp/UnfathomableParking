using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

ParkingBeachManager _beachManager = new ParkingBeachManager();
_beachManager.AddParkingBeach("NiggaBeach",10,4,23.5m); //To test

var scene = new MainMenuScene(_beachManager, 0, 0);
var engine = new Engine(scene);
engine.Start();
Console.Clear();
