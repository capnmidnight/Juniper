/* global require, module, exports */
var Thing = require( "./thing.js" );
var core = require( "./core.js" );
var fs = require( "fs" );
/* Recipe class
 *  A set of criteria to be able to create items in the
 *  users inventory. Each parameter is an associative array
 *  combining an item ID with a count.
 *
 *  - db: a database of all Things.
 *  - id: the id to use for this Thing.
 *  - description: every Thing has a physical meaning to it,
 *      that is expressed through a description. For now, this is just
 *      prose text. One day, it might be more.
 *  - ingredients: the name and count of items that must be
 *          consumed out of the users inventory to be able
 *          to create the item.
 *  - results: the name and count of items that will be added
 *          to the users inventory after the recipe has ran.
 *  - tools (optional): the name and count of items that must exist in
 *          the users inventory (but will not get consumed).
 */
function Recipe ( db, id, description, ingredients, results, tools )
{
  Thing.call( this, db, "recipes", id, description );
  this.ingredients = ingredients || { };
  this.results = results || { };
  this.tools = tools || { };
}
Recipe.prototype = Object.create( Thing.prototype );
module.exports = Recipe;

function parseItemList ( part, options, state, name )
{
  name = name.toLocaleLowerCase();
  if ( part[part.length - 1] === ',' )
    part = part.substring( 0, part.length - 1 );
  else if ( part[part.length - 1] === '.' ) {
    part = part.substring( 0, part.length - 1 );
    state = "quit";
  }
  else if ( part[part.length - 1] === ';' ) {
    part = part.substring( 0, part.length - 1 );
    state = "none";
  }

  var isNumber = part * 1 === part;
  if ( isNumber ) {
    options[name][options.lastItem] = part * 1;
    options.lastItem = null;
  }
  else {
    options.lastItem = part;
    options[name][part] = 1;
  }

  if ( state === "none" )
    options.lastItem = null;
  return state;
}

var parsers = {
  none: function ( part, options ) {
    return part;
  },
  from: function ( part, options ) {
    return parseItemList( part, options, "from", "ingredients" );
  },
  using: function ( part, options ) {
    return parseItemList( part, options, "using", "tools" );
  },
  to: function ( part, options ) {
    return parseItemList( part, options, "to", "results" );
  }
};

Recipe.parse = function ( db, text )
{
  var options = {
    ingredients: { },
    results: { },
    tools: { }
  };
  var parts = text.split( " " );
  var state = "none";
  var id = parts.shift();
  core.test( ">>>> RECIPE", id );
  while ( parts.length > 0 && state !== "quit" ) {
    var oldState = state;
    var line = parts.shift();
    state = parsers[state]( line, options );
    core.test( oldState, "->", line, "->", state );
  }
  var description = parts.join( " " );
  new Recipe( db, id, description,
      options.ingredients, options.results, options.tools );
};

Recipe.parseAll = function ( db, lines ) {
  while ( lines.length > 0 ) {
    var line = lines.shift();
    if ( line.length > 0 )
      Recipe.parse( db, line );
  }
};

Recipe.load = function ( db, fileName ) {
  var data = fs.readFileSync( fileName, { encoding: "utf8" } );
  var lines = data.split( "\n" )
      .map( function ( l ) {
        return l.trim();
      } );
  Recipe.parseAll( db, lines );
};