window.onload = function() {
        var height = 600;
        var width = 800;
        
        var game = new Phaser.Game(width, height, Phaser.AUTO, '', { preload: preload, render: render});
        
        var buffer;
        
        function preload () {
            	buffer = game.add.bitmapData(game.width, game.height);
                buffer.fill(100, 100, 100, 1);
                buffer.addToWorld();
        }
        
        function  render(){
            applyChanges();
        }
        
        function applyChanges(){
            //rather than generating dummy changes, we will call the simulation
            var changes = generateDummyChanges();
            
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
            }
        }
    
        function generateDummyChanges(){
            var changes = [[]]
            
            for(i = 0; i < 100; i ++){
                changes.push([getRandomInt(0, width), getRandomInt(0, height), 255, 0, 0])
            }
            
            console.log(changes);
            return changes
        }
        
        function getRandomInt(min, max) {
          min = Math.ceil(min);
          max = Math.floor(max);
          return Math.floor(Math.random() * (max - min)) + min;
        }
    };