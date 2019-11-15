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


By default PointOfSale uses built-in in-memory storage. To use another storage simply initialize PointOfSaleTerminal with you IPricesStorage implementation

```
PointOfSaleTerminal terminal = new PointOfSaleTerminal(new MyDbStorage()); // where MyDbStorage implements IPricesStorage
```
