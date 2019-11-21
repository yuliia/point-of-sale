# PointOfSale
PointOfSale is a simple library for Point of sale terminal.
PointOfSale targets .NET Standard 2.0


# Usage

```
PointOfSaleTerminal terminal = new PointOfSaleTerminal();
terminal.SetPrice("A", 1, 1.25m);
terminal.SetPrice("C", 1, 1m);
terminal.Scan("A");
terminal.Scan("C", 2);
decimal result = terminal.GetTotal(); // result will be 3.25
terminal.CloseCheck();
```

PointOfSaleTerminal allows to change price of an item without affecting check that are currently being scanned:

```
PointOfSaleTerminal terminal1 = new PointOfSaleTerminal();
terminal1.SetPrice("A", 1, 1.25m);
terminal1.Scan("A");
terminal1.SetPrice("A", 1, 1m);
terminal2 = new PointOfSaleTerminal();
terminal1.Scan("A");
terminal2.Scan("A");
decimal result1 = terminal1.GetTotal(); // result will be 2.5
decimal result2 = terminal2.GetTotal(); // result will be 1
```

## Cumulative discount
PointOfSaleTerminal now supports cumulative discounts.
Add discount to system before usage and then just scan it's code during sale:

```
PointOfSaleTerminal terminal = new PointOfSaleTerminal();
terminal.AddDiscount("my_discount_1", initialDiscount: 0.01);
terminal1.SetPrice("A", 1, 10m);

terminal.Scan("A");
terminal.Scan("my_discount_1");
decimal total = terminal.GetTotal(); // 9.9
```
By default initial discount is 0 and is accumulated during the sales by this rule:

| Amount    |  % |
| --------- |--- |
| 1000-1999 | 1% |
| 2000-4999 | 3% |
| 5000-9999 | 5% |
| over 9999 | 7% |


By default PointOfSale uses built-in in-memory storage. To use another storage simply initialize PointOfSaleTerminal with you IPricesStorage implementation

```
PointOfSaleTerminal terminal = new PointOfSaleTerminal(new MyDbStorage()); // where MyDbStorage implements IPricesStorage
```
