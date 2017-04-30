var GROUND = 0;
var WATER = 1;
var FIRE = 2;

var SPARSE_TREE = 3;
var TREE = 4;
var DENSE_TREE = 5;

function is_tree(state) {
    return (state == SPARSE_TREE) || (state == TREE) || (state == DENSE_TREE);
}