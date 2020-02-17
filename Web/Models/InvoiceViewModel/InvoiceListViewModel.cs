namespace Web.ViewModels {
    public class InvoiceListViewModel {
        public long Id { get; set; }
        public string No { get; set; }
        public double Subtotal { get; set; }
        public double TaxRate { get; set; }
        public string Amount => (Subtotal * (1 + TaxRate / 100)).ToString("0.##");
        public string Date { get; set; }
        public string DueDate { get; set; }

        public double PaymentAmount { get; set; }
        public string PaymentDate { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
    }
}
