namespace tl2_tp6_2024_mainaessens.Models;


public class ErrorViewModel
{
    public string RequestId { get; set; } // Le saque el ? en string?

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
