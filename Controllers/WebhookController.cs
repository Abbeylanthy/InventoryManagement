using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Services.Interfaces;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;

[ApiController]
[Route("api/payments")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _config;

    public PaymentWebhookController(IPaymentService paymentService, IConfiguration config)
    {
        _paymentService = paymentService;
        _config = config;
    }

   [HttpPost("webhook")]
public async Task<IActionResult> PaystackWebhook() 
{
    try
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync(); // 

        Console.WriteLine("WEBHOOK HIT");
        Console.WriteLine(json);

        var secret = _config["Paystack:SecretKey"]
            ?? throw new Exception("Missing secret");

        var signature = Request.Headers["x-paystack-signature"].ToString();

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));

        var computedHash = BitConverter.ToString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(json))
        ).Replace("-", "").ToLower();

        if (string.IsNullOrEmpty(signature) || computedHash != signature)
        {
            Console.WriteLine("INVALID SIGNATURE");
            return Unauthorized();
        } 

        var webhookEvent = JsonSerializer.Deserialize<PaystackWebhookDto>(json);

        if (webhookEvent?.data?.status == "success")
        {
            await _paymentService.HandleSuccessfulPayment(webhookEvent.data.reference);
        }

        return Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine("WEBHOOK ERROR: " + ex.Message);
        return StatusCode(500, ex.Message);
    }
}
}