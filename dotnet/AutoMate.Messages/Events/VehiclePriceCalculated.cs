namespace AutoMate.Messages.Events {
    public interface VehiclePriceCalculated {
        string Registration { get; set; }
        int Price { get; set; }
        string CurrencyCode { get; set; }
    }
}