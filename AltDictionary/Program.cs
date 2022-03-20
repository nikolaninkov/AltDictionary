using Alt;

var dict = new AltDictionary<int, string>();
dict.Add(1, "Jedan");
dict.Add(2, "Dva");
dict.Add(3, "Tri");
Console.WriteLine(dict[1]);
Console.WriteLine(dict[2]);
Console.WriteLine(dict[3]);
