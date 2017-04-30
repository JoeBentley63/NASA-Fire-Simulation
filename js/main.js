var map_color_codes = {}
map_color_codes[GROUND] = {r: 125, g: 68, b: 29}
map_color_codes[WATER] = {r: 64, g: 164, b: 223}
map_color_codes[FIRE] = {r: 226, g: 88, b: 34}
map_color_codes[SPARSE_TREE] = {r: 124, g: 252, b: 0}
map_color_codes[TREE] = {r: 0, g: 150, b: 0}
map_color_codes[DENSE_TREE] = {r: 0, g: 75, b: 0}

var game;
var buffer;
var rainfallNumber = 0;
var forest_grid = [];
var forest_wildfire;

window.addEventListener('resize', function () {  game.scale.refresh();});

function get_random_pixel() {
    var rnd = Math.random();
    if (rnd < 0.125) return GROUND;
    if (rnd < 0.250) return WATER;
    if (rnd < 0.500) return SPARSE_TREE;
    if (rnd < 0.750) return DENSE_TREE;
    return TREE;
}

function preload () {
    buffer = game.add.bitmapData(game.width, game.height);
    for (var i = 0; i < game.width; i++) {
        forest_grid.push([]);
        for (var j = 0; j < game.height; j++) {
            var pixel = get_random_pixel();
            forest_grid[i].push(pixel);
            buffer.setPixel(i, j, map_color_codes[pixel].r, map_color_codes[pixel].g, map_color_codes[pixel].b, 255, true);
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
            r: map_color_codes[change.state].r,
            g: map_color_codes[change.state].g,
            b: map_color_codes[change.state].b
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

