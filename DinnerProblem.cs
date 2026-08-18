using System;
using System.Threading;

class Fork
{
    public int ID { get; set; }

    public Mutex Mutex { get; set; }

}
class Guest
{
    public int ID { get; set; }
    private Fork leftFork;
    private Fork rightFork;

    public Guest(int id, Fork left, Fork right)
    {
        ID = id;
        leftFork = left;
        rightFork = right;
    }

    public void Dine()
    {
        while (true)
        {
            Think();
            Eat();
        }
    }

    private void Think()
    {
        Console.WriteLine($"Guest {ID} is thinking...");
        Thread.Sleep(new Random().Next(500, 1000));
    }

    private void Eat()
    {
        Fork first = leftFork.ID < rightFork.ID ? leftFork : rightFork;
        Fork second = leftFork.ID < rightFork.ID ? rightFork : leftFork;

        first.Mutex.WaitOne(); 
        second.Mutex.WaitOne(); 

        Console.WriteLine($"Guest {ID} is eating with forks {leftFork.ID} and {rightFork.ID}");
        Thread.Sleep(new Random().Next(500, 1000));

        second.Mutex.ReleaseMutex(); 
        first.Mutex.ReleaseMutex(); 

        Console.WriteLine($"Guest {ID} finished eating and released forks.");
    }
}

class Program
{
    static void Main()
    {
        int numGuests = 5;
        Fork[] forks = new Fork[numGuests];
        for (int i = 0; i < numGuests; i++)
        {
            forks[i] = new Fork { ID = i };
        }

        Thread[] threads = new Thread[numGuests];
        for (int i = 0; i < numGuests; i++)
        {
            Guest guest = new Guest(i, forks[i], forks[(i + 1) % numGuests]);
            threads[i] = new Thread(new ThreadStart(guest.Dine));
            threads[i].Start();
        }

        Console.ReadLine(); // keep program running
    }
}
