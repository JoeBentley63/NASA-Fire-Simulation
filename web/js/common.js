// ---------------- Types of terrain ----------------

var GROUND = 0;
var WATER = 1;
var FIRE = 2;
var SPARSE_TREE = 3;
var TREE = 4;
var DENSE_TREE = 5;


// ---------------- Types of wind ----------------

var NO_WIND = 0;
var LIGHT_WIND = 1;
var MODERATE_WIND = 2;
var STRONG_WIND = 3;


// ---------------- Helper functions ----------------

function is_tree(state) {
    return (state == SPARSE_TREE) || (state == TREE) || (state == DENSE_TREE);
}

/*
 * Classifies wind strength according to its speed and direction
 * see http://www.windows2universe.org/earth/Atmosphere/wind_speeds.html for wind classification
 *
 * Parameters 
 * speed - expected in km/hr
 * angle - expected in radians
 *
 * Returns
 * Type of wind
 */
function classify_wind_strength(speed, angle) {
    var speed_projection = speed * Math.cos(angle);

    if (speed_projection <= 1) return NO_WIND;
    if (speed_projection <= 11) return LIGHT_WIND;
    if (speed_projection <= 38) return MODERATE_WIND;
    return STRONG_WIND;
}