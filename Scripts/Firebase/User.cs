public class User
{
    public string username;
    public string email;
    public int coin;
    public int gem;
    public int level;
    public bool removeAd;
  

    public User(string username, string email, int coin,int gem,int level,bool removeAd)
    {
      
        this.username = username;
        this.email = email;
        this.coin = coin;
        this.gem = gem;
        this.level = level;
        this.removeAd = removeAd;   
     
   
    }
}
