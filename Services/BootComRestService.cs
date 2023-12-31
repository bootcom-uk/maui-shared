namespace Services
{
    public class BootComRestService : RestServiceBase
    {
        public override Uri BaseAddress { get; set; } = new Uri("https://money.bootcom.co.uk");

    }
}
