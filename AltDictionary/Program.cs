using Alt;

var altDict = new AltDictionary<int, int>();
var regularDict = new Dictionary<int, int>();

TimeSpan regularMax = TimeSpan.Zero;
TimeSpan altMax = TimeSpan.Zero;
var startTime = DateTime.Now;
var endTime = DateTime.Now;
var startTimeGet = DateTime.Now;
var endTimeGet = DateTime.Now;
int elementCount = 1000000;
int randomAccessCount = 5000000;

startTime = DateTime.Now;
for (int i = 0; i < elementCount; i++)
{
    altDict.Add(i, i);
}
endTime = DateTime.Now;
Console.WriteLine("Time to add elements into alt: {0}", endTime - startTime);

var rand = new Random(DateTime.Now.Millisecond);
startTime = DateTime.Now;
for (int i = 0; i < randomAccessCount; i++)
{
    startTimeGet = DateTime.Now;
    altDict.TryGetValue(rand.Next(0, 1000000), out _);
    endTimeGet = DateTime.Now;
    if (altMax < endTimeGet - startTimeGet)
    {
        altMax = endTimeGet - startTimeGet;
    }
}
endTime = DateTime.Now;
Console.WriteLine("Worst time for random access, alt: {0}", altMax);
Console.WriteLine("Total time for random access, alt: {0}", endTime - startTime);

startTime = DateTime.Now;
for (int i = 0; i < elementCount; i++)
{
    altDict.Remove(i);
}
endTime = DateTime.Now;
Console.WriteLine("Time to remove elements from alt: {0}", endTime - startTime);


startTime = DateTime.Now;
for (int i = 0; i < elementCount; i++)
{
    regularDict.Add(i, i);
}
endTime = DateTime.Now;
Console.WriteLine("Time to add elements into regular: {0}", endTime - startTime);

var rand2 = new Random(DateTime.Now.Millisecond);
startTime = DateTime.Now;
for (int i = 0; i < randomAccessCount; i++)
{
    startTimeGet = DateTime.Now;
    altDict.TryGetValue(rand2.Next(0, 1000000), out _);
    endTimeGet = DateTime.Now;
    if (regularMax < endTimeGet - startTimeGet)
    {
        regularMax = endTimeGet - startTimeGet;
    }
}
endTime = DateTime.Now;
Console.WriteLine("Total time for random access, regular: {0}", endTime - startTime);
Console.WriteLine("Worst time for random access, regular: {0}", regularMax);

startTime = DateTime.Now;
for (int i = 0; i < elementCount; i++)
{
    regularDict.Remove(i);
}
endTime = DateTime.Now;
Console.WriteLine("Time to remove elements from regular: {0}", endTime - startTime);