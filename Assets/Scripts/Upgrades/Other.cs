[System.Serializable]
    public struct ResourceAmount
    {
        public ResourceAmount(ResourceType type, BFN amount)
        {
            this.type = type;
            this.amount = amount;
        }

        public ResourceType type;
        public BFN amount;

        public static ResourceAmount operator +(ResourceAmount a, ResourceAmount b)
        {
            return new ResourceAmount(a.type, a.amount + b.amount);
        }

        public static ResourceAmount operator -(ResourceAmount a, ResourceAmount b)
        {
            return new ResourceAmount(a.type, a.amount - b.amount);
        }

        public override string ToString()
        {
            return $"{type} {amount}";
        }

        public string ToPrettyString()
        {
            return $"Type: {type} Amount: {amount}";
        }

        public void ClearResource()
        {
            amount = BFN.Zero;
        }
    }

    public enum ResourceType
    {
        gold,
        soul,
        ether,
        crystal
    }

    public enum Stat
    {
        damage,
        revenue,
        price
    }