using System;
using System.Threading;

class Fork
{
    public int ID { get; set; }
    private bool available = true;
    private readonly object lockObj = new object();

    public bool TryPickUp()
    {
        lock (lockObj)
        {
            if (available)
            {
                available = false;
                return true;
            }
            return false;
        }
    }

    public void PutDown()
    {
        lock (lockObj)
        {
            available = true;
        }
    }
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
        if (leftFork.TryPickUp())
        {
            if (rightFork.TryPickUp())
            {
                Console.WriteLine($"Guest {ID} is eating with forks {leftFork.ID} and {rightFork.ID}");
                Thread.Sleep(new Random().Next(500, 1000));
                first.PutDown();
                second.PutDown();
                Console.WriteLine($"Guest {ID} finished eating and released forks.");
            }
            else
            {
                leftFork.PutDown();
            }
        }
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

        Console.ReadLine(); 
    }
}
