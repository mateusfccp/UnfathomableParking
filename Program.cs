using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

ParkingBeachManager _beachManager = new ParkingBeachManager();
_beachManager.AddParkingBeach(10, 1);
_beachManager.AddParkingBeach(10, 2);
_beachManager.AddParkingBeach(10, 3);
_beachManager.AddParkingBeach(10, 4);
_beachManager.AddParkingBeach(10, 5);
_beachManager.AddParkingBeach(10, 6);
_beachManager.AddParkingBeach(10, 7);

var scene = new MainMenuScene(_beachManager, 0, 0);
var engine = new Engine(scene);
engine.Start();
