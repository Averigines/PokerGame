using System;

namespace Model
{
    public sealed class Player
    {
        public string Name { get; }

        public Player(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A player needs a name.", nameof(name));
            Name = name;
        }
    }
}
