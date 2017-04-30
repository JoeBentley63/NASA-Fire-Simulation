var FIRE_RGB = {r: 255, g: 255, b: 0};
var TREE_RGB = {r: 0, g: 255, b: 0};

var game;
var buffer;
var rainfallNumber = 0;
var forest_grid = [];
var forest_wildfire;

window.addEventListener('resize', function () {  game.scale.refresh();});

function preload () {
    buffer = game.add.bitmapData(game.width, game.height);
    for (var i = 0; i < game.width; i++) {
        forest_grid.push([]);
        for (var j = 0; j < game.height; j++) {
            forest_grid[i].push(TREE);
            buffer.setPixel(i, j, TREE_RGB.r, TREE_RGB.g, TREE_RGB.b, 255, true);
        }
    }
    forest_wildfire = new ForestWildfire(forest_grid);
}

function create(){
    buffer.addToWorld();
}

function render(){
    applyChanges();
}

function applyChanges(){
    var changes = forest_wildfire.update_fire_spread([{x: 50, y: 50}], 0, 0, 0);

    for (i = 0; i < changes.length; i++){
        var change = changes[i];
        var delta = {
            x: change.x,
            y: change.y,
            r: FIRE_RGB.r,
            g: FIRE_RGB.g,
            b: FIRE_RGB.b
        }
        
        buffer.setPixel(delta.x, delta.y, delta.r, delta.g, delta.b, 255, true);
        buffer.update();
    }
}

function generateDummyRainfallChanges(){
    var changes = [[]]

    for(i = 0; i < rainfallNumber; i ++){
        changes.push([getRandomInt(0, game.width), getRandomInt(0, game.height), 255, 0, 0]);
        console.log("generated");
    }

    return changes
}

function getRandomInt(min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min)) + min;
}

window.onload = function() {
    game = new Phaser.Game(200, 200, Phaser.AUTO, 'phaser-div', { create: create, preload: preload, render: render});
    //game.time.desiredFps = 1;
    //game.time.slowMotion = 5.0;
};


function rainfallChange(newValue){
    rainfallNumber = newValue;
}

