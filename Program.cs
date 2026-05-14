using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

ParkingBeachManager _beachManager = new ParkingBeachManager();
_beachManager.AddParkingBeach(10, 3, 23.5m);
_beachManager.AddParkingBeach(10, 4, 23m);
_beachManager.AddParkingBeach(10, 3, 20.4m);
_beachManager.AddParkingBeach(10, 6, 23.3m);
_beachManager.AddParkingBeach(10, 7, 26.5m);

var scene = new MainMenuScene(_beachManager, 0, 0);
var engine = new Engine(scene);
engine.Start();
