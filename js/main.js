var game;
var buffer;
var rainfallNumber = 0;       

window.addEventListener('resize', function () {  game.scale.refresh();});

function preload () {
    game.load.image('initial', 'images/initial.jpg');
}

function create(){
    buffer = game.add.bitmapData(game.width, game.height);
    buffer.copy('initial');
    buffer.addToWorld();
}

function render(){
    applyChanges();
}

function applyChanges(){
    //rather than generating dummy changes, we will call the simulation
    var changes = generateDummyRainfallChanges();

    for(i = 0; i < changes.length; i ++){
        var change = changes[i];
        var delta = {
            x: change[0],
            y: change[1],
            r: change[2],
            g: change[3],
            b: change[4]
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
    game = new Phaser.Game("100", "100", Phaser.AUTO, 'phaser-div', { create: create, preload: preload, render: render});
};


function rainfallChange(newValue){
    rainfallNumber = newValue;
}

