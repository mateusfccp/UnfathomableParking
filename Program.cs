using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

ParkingBeachManager _beachManager = new ParkingBeachManager();

var scene = new MainMenuScene(_beachManager, 0, 0);
var engine = new Engine(scene);
engine.Start();
