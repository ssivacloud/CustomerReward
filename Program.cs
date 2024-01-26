using CsvHelper;
using CsvHelper.Configuration;
using PurchasePoint;
using System.Globalization;
using System.Reflection;

class Program
{
    static void Main()
    {

        string csvpath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, @"Data\customer.csv");
        var customers = ReadcustomersFromCsv(csvpath);

        CalculateAndDisplayRewardPoints(customers);
    }

    static List<CustomerModel> ReadcustomersFromCsv(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            return csv.GetRecords<CustomerModel>().ToList();
        }
    }

    static void CalculateAndDisplayRewardPoints(List<CustomerModel> purchases)
    {
        
        int[] totalRewardPoints = new int[100]; 
        int[][] RewardPointsPerMonth = new int[12][];
        List<int> purchaseYear = new List<int>();
        int yearIndex = 0;
        for (int i = 0; i < 12; i++)
        {
            RewardPointsPerMonth[i] = new int[100];
        }

        foreach (var purchase in purchases)
        {
            int customerIndex = purchase.CustomerId - 1;

            int points = CalculateRewardPoints(purchase.PurchaseAmount);

            totalRewardPoints[customerIndex] += points;

            int monthIndex = purchase.PurchaseDate.Month - 1; 
            RewardPointsPerMonth[monthIndex][customerIndex] += points;
            if (purchaseYear.Count >= 1) {
                if(!purchaseYear.Contains(purchase.PurchaseDate.Year))
                { 
                    purchaseYear.Add(purchase.PurchaseDate.Year); 
                }
                    
            }
            else
            purchaseYear.Add(purchase.PurchaseDate.Year);
        }

      
        Console.WriteLine("Reward points per month:");
        for (int y = 0; y < purchaseYear.Count; y++)
        {
            for (int i = 0; i < 12; i++)
            {
                DateTime month = new DateTime(purchaseYear[y], i + 1, 1);
              
                for (int j = 0; j < 100; j++)
                {
                    
                    if (RewardPointsPerMonth[i][j] > 0)
                    {
                        Console.WriteLine($"{month.ToString("MMMM yyyy")}:");
                        Console.WriteLine($"  Customer {j + 1}: {RewardPointsPerMonth[i][j]} Reward points");
                    }
                }
            }
        }

        Console.WriteLine("\nTotal Reward points:");
        for (int i = 0; i < 100; i++)
        {
            if (totalRewardPoints[i] > 0)
            {
                Console.WriteLine($"Customer {i + 1}: {totalRewardPoints[i]} Reward points");
            }
        }
    }

    static int CalculateRewardPoints(decimal amount)
    {
        int points = 0;

        if (amount > 100)
        {
            points += (int)((amount - 100) * 2);
        }

        if (amount > 50)
        {
            points += (int)((Math.Min(amount, 100) - 50) * 1);
        }

        return points;
    }
}