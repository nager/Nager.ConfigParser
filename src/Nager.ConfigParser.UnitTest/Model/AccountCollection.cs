namespace Nager.ConfigParser.UnitTest.Model
{
    public class AccountCollection
    {
        [ConfigKey("account.")]
        [ConfigArray]
        public Account[] Accounts { get; set; }
    }
}
