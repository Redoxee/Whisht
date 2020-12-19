using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serialization
{
    public static class CardExtension
    {


        public static FlatBuffers.Offset<Card> CreateCard(FlatBuffers.FlatBufferBuilder builder, WistGame.Card card)
        {
            return Card.CreateCard(builder, (Sigil)card.Sigil, card.Value);
        }
    }
}
