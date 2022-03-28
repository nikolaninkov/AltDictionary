using Alt;

//var altDict = new AltDictionary<int, int>();
//var regularDict = new Dictionary<int, int>();

//var startTimeAlt = DateTime.Now;

//for (int i = 0; i < 1000000; i++)
//{
//    altDict.Add(i, i);
//}
//var rand = new Random(startTimeAlt.Millisecond);
//var startTimeRandomAccessAlt = DateTime.Now;
//for (int i = 0; i < 5000000; i++)
//{
//    altDict.TryGetValue(rand.Next(0, 1000000), out _);
//}
//var endTimeRandomAccessAlt = DateTime.Now;
//Console.WriteLine("Time for random accesses, alt:");
//Console.WriteLine(endTimeRandomAccessAlt - startTimeRandomAccessAlt);
//for (int i = 0; i < 1000000; i++)
//{
//    altDict.Remove(i);
//}
//var endTimeAlt = DateTime.Now;
//Console.WriteLine("Total time, alt:");
//Console.WriteLine(endTimeAlt - startTimeAlt);

//var startTimeRegular = DateTime.Now;
//for (int i = 0; i < 1000000; i++)
//{
//    regularDict.Add(i, i);
//}
//var rand2 = new Random(startTimeRegular.Millisecond);
//var startTimeRandomAccessRegular = DateTime.Now;
//for (int i = 0; i < 5000000; i++)
//{
//    altDict.TryGetValue(rand2.Next(0, 1000000), out _);
//}
//var endTimeRandomAccessRegular = DateTime.Now;
//Console.WriteLine("Time for random accesses, regular:");
//Console.WriteLine(endTimeRandomAccessRegular - startTimeRandomAccessRegular);
//for (int i = 0; i < 1000000; i++)
//{
//    regularDict.Remove(i);
//}
//var endTimeRegular = DateTime.Now;
//Console.WriteLine("Total time, regular:");
//Console.WriteLine(endTimeRegular - startTimeRegular);

int min = 0;
int max = 1000000;

var startTime = DateTime.Now;
var tree = new RedBlackTree<int, int>();
for(int i = min; i < max; i++)
{
    tree.Add(i, i);
}
Console.WriteLine("added everything!");

var rand = new Random(startTime.Millisecond);
//for (int i = min; i < max; i++)
//{
//    tree.Contains(rand.Next(min, max));
//}
var r = rand.Next(min, max);
if (tree.Contains(r))
{
    Console.WriteLine("Found it!");
    Console.WriteLine("Key:{0}", r);
}

var endTime = DateTime.Now;
Console.WriteLine(endTime - startTime);