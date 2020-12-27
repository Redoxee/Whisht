using System;
using System.Collections.Generic;
using System.Text;

namespace WistGame
{
    public class GameChangePool
    {
        private const int SizeIncrement = 20;
        private GameChange[] data;
        private int count;

        public int Count
        {
            get => this.count;
        }

        public GameChangePool()
        {
            this.data = new GameChange[GameChangePool.SizeIncrement];
            this.count = 0;
        }

        public ref GameChange AllocateGameChange()
        {
            if (this.count == this.data.Length)
            {
                System.Array.Resize(ref this.data, this.data.Length + GameChangePool.SizeIncrement);
            }

            return ref this.data[count++];
        }

        public ref GameChange AllocateGameChange(GameChange.GameChangeType changeType)
        {
            ref GameChange result = ref this.AllocateGameChange();
            result.ChangeType = changeType;
            return ref result;
        }

        public ref GameChange GetGameChange(int index)
        {
            if (index < 0 || index >= this.count)
            {
                throw new ArgumentOutOfRangeException($"index {index} is out of range [0,{this.count}].");
            }

            return ref this.data[index];
        }

        public void Clear()
        {
            this.count = 0;
        }

        public GameChange[] GetGameChanges()
        {
            GameChange[] result = new GameChange[this.count];

            if (count == 0)
            {
                return result;
            }

            System.Array.Copy(this.data, result, this.count);

            return result;
        }
    }
}
