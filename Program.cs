using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

ParkingBeachManager _beachManager = new ParkingBeachManager();
_beachManager.AddParkingBeach(20, 20);
_beachManager.AddParkingBeach(15, 20);
_beachManager.AddParkingBeach(20, 10);
_beachManager.AddParkingBeach(17, 12);
_beachManager.AddParkingBeach(10, 20);
_beachManager.AddParkingBeach(15, 3);

var scene = new MainMenuScene(_beachManager);
var engine = new Engine(scene);
engine.Start();
