namespace NSE.Identity.API.Extensions
{
  public class AppSettings
  {
    public string Secret { get; set; }
    public int ExpiresHours { get; set; }
    public string Emitter { get; set; }
    public string ValidIn { get; set; }
  }
}