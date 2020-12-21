// automatically generated by the FlatBuffers compiler, do not modify

/**
 * @const
 * @namespace
 */
var Serialization = Serialization || {};

/**
 * @enum {number}
 */
Serialization.MessageID = {
  None: 0,
  SandboxState: 1,
  OrderBet: 2,
  OrderPlayCard: 3
};

/**
 * @enum {string}
 */
Serialization.MessageIDName = {
  '0': 'None',
  '1': 'SandboxState',
  '2': 'OrderBet',
  '3': 'OrderPlayCard'
};

/**
 * @enum {number}
 */
Serialization.State = {
  Initialize: 0,
  Betting: 1,
  Fold: 2,
  EndGame: 3,
  Unkown: 4
};

/**
 * @enum {string}
 */
Serialization.StateName = {
  '0': 'Initialize',
  '1': 'Betting',
  '2': 'Fold',
  '3': 'EndGame',
  '4': 'Unkown'
};

/**
 * @enum {number}
 */
Serialization.Sigil = {
  Spade: 0,
  Club: 1,
  Heart: 2,
  Diamond: 3
};

/**
 * @enum {string}
 */
Serialization.SigilName = {
  '0': 'Spade',
  '1': 'Club',
  '2': 'Heart',
  '3': 'Diamond'
};

/**
 * @constructor
 */
Serialization.Card = function() {
  /**
   * @type {flatbuffers.ByteBuffer}
   */
  this.bb = null;

  /**
   * @type {number}
   */
  this.bb_pos = 0;
};

/**
 * @param {number} i
 * @param {flatbuffers.ByteBuffer} bb
 * @returns {Serialization.Card}
 */
Serialization.Card.prototype.__init = function(i, bb) {
  this.bb_pos = i;
  this.bb = bb;
  return this;
};

/**
 * @param {flatbuffers.ByteBuffer} bb
 * @param {Serialization.Card=} obj
 * @returns {Serialization.Card}
 */
Serialization.Card.getRootAsCard = function(bb, obj) {
  return (obj || new Serialization.Card).__init(bb.readInt32(bb.position()) + bb.position(), bb);
};

/**
 * @param {flatbuffers.ByteBuffer} bb
 * @param {Serialization.Card=} obj
 * @returns {Serialization.Card}
 */
Serialization.Card.getSizePrefixedRootAsCard = function(bb, obj) {
  bb.setPosition(bb.position() + flatbuffers.SIZE_PREFIX_LENGTH);
  return (obj || new Serialization.Card).__init(bb.readInt32(bb.position()) + bb.position(), bb);
};

/**
 * @returns {Serialization.Sigil}
 */
Serialization.Card.prototype.Family = function() {
  var offset = this.bb.__offset(this.bb_pos, 4);
  return offset ? /** @type {Serialization.Sigil} */ (this.bb.readInt8(this.bb_pos + offset)) : Serialization.Sigil.Spade;
};

/**
 * @returns {number}
 */
Serialization.Card.prototype.Value = function() {
  var offset = this.bb.__offset(this.bb_pos, 6);
  return offset ? this.bb.readInt16(this.bb_pos + offset) : 0;
};

/**
 * @param {flatbuffers.Builder} builder
 */
Serialization.Card.startCard = function(builder) {
  builder.startObject(2);
};

/**
 * @param {flatbuffers.Builder} builder
 * @param {Serialization.Sigil} Family
 */
Serialization.Card.addFamily = function(builder, Family) {
  builder.addFieldInt8(0, Family, Serialization.Sigil.Spade);
};

/**
 * @param {flatbuffers.Builder} builder
 * @param {number} Value
 */
Serialization.Card.addValue = function(builder, Value) {
  builder.addFieldInt16(1, Value, 0);
};

/**
 * @param {flatbuffers.Builder} builder
 * @returns {flatbuffers.Offset}
 */
Serialization.Card.endCard = function(builder) {
  var offset = builder.endObject();
  return offset;
};

/**
 * @param {flatbuffers.Builder} builder
 * @param {Serialization.Sigil} Family
 * @param {number} Value
 * @returns {flatbuffers.Offset}
 */
Serialization.Card.createCard = function(builder, Family, Value) {
  Serialization.Card.startCard(builder);
  Serialization.Card.addFamily(builder, Family);
  Serialization.Card.addValue(builder, Value);
  return Serialization.Card.endCard(builder);
}
