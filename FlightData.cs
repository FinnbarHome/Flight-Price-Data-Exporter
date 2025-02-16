namespace CheapestFlightsRewrite
{
    internal class FlightData
    {
        public string Date { get; }
        public string Departure { get; }
        public string Destination { get; }
        public string Time { get; }
        public string Price { get; }

        public FlightData(string date, string departure, string destination, string time, string price)
        {
            Date = date;
            Departure = departure;
            Destination = destination;
            Time = time;
            Price = price;
        }

        public override string ToString()
        {
            return $"A flight from {Departure} to {Destination} departing at {Time} on {Date} is priced at {Price}";
        }
    }
}
