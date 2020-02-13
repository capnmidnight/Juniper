/* global require, module, exports, process */
// A few functions used in conjunction with
// hashMap and where
exports.equal = function ( a, b ) {
  return a === b;
};
exports.notEqual = function ( a, b ) {
  return a !== b;
};
exports.key = function ( k, v ) {
  return k;
};
exports.keys = function ( obj ) {
  return this.hashMap( obj, this.key );
};
exports.value = function ( k, v ) {
  return v;
};
exports.values = function ( obj ) {
  return this.hashMap( obj, this.value );
};

// Applies a exports.to the contents of an associative
// array, returning the results of each call on that
// exports.in an array.
//          - hsh: the associative array to process
//          - thunk: a function, taking two parameters "key" and "value",
//                              that returns a single-value result.
exports.hashMap = function ( hsh, thunk )
{
  var output = [ ];
  for ( var key in hsh )
    output[output.length] = thunk( key, hsh[key] );
  return output;
};

// filters an associative array.
//    - hsh: the associative array to process.
//    - getter: a function, taking two parameters "key"
//            and "value", that returns a single-value
//            result, as in hashMap.
//    - comparer: a function, taking two values A and B,
//            that compares the output of getter to the
//            val parameter.
//    - val: a filtering value.
exports.where = function ( hsh, getter, comparer, val )
{
  var output = { };
  if ( hsh && getter && comparer )
    for ( var key in hsh )
      if ( comparer( getter( key, hsh[key] ), val ) )
        output[key] = hsh[key];
  return output;
};

// Picks a random item out of an array
exports.selectRandom = function ( arr )
{
  if ( arr )
    return arr[Math.floor( Math.random() * arr.length )];
};

// Frequently, it's necessary to print the status of a
// hash. This exports.will run the printing, or return
// the word "none" if there is nothing in the hash.
//    - formatter: a function, taking two parameters "key"
//            and "value", that returns a single-value
//            result, as in hashMap (as that is where it
//            will be used). The exports.should return
//                           a string.
//          - hsh: the associative array to process
exports.formatHash = function ( hsh, formatter )
{
  if ( hsh )
  {
    var strs = this.hashMap( hsh, formatter );
    if ( strs.length > 0 )
      return strs.join( "\n\n" );
  }
  return "\tnone";
};

exports.hashSatisfies = function ( onHand, required )
{
  for ( var k in required )
    if ( !onHand[k] || onHand[k] < required[k] )
      return false;
  return true;
};

exports.inc = function ( hsh, itm, amt )
{
  amt = amt || 1;
  if ( !hsh[itm] )
    hsh[itm] = 0;
  hsh[itm] += amt;
};

exports.dec = function ( hsh, itm, amt )
{
  amt = amt || 1;
  if ( hsh[itm] )
  {
    hsh[itm] -= amt;
    if ( hsh[itm] === 0 )
      delete hsh[itm];
  }
};

exports.transfer = function ( itm, from, to, amt )
{
  if ( from[itm] )
  {
    this.dec( from, itm, amt );
    this.inc( to, itm, amt );
    return true;
  }
  return false;
};

exports.time = function () {
  var d = new Date();
  return ( d.getHours() * 60 + d.getMinutes() ) * 60 + d.getSeconds();
};

if ( process.argv.indexOf( "--headless" ) === -1 ) {
  exports.log = console.log;
}
else {
  exports.log = function () {
  };
}

if ( process.argv.indexOf( "--test" ) === -1 ) {
  exports.test = function () {
  };
}
else {
  exports.test = console.log;
}