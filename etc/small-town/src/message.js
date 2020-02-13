/* global require, module, exports */
// Message class
//  All messages to the player are communicated through Messages.
//  The Message is structured such that the AI system can figure
//  out what was done to it, while also being able to inform the
//  user in a meaningful way what happened.

//  - fromId: the person who caused the message to occur.
//  - msg: the actual message, what happened.
//  - payload (optional): an array that provides detailed information
//          about the message.
function Message ( fromId, msg, payload, type )
{
  this.fromId = fromId;
  this.message = msg || "";
  this.payload = payload || [ "" ];
  this.type = type || "news";
}

module.exports = Message;
