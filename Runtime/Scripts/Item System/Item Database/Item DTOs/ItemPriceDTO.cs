namespace SF.ItemModule
{
    [System.Serializable]
    public class ItemPriceDTO
    {
        public bool CanBuy;
        public bool CanSell;
        public int BuyPrice = 10;
        public int SellPrice = 5;

        public ItemPriceDTO() { }
        
        public ItemPriceDTO(bool canBuy = false, bool canSell = false, int buyPrice = 0, int sellPrice = 0)
        {
            CanBuy = canBuy;
            CanSell = canSell;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }
    }
}
