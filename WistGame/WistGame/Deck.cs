using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    public class Deck
    {
        public Card[] Cards;
        public int NumberOfCards;
        public int TotalNumberOfCards;

        public Deck()
        {
            this.RefillDeck();
        }

        public void RefillDeck()
        {
            this.NumberOfCards = 0;

            Array sigils = Enum.GetValues(typeof(Sigil));
            this.TotalNumberOfCards = Card.ValueNames.Length * sigils.Length;
            this.Cards = new Card[this.TotalNumberOfCards];

            for (int sigilIndex = 0; sigilIndex < sigils.Length; ++sigilIndex)
            {
                for (short cardValueIndex = 0; cardValueIndex < Card.ValueNames.Length; ++cardValueIndex)
                {
                    ref Card card = ref this.Cards[this.NumberOfCards++];
                    card.Sigil = (Sigil)sigilIndex;
                    card.Value = cardValueIndex;
                }
            }
        }

        public void Shuffle()
        {
            Random random = new Random();

            for (int index = 0; index < this.NumberOfCards - 1; ++index)
            {
                int newIndex = random.Next(index, this.NumberOfCards);
                if (index == newIndex)
                {
                    continue;
                }

                Card temp = this.Cards[index];
                this.Cards[index] = this.Cards[newIndex];
                this.Cards[newIndex] = temp;
            }
        }

        public Card PickCard()
        {
            if (this.NumberOfCards < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.Cards[--this.NumberOfCards];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            for (int index = 0; index < this.NumberOfCards; ++index)
            {
                builder.Append(this.Cards[index].ToString());
                if (index < this.NumberOfCards - 1)
                {
                    builder.Append(",");
                }
            }

            builder.Append($"] ({this.NumberOfCards})");
            return builder.ToString();
        }
    }
}
