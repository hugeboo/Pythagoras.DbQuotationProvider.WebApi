namespace Pythagoras.ClockSignal.WebApi.Hubs
{
    public interface IQuotationProviderHub
    {
        Task NewClockTime(DateTime time);
        Task NewVirtualTime(DateTime time);
        Task StateChanged(string state);
    }
}
