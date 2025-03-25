namespace Eshop.Frontend.Utils;

public class CartService
{
    public event Action? OnCartChanged;

    public int CartItemsCount;
    //taky možnost s private setterem, který by zabraňoval měnit počet napřímo,
    //jelikož počet měním vždy pomocí UpdateCartCount není to potřeba

    public void UpdateCartCount(int newCount)
    {
        CartItemsCount = newCount;
        NotifyCartChanged();
    }

    public void NotifyCartChanged()
    {
        OnCartChanged?.Invoke();
    }

}

