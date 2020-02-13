/* global require, module, exports */
var Thing = require( "./thing.js" );
var Exit = require( "./exit.js" );
var Item = require( "./item.js" );
var fs = require( "fs" );
var core = require( "./core.js" );
var format = require( "util" ).format;

// Room class
//  - db: a database of all things
//  - description: the description of the room, that will get
//          printed for the user when they "look".
function Room ( db, id, description, startItems ) {
  Thing.call( this, db, "rooms", id, description );
  this.exits = { };
  this.startItems = startItems || { };
  this.items = { };
}

Room.prototype = Object.create( Thing.prototype );
module.exports = Room;

Room.prototype.spawnItems = function () {
  for ( var itemId in this.startItems ) {
    itemId = itemId.toLocaleLowerCase();
    if ( !this.items[itemId] ) {
      this.items[itemId] = 0;
    }
    this.items[itemId] = Math.max( this.items[itemId],
        this.startItems[itemId] );
  }
};

Room.prototype.describe = function ( user, t ) {
  var people = this.db.getPeopleIn( this.id, user.id )
      .map( function ( u ) {
        return format(
            "\t%s%s",
            u.id,
            u.hp > 0 ? "" : " (KNOCKED OUT)" );
      } );
  var exits = core.values( this.exits )
      .map( function ( x ) {
        return x.describe( user, t );
      } )
      .filter( function ( s ) {
        return s.trim().length > 0;
      } );

  var _this = this;
  var itemList = core.formatHash(
      this.items,
      function ( k, v ) {
        var item = _this.db.getItem(k);
        return format( "\t%d %s - %s", v, k,
            ( item && item.description || "(UNKNOWN)" ) );
      } );
  return format( "%s\n\nITEMS:\n\n%s\n\nPEOPLE:\n\n%s\n\nEXITS:\n\n%s\n\n<hr>",
      this.description,
      itemList,
      people.join( "\n\n" ),
      exits.join( "\n\n" ) );
};

var parsers = {
  none: function ( line, options ) {
    return parsers[line] ? line : "quit";
  },
  exits: function ( line, options ) {
    return collectLines( line, options, "exits" );
  },
  items: function ( line, options ) {
    return collectLines( line, options, "items" );
  },
  npcs: function ( line, options ) {
    return collectLines( line, options, "npcs" );
  }
};

function collectLines ( line, options, name ) {
  if ( line.length === 0 ) {
    return "none";
  }
  else {
    if ( !options[name] ) {
      options[name] = [ ];
    }
    options[name].push( line );
    return name;
  }
}

Room.parse = function ( db, roomId, text ) {
  text = text
      .replace( /\r/g, "" );
  var lines = text
      .split( "\n" )
      .map( function ( l ) {
        return l.trim();
      } );
  var options = {
    items: [ ],
    exits: [ ],
    npcs: [ ]
  };
  var state = "none";
  while ( lines.length > 0 && state !== "quit" ) {
    var line = lines.shift();
    state = parsers[state]( line, options );
    if ( state === "quit" ) {
      lines.unshift( line );
    }
  }
  var description = lines.join( "\r\n" );
  var startItems = { };
  for ( var i = 0; i < options.items.length; ++i ) {
    var parts = options.items[i].split( ' ' );
    var count = 1;
    if ( parts.length === 2 ) {
      count = parts[1] * 1;
    }
    startItems[parts[0]] = count;
  }
  new Room( db, roomId, description, startItems );

  return options.exits;
};

Room.loadAll = function ( db, dir, roomFiles ) {
  var exits = { },
      i,
      roomFile,
      roomId,
      test;

  for ( i = 0; i < roomFiles.length; ++i ) {
    roomFile = roomFiles[i];
    test = roomFile.match( /([^\\/]+).room/ );
    if ( test && test.length >= 2 ) {
      roomId = test[1].toLocaleLowerCase();
      exits[roomId] = Room.parse( db, roomId, fs.readFileSync( dir + roomFile,
          { encoding: "utf8" } ) );
    }
  }

  for ( roomId in exits ){
    roomId = roomId.toLocaleLowerCase()
    for ( i = 0; i < exits[roomId].length; ++i ) {
      Exit.parse( db, roomId, exits[roomId][i] );
    }
  }
};

Room.loadFromDir = function ( db, dirName ) {
  Room.loadAll( db, dirName, fs.readdirSync( dirName )
      .filter( function ( f ) {
        return f.match( /\.room$/ );
      } ) );
};
