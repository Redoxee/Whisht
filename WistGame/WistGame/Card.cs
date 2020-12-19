using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    public struct Card
    {
        public short Value;
        public Sigil Sigil;

        public override string ToString()
        {
            string cardValue = this.Value < Card.ValueNames.Length ? Card.ValueNames[this.Value] : this.Value.ToString();
            return $"[{cardValue} of {this.Sigil}]";
        }

        public static readonly string[] ValueNames = {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "Jack",
            "Queen",
            "King",
            "Ace",
        };
    }

    public enum Sigil
    {
        Spade,
        Club,
        Heart,
        Diamond,
    }
}
